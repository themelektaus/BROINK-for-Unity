using UnityEngine;

namespace BROINK
{
    public class Player_Human1 : Player_Human
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