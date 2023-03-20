using UnityEngine;

namespace BROINK
{
    public class Player_Human2 : Player_Human
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