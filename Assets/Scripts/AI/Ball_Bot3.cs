using UnityEngine;

namespace BROINK
{
    public class Ball_Bot3 : Ball_Bot
    {
        public override float speedOffset => AISettings.active.bot3SpeedOffset;
        public override float outwardsFactor => AISettings.active.bot3OutwardsFactor;

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