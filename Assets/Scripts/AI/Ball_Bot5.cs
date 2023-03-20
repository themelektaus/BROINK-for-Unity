using UnityEngine;

namespace BROINK
{
    public class Ball_Bot5 : Ball_Bot
    {
        public override float speedOffset => AISettings.active.bot5SpeedOffset;
        public override float outwardsFactor => AISettings.active.bot5OutwardsFactor;

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
                if (score > AISettings.active.bot5MinPositionScore)
                    ModeOffensive(ref output);
                else
                    ModeDefensive(ref output, advanced: true);
            }
            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}