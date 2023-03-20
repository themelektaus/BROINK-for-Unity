using UnityEngine;

namespace BROINK
{
    public class Player_Bot6 : Player_Bot
    {
        public override Config config => AISettings.active.bot6Config;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < AISettings.active.bot6ModeSwitchGameRadiusThreshold)
            {
                ModeOffensive(ref output);
                return;
            }

            if (playingField.barrier.enabled)
                ModeDefensive(ref output);
            else
                ModeCampCenter(ref output);

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}