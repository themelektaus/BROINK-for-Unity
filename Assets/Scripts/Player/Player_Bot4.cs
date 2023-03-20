using UnityEngine;

namespace BROINK
{
    public class Player_Bot4 : Player_Bot
    {
        public override Config config => AISettings.active.bot4Config;

        public override void Process(ref Vector2 output)
        {
            if (playingField.barrier.enabled)
            {
                if (ModeOpening(ref output))
                    return;

                ModeOpeningDodge(ref output);
                OutOfBoundsEmergencyBreak(ref output);
                return;
            }

            var score = CalculatePositionScore();
            if (score > AISettings.active.bot4MinPositionScore)
                ModeOffensive(ref output);
            else
                ModeDefensive(ref output, advanced: true);

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}