using UnityEngine;

namespace BROINK
{
    public class Ball_Bot3 : Ball_Bot
    {
        public override Vector2 GetInput()
        {
            if (playingField.lifetimeFactor < .4f)
                return GetInput_ModeOffensive();

            return GetInput_ModeDefensive();
        }
    }
}