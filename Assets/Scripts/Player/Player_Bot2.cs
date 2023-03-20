using UnityEngine;

namespace BROINK
{
    public class Player_Bot2 : Player_Bot
    {
        public override Config config => AISettings.active.bot2Config;

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

            mode = Mode.Offensive;
            output = playerOther_pos - playerSelf_pos;
            output += -playerSelf_speed * 4;

            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}