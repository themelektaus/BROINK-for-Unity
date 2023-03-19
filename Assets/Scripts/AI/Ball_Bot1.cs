using UnityEngine;

namespace BROINK
{
    public class Ball_Bot1 : Ball_Bot
    {
        public override Vector2 GetInput()
        {
            return -ball.position;
        }
    }
}