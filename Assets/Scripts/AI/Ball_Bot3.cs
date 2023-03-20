using UnityEngine;

namespace BROINK
{
    public class Ball_Bot3 : Ball_Bot
    {
        public override float speedOffset => 10;
        public override float outwardsFactor => 0;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < 300)
            {
                ModeOffensive(ref output);
                return;
            }

            ModeDefensive(ref output);
        }
    }
}