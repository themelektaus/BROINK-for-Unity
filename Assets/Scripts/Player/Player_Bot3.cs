using UnityEngine;

namespace BROINK
{
    public class Player_Bot3 : Player_Bot
    {
        public override Config config => AISettings.active.bot3Config;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < AISettings.active.bot3ModeSwitchGameRadiusThreshold)
            {
                ModeOffensive(ref output);
                return;
            }

            ModeDefensive(ref output);
        }
    }
}