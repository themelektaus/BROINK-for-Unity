using UnityEngine;

namespace BROINK
{
    public class Player_Bot5 : Player_Bot
    {
        public override float speedOffset => AISettings.active.bot5SpeedOffset;
        public override float outwardsFactor => AISettings.active.bot5OutwardsFactor;

        public override void Process(ref Vector2 output)
        {
            if (playingField.barrier.enabled)
            {
                if (ModeOpening(ref output))
                    return;

                ModeOffensive(ref output);
                OutOfBoundsEmergencyBreak(ref output);
                return;
            }
            
            var score = CalculatePositionScore();
            if (score > AISettings.active.bot5MinPositionScore)
                ModeOffensive(ref output);
            else
                ModeDefensive(ref output, advanced: true);

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}