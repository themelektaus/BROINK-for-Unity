using UnityEngine;

namespace BROINK
{
    public class Ball_Bot5 : Ball_Bot
    {
        public override float speedOffset => 10;
        public override float outwardsFactor => 30;

        public override void Process(ref Vector2 output)
        {
            if (playingField.barrier.enabled)
            {
                if (!ModeOpening(ref output))
                    ModeOffensive(ref output);
            }
            else
            {
                var score = CalculatePositionScore();
                if (score > 0)
                    ModeOffensive(ref output);
                else
                    ModeDefensive(ref output, advanced: true);
            }
            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}