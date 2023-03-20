using UnityEngine;

namespace BROINK
{
    using static GameMakerFunctions;

    [RequireComponent(typeof(Ball))]
    public abstract class Ball_Bot : MonoBehaviour
    {
        public abstract float speedOffset { get; }
        public abstract float outwardsFactor { get; }

        public abstract void Process(ref Vector2 output);

        protected Ball ball { get; private set; }

        public Ball opponentBall { get; set; }
        public PlayingField playingField { get; set; }

        protected Vector2 playerSelf_pos => ball.GetPosition();
        protected Vector2 playerSelf_speed => ball.GetSpeed();

        protected Vector2 playerOther_pos => opponentBall.GetPosition();
        protected Vector2 playerOther_speed => opponentBall.GetSpeed();

        protected float gameRadius => playingField.radius * 100;

        float player_acceleration => ball.acceleration;

        protected virtual void Awake()
        {
            ball = GetComponent<Ball>();
        }

        void Update()
        {
            var output = new Vector2();
            Process(ref output);
            ball.input = new(output.x, -output.y);
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
            var max = playingField.barrier.maxLifetime;
            if (current > max - .75f)
            {
                if (current > max - .4f)
                {
                    var opening_y = 1 - Random.value * 2;
                    output = new(sign(playerSelf_pos.x), opening_y);
                }
                return true;
            }
            return false;
        }

        protected void ModeCampCenter(ref Vector2 output)
        {
            var enemydirfromcenter = point_direction(new(), playerOther_pos);
            var target = lengthdir(50, enemydirfromcenter);
            output = (target - playerSelf_pos) / 10 - playerSelf_speed / 2;
            output -= playerOther_speed;
            output = GetOutputByDirection(point_direction(new(), output), 100000);
        }

        protected void ModeOffensive(ref Vector2 output)
        {
            var distance = point_distance(playerSelf_pos, playerOther_pos);
            var target = playerOther_pos;
            var target_direction = point_direction(playerSelf_pos, target);

            var target_offset = (playerOther_speed - playerSelf_speed) * (distance * .2f);
            var offset_direction = point_direction(new(), target_offset);
            var angledifffactor = (90 - abs(abs(angle_difference(target_direction, offset_direction)) - 90)) / 90;
            target += target_offset * angledifffactor;

            target_direction = point_direction(playerSelf_pos, target);
            var enemyspeed = point_distance(playerOther_speed, new());
            output = GetOutputByDirection(target_direction, enemyspeed + speedOffset);
        }

        protected void ModeDefensive(ref Vector2 output, bool advanced = false)
        {
            var enemy_speed = point_distance(playerOther_speed, new());
            if (advanced)
            {
                var otherDirection = point_direction(new(), playerOther_speed);
                var otherDirectionToEnemy = point_direction(playerOther_pos, playerSelf_pos);
                var otherDirectionAligned = (180 - (abs(angle_difference(otherDirectionToEnemy, otherDirection)) + 90)) / 90;
                if (otherDirectionAligned < 0)
                {
                    var direction_to_enemy = point_direction(playerSelf_pos, playerOther_pos);
                    output = GetOutputByDirection(direction_to_enemy, enemy_speed + speedOffset);
                    return;
                }
            }

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
            mytargetdir = my_direction_from_center + (90 + own_speed * 4 + outwards_percentage * outwardsFactor) * sign(angle_diff);

            output = GetOutputByDirection(mytargetdir, enemy_speed + speedOffset);
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
                var acceleration = player_acceleration / 1.125f;
                fakespeed -= acceleration;
                fakexspeed -= lengthdir_x(acceleration, breakdirection);
                fakeyspeed -= lengthdir_y(acceleration, breakdirection);
                if (point_distance(0, 0, fakexpos, fakeypos) >= fakeradius - 50)
                {
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