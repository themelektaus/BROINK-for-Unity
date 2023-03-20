using UnityEngine;

namespace BROINK
{
    public class Ball_Bot4 : Ball_Bot
    {
        public override float speedOffset => 6;
        public override float outwardsFactor => 50;

        public override void Process(ref Vector2 output)
        {
            if (playingField.barrier.enabled)
            {
                if (!ModeOpening(ref output))
                    ModeDefensive(ref output, advanced: true);
            }
            else
            {
                var score = CalculatePositionScore();
                if (score > 75)
                    ModeOffensive(ref output);
                else
                    ModeDefensive(ref output, advanced: true);
            }
            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}