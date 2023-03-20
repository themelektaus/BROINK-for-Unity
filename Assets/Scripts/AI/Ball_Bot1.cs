using UnityEngine;

namespace BROINK
{
    public class Ball_Bot1 : Ball_Bot
    {
        public override float speedOffset => 10;
        public override float outwardsFactor => 0;

        public override void Process(ref Vector2 output)
        {
            output = -playerSelf_pos / 10;
            output += -playerSelf_speed * .2f;
        }
    }
}