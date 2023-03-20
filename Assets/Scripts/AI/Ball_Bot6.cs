using UnityEngine;

namespace BROINK
{
    public class Ball_Bot6 : Ball_Bot
    {
        public override float speedOffset => 10;
        public override float outwardsFactor => 0;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < 200)
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