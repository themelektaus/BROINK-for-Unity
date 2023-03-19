using UnityEngine;

namespace BROINK
{
    public class Ball_Player2 : Ball_Player
    {
        protected override void Awake()
        {
            base.Awake();

            up = KeyCode.UpArrow;
            left = KeyCode.LeftArrow;
            right = KeyCode.RightArrow;
            down = KeyCode.DownArrow;
        }
    }
}