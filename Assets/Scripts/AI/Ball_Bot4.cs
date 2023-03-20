using UnityEngine;

namespace BROINK
{
    public class Ball_Bot4 : Ball_Bot
    {
        public override float speedOffset => AISettings.active.bot4SpeedOffset;
        public override float outwardsFactor => AISettings.active.bot4OutwardsFactor;

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
                if (score > AISettings.active.bot4MinPositionScore)
                    ModeOffensive(ref output);
                else
                    ModeDefensive(ref output, advanced: true);
            }
            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}