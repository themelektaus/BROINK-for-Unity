using UnityEngine;

namespace BROINK
{
    using static GameMakerFunctions;

    public class Player_Bot : Player
    {
        public override bool isHuman => false;

        [System.Serializable]
        public class Config
        {
            [Range(0, 10)] public float speedOffset = 10;
            [Range(0, 100)] public float outwardsFactor;
        }
        public virtual Config config => new();

        public enum Mode { None, CampCenter, Offensive, Defensive, Emergency }
        protected Mode mode;

        public virtual void Process(ref Vector2 output)
        {

        }

        protected Vector2 playerSelf_pos => ball.GetPosition();
        protected Vector2 playerSelf_speed => ball.GetSpeed();

        protected Vector2 playerOther_pos => opponentBall.GetPosition();
        protected Vector2 playerOther_speed => opponentBall.GetSpeed();

        protected float gameRadius => playingField.radius * 100;

        static float player_acceleration => GameSettings.active.ballAcceleration;

        float opening_y;
        float? opening_dodge;

        protected override void Awake()
        {
            base.Awake();
            opening_y = Random.Range(
                AISettings.active.openingY.x,
                AISettings.active.openingY.y
            );
        }

        void Update()
        {
            mode = Mode.None;
            var output = new Vector2();
            if (playingField.enabled)
                Process(ref output);
            else if (ball.GetSpeed().magnitude > .5f)
                output = -ball.GetSpeed();
            ball.input = new(output.x, -output.y);
            ball.icon = mode switch
            {
                Mode.CampCenter => ball.icons.guard,
                Mode.Offensive => ball.icons.attack,
                Mode.Defensive => ball.icons.dodge,
                Mode.Emergency => ball.icons.warning,
                _ => null
            };
        }

        protected float CalculatePositionScore()
        {
            float score = 0;
            var selfSpeed = point_distance(new(), playerSelf_speed);
            var otherSpeed = point_distance(new(), playerOther_speed);
            var selfDirection = point_direction(new(), playerSelf_speed);
            var otherDirection = point_direction(new(), playerOther_speed);
            var selfDirectionToEnemy = point_direction(playerSelf_pos, playerOther_pos);
            var otherDirectionToEnemy = point_direction(playerOther_pos, playerSelf_pos);
            var selfDirectionAligned = (180 - (abs(angle_difference(selfDirectionToEnemy, selfDirection)) + 90)) / 90;
            var otherDirectionAligned = (180 - (abs(angle_difference(otherDirectionToEnemy, otherDirection)) + 90)) / 90;
            score += selfDirectionAligned * selfSpeed * 15;
            score -= otherDirectionAligned * otherSpeed * 15;
            return score * (800 / gameRadius);
        }

        protected bool ModeOpening(ref Vector2 output)
        {
            var current = playingField.barrier.currentLifetime;
            var max = GameSettings.active.barrierLifetime;
            if (current > max - AISettings.active.openingTotalDuration)
            {
                mode = Mode.Offensive;
                if (current > max - AISettings.active.openingBackstepDuration)
                    output = new(sign(playerSelf_pos.x), opening_y);
                return true;
            }
            return false;
        }

        protected void ModeOpeningDodge(ref Vector2 output)
        {
            opening_dodge ??= playerOther_speed.magnitude > AISettings.active.openingDodgeThreshold ? 1 : -1;
            if (opening_dodge.Value > 0)
                opening_dodge = Mathf.Max(0, opening_dodge.Value - Time.fixedDeltaTime);
            if (opening_dodge.Value == 0)
            {
                ModeDefensive(ref output, opening: true);
                return;
            }
            ModeOffensive(ref output);
        }

        protected void ModeCampCenter(ref Vector2 output)
        {
            mode = Mode.CampCenter;

            var enemydirfromcenter = point_direction(new(), playerOther_pos);
            var target = lengthdir(50, enemydirfromcenter);
            output = (target - playerSelf_pos) / 10 - playerSelf_speed / 2;
            output -= playerOther_speed;
            output = GetOutputByDirection(point_direction(new(), output), 100000);
        }

        protected void ModeOffensive(ref Vector2 output)
        {
            mode = Mode.Offensive;

            var distance = point_distance(playerSelf_pos, playerOther_pos);
            var target = playerOther_pos;
            var target_direction = point_direction(playerSelf_pos, target);

            var target_offset = (playerOther_speed - playerSelf_speed) * (distance * .2f);
            var offset_direction = point_direction(new(), target_offset);
            var angledifffactor = (90 - abs(abs(angle_difference(target_direction, offset_direction)) - 90)) / 90;
            target += target_offset * angledifffactor;

            target_direction = point_direction(playerSelf_pos, target);
            var enemyspeed = point_distance(playerOther_speed, new());
            output = GetOutputByDirection(target_direction, enemyspeed + config.speedOffset);
        }

