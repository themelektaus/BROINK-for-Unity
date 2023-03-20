using UnityEngine;

namespace BROINK
{
    public class Ball_Bot1 : Ball_Bot
    {
        public override void Process(ref Vector2 output)
        {
            output = -playerSelf_pos / 10;
            output += -playerSelf_speed * AISettings.active.bot1SpeedDamping;
        }
    }
}