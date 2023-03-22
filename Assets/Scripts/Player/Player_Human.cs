using UnityEngine;

namespace BROINK
{
    public class Player_Human : Player
    {
        [SerializeField] protected KeyCode up;
        [SerializeField] protected KeyCode left;
        [SerializeField] protected KeyCode right;
        [SerializeField] protected KeyCode down;

        void Update()
        {
            ball.input.x = 0;
            ball.input.x -= Input.GetKey(left) ? 1 : 0;
            ball.input.x += Input.GetKey(right) ? 1 : 0;

            ball.input.y = 0;
            ball.input.y += Input.GetKey(up) ? 1 : 0;
            ball.input.y -= Input.GetKey(down) ? 1 : 0;
        }
    }
}