using System.Linq;

using UnityEngine;

namespace BROINK
{
    using static GameMakerFunctions;

    public class Player_Bot : Player
    {
        [SerializeField, Range(0, 10)] protected float speedOffset = 10;
        [SerializeField, Range(0, 100)] protected float outwardsFactor = 0;

        float GetSpeedOffset() => opponent is Player_Bot
            ? Mathf.Max(5, speedOffset)
            : speedOffset;

        public enum Mode { None, CampCenter, Offensive, Defensive, Emergency }
        public Mode mode { get; protected set; }

        protected Vector2 playerSelf_pos => ball.GetPosition();
        protected Vector2 playerSelf_speed => ball.GetSpeed();

        protected Vector2 playerOther_pos => opponent.ball.GetPosition();
        protected Vector2 playerOther_speed => opponent.ball.GetSpeed();

        protected float gameRadius => playingField.radius * 100;

        static float player_acceleration => GameSettings.active.ballAcceleration;

        float seed;

        Player opponent;
        float opponentTimer;

        float opening_y;
        float? opening_dodge;

        void Awake()
        {
            seed = Random.value * 100000;
            if (AISettings.active.randomOpening)
            {
                opening_y = Random.Range(
                    AISettings.active.openingY.x,
                    AISettings.active.openingY.y
                );
            }
        }

        public Player_Bot AddIcons(Ball_Icons icons)
        {
            var instance = Instantiate(icons, transform);
            instance.bot = this;
            return this;
        }

        public virtual void Process(ref Vector2 output) { }

        void Update()
        {
            UpdateOpponent();

            mode = Mode.None;
            var output = new Vector2();
            if (playingField.enabled)
            {
                Process(ref output);
                ProcessRandomRotate(ref output);
            }
            else if (ball.GetSpeed().magnitude > .5f)
            {
                output = -ball.GetSpeed();
            }
            ball.input = new(output.x, -output.y);
        }

        void UpdateOpponent()
        {
            var opponents = players
                .Where(x => x.ball.color != ball.color)
                .OrderBy(x => x.ball.hasDropped);

            var opponent = opponents.FirstOrDefault();
            if (!opponent || opponent.ball.hasDropped)
            {
                this.opponent = opponent;
                return;
            }

            if (this.opponent && this.opponent.ball.hasDropped)
                opponentTimer = 0;

            if (opponentTimer > 0)
            {
                opponentTimer -= Time.deltaTime;
                return;
            }

            opponentTimer = GameSettings.active.barrierLifetime + 1 + Random.value * 3;
            this.opponent = choose(opponents.Where(x => !x.ball.hasDropped).ToArray());
        }

        void ProcessRandomRotate(ref Vector2 output)
        {
            if (opponent is not Player_Bot)
                return;

            var distance = point_distance(playerSelf_pos, playerOther_pos);
            var speed = playerSelf_speed.magnitude + playerOther_speed.magnitude;
            var f = Mathf.Lerp(1, 5, (200 - distance) / 200 * Mathf.Max(0, 4 - speed));
            var angle = AISettings.active.randomRotationRangeAgainstAI * f;
            var degrees = Mathf.PerlinNoise1D(Time.time + seed) * angle - angle / 2;
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            float tx = output.x;
            float ty = output.y;
            output.x = (cos * tx) - (sin * ty);
            output.y = (sin * tx) + (cos * ty);
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
            output = GetOutputByDirection(target_direction, enemyspeed + GetSpeedOffset());
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
                    output = GetOutputByDirection(direction_to_enemy, enemy_speed + GetSpeedOffset());
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
            mytargetdir = my_direction_from_center + (90 + own_speed * hardness + outwards_percentage * outwardsFactor) * sign(angle_diff);

            output = GetOutputByDirection(mytargetdir, enemy_speed + GetSpeedOffset());
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