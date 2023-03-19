using UnityEngine;

namespace BROINK
{
    public class Ball_Bot2 : Ball_Bot
    {
        public override Vector2 GetInput()
        {
            return GetInput_ModeOffensive();
        }
    }
}