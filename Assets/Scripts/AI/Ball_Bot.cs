using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(Ball))]
    public abstract class Ball_Bot : MonoBehaviour
    {
        public abstract Vector2 GetInput();

        protected Ball ball { get; private set; }

        public Ball opponentBall { get; set; }
        public PlayingField playingField { get; set; }

        void Awake()
        {
            ball = GetComponent<Ball>();
        }

        void Update()
        {
            ball.input = GetInput();
        }

        protected Vector2 GetInput_ModeOpening()
        {
            // TODO: Ball_Bot.GetInput_ModeOpening()

            return new();
        }

        protected Vector2 GetInput_ModeOffensive()
        {
            // TODO: Ball_Bot.GetInput_ModeOffensive()

            var oppenentPosition = opponentBall.position;
            var directionToOpponent = (oppenentPosition - ball.position).normalized;
            var offset = (opponentBall.velocity - ball.velocity) * (directionToOpponent.magnitude * .2f);
            oppenentPosition += Vector2.Angle(directionToOpponent, offset.normalized) / 90 * offset;
            return GetInputByDirection((oppenentPosition - ball.position).normalized, .1f);
        }

        protected Vector2 GetInput_ModeDefensive()
        {
            // TODO: Ball_Bot.GetInput_ModeDefensive()

            var predictedOpponentPosition = opponentBall.position + opponentBall.velocity * 10;

            var directionToCenter = -ball.position.normalized;
            var directionToOpponent = (predictedOpponentPosition - ball.position).normalized;

            var sidestepFactor = Mathf.Lerp(5, 10, 1 - playingField.lifetimeFactor);
            sidestepFactor = 1 - Mathf.Min(1, ball.velocity.magnitude * sidestepFactor);

            var targetAngle = directionToCenter.ToAngle() + 90 * sidestepFactor;
            var targetAngleDiff = Utils.AbsDeltaAngle(targetAngle, directionToOpponent);

            var angle = directionToCenter.ToAngle() - 90 * sidestepFactor;
            var angleDiff = Utils.AbsDeltaAngle(angle, directionToOpponent);
            if (targetAngleDiff < angleDiff)
                targetAngle = angle;

            return GetInputByDirection(targetAngle.ToDirectionVector(), .1f);
        }

        protected void CalculatePositionScore()
        {
            // TODO: Ball_Bot.CalculatePositionScore()
        }

        protected Vector2 GetInput_ModeCampCenter()
        {
            // TODO: Ball_Bot.GetInput_ModeCampCenter()
            return new();
        }

        protected void OutOfBoundsEmergencyBreak(ref Vector2 input)
        {
            // TODO: Ball_Bot.OutOfBoundsEmergencyBreak(ref ...)
        }

        Vector2 GetInputByDirection(Vector2 targetDirection, float maxVelocity)
        {
            // TODO: Ball_Bot.GetOffensiveInputByDirection(...)

            var ballDirection = ball.velocity.normalized;
            var input = targetDirection - ballDirection * (Vector2.Angle(ballDirection, targetDirection) / 180);
            var velocity = ball.velocity.magnitude;
            if (velocity > maxVelocity)
                input += -(ballDirection - Vector2.one * (velocity - maxVelocity));
            return input;
        }

        Vector2 GetOffensiveInputByDirection(Vector2 targetDirection, float maxVelocity)
        {
            // TODO: Ball_Bot.GetOffensiveInputByDirection(...)

            return GetInputByDirection(targetDirection, maxVelocity);
        }
    }
}