using UnityEngine;

namespace BROINK
{
    public class Player_Bot6 : Player_Bot
    {
        [SerializeField, Range(0, 500)]
        float modeSwitchGameRadiusThreshold = 200;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < modeSwitchGameRadiusThreshold)
            {
                ModeOffensive(ref output);
                return;
            }

            if (playingField.barrier.enabled)
            {
                ModeDefensive(ref output);
                OutOfBoundsEmergencyBreak(ref output);
                return;
            }

            if (playerOther_pos.sqrMagnitude > playerSelf_pos.sqrMagnitude)
                ModeCampCenter(ref output);
            else
                ModeDefensive(ref output);

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}