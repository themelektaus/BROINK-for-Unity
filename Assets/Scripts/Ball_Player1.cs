using UnityEngine;

namespace BROINK
{
    public class Ball_Player1 : Ball_Player
    {
        protected override void Awake()
        {
            base.Awake();

            up = KeyCode.W;
            left = KeyCode.A;
            right = KeyCode.D;
            down = KeyCode.S;
        }
    }
}