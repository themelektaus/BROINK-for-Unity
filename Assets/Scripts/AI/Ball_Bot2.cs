using UnityEngine;

namespace BROINK
{
    public class Ball_Bot2 : Ball_Bot
    {
        public override float speedOffset => 10;
        public override float outwardsFactor => 0;

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

            output = playerOther_pos - playerSelf_pos;
            output += -playerSelf_speed * 4;
            OutOfBoundsEmergencyBreak(ref output);
        }
    }
}