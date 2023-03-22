using UnityEngine;

namespace BROINK
{
    public class Player_Bot3 : Player_Bot
    {
        [SerializeField, Range(0, 500)]
        float modeSwitchGameRadiusThreshold = 300;

        public override void Process(ref Vector2 output)
        {
            if (gameRadius < modeSwitchGameRadiusThreshold)
            {
                ModeOffensive(ref output);
                return;
            }

            ModeDefensive(ref output);
        }
    }
}