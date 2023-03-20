using UnityEngine;

namespace BROINK
{
    public class Ball_Bot6 : Ball_Bot
    {
        public override float speedOffset => AISettings.active.bot6SpeedOffset;
        public override float outwardsFactor => AISettings.active.bot6OutwardsFactor;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < AISettings.active.bot6ModeSwitchGameRadiusThreshold)
            {
                ModeOffensive(ref output);
                return;
            }

            if (playingField.barrier.enabled)
            {
                if (!ModeOpening(ref output))
                    ModeOffensive(ref output);
            }
            else
            {
                ModeCampCenter(ref output);
            }

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}