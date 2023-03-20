using UnityEngine;

namespace BROINK
{
    public class Player_Bot6 : Player_Bot
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
                if (ModeOpening(ref output))
                    return;

                ModeOpeningDodge(ref output);
                OutOfBoundsEmergencyBreak(ref output);
                return;
            }

            ModeCampCenter(ref output);

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}