        protected void ModeDefensive(ref Vector2 output, bool opening = false, bool advanced = false)
        {
            var enemy_speed = point_distance(playerOther_speed, new());
            if (advanced)
            {
                var otherDirection = point_direction(new(), playerOther_speed);
                var otherDirectionToEnemy = point_direction(playerOther_pos, playerSelf_pos);
                var otherDirectionAligned = (180 - (abs(angle_difference(otherDirectionToEnemy, otherDirection)) + 90)) / 90;
                if (otherDirectionAligned < 0)
                {
                    mode = Mode.Defensive;

                    var direction_to_enemy = point_direction(playerSelf_pos, playerOther_pos);
                    output = GetOutputByDirection(direction_to_enemy, enemy_speed + config.speedOffset);
                    return;
                }
            }

            mode = Mode.Defensive;

            var enemy_movedir = point_direction(new(), playerOther_speed);
            var own_speed = point_distance(playerSelf_speed, new());
            var enemy_distance = point_distance(playerSelf_pos, playerOther_pos);
            var predicted_enemy_pos = playerOther_pos + lengthdir(enemy_distance, enemy_movedir);

            var mytargetdir = point_direction(predicted_enemy_pos, playerSelf_pos);
            var my_direction_from_center = point_direction(new(), playerSelf_pos);

            var angle_diff = angle_difference(mytargetdir, my_direction_from_center);
            if (angle_diff == 0 || angle_diff == 180 || angle_diff == -180)
                angle_diff = choose(1, -1);
            var outwards_percentage = point_distance(new(), playerSelf_pos) / gameRadius;
            var hardness = opening ? AISettings.active.openingDefenseHardness : AISettings.active.defenseHardness;
            mytargetdir = my_direction_from_center + (90 + own_speed * hardness + outwards_percentage * config.outwardsFactor) * sign(angle_diff);

            output = GetOutputByDirection(mytargetdir, enemy_speed + config.speedOffset);
        }

        protected void OutOfBoundsEmergencyBreak(ref Vector2 output, bool advanced = false)
        {
            if (advanced)
            {
                var selfDirection = point_direction(new(), playerSelf_speed);
                var selfDirectionToEnemy = point_direction(playerSelf_pos, playerOther_pos);
                var selfDirectionAligned = (180 - (abs(angle_difference(selfDirectionToEnemy, selfDirection)) + 90)) / 90;
                var otherDirection = point_direction(new(), playerOther_speed);
                var otherDirectionToEnemy = point_direction(playerOther_pos, playerSelf_pos);
                var otherDirectionAligned = (180 - (abs(angle_difference(otherDirectionToEnemy, otherDirection)) + 90)) / 90;
                if (!(selfDirectionAligned < .9f || otherDirectionAligned < .9f))
                    return;
            }

            var fakexpos = playerSelf_pos.x;
            var fakeypos = playerSelf_pos.y;
            var fakexspeed = playerSelf_speed.x;
            var fakeyspeed = playerSelf_speed.y;
            var fakeradius = gameRadius - 10;
            var fakespeed = point_distance(0, 0, fakexspeed, fakeyspeed);
            var fakedirection = point_direction(0, 0, fakexspeed, fakeyspeed);
            var breakdirection = fakedirection;
            var outside = point_direction(new(), playerSelf_pos);
            var angle = angle_difference(breakdirection, outside);
            if (abs(angle) >= 90)
                breakdirection = outside + (180 - abs(angle)) * sign(angle) * .4f;
            else
                breakdirection = outside + (abs(angle)) * sign(angle) * .4f;
            fakexpos += fakexspeed;
            fakeypos += fakeyspeed;
            while (fakespeed > 0)
            {
                fakexpos += fakexspeed;
                fakeypos += fakeyspeed;
                // TODO: Make it better
                var acceleration = player_acceleration / AISettings.active.breakAccelerationFactor;
                fakespeed -= acceleration;
                fakexspeed -= lengthdir_x(acceleration, breakdirection);
                fakeyspeed -= lengthdir_y(acceleration, breakdirection);
                if (point_distance(0, 0, fakexpos, fakeypos) >= fakeradius - 40)
                {
                    mode = Mode.Emergency;
                    output = -lengthdir(10, breakdirection);
                    return;
                }
            }
        }

        Vector2 GetOutputByDirection(float _targetDirection, float _maxVelocity)
        {
            var currentDirection = point_direction(new(), playerSelf_speed);
            var directionsMatch = ((180 - (abs(angle_difference(currentDirection, _targetDirection)) + 90)) / 90 + 1) / 2;
            directionsMatch = lerp(directionsMatch, 0, .4f);
            var output = lengthdir(100, _targetDirection) * directionsMatch;
            output += -lengthdir(100, currentDirection) * (1 - directionsMatch);
            var velocity = point_distance(playerSelf_speed, new());
            if (velocity > _maxVelocity)
                output += -lengthdir(velocity - _maxVelocity, currentDirection) * 50;
            return output;
        }
    }
}