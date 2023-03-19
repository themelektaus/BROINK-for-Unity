using UnityEngine;

namespace BROINK
{
    [RequireComponent(typeof(Ball))]
    public abstract class Ball_Player : MonoBehaviour
    {
        [SerializeField] protected KeyCode up;
        [SerializeField] protected KeyCode left;
        [SerializeField] protected KeyCode right;
        [SerializeField] protected KeyCode down;

        public Ball ball { get; private set; }

        protected virtual void Awake()
        {
            ball = GetComponent<Ball>();
        }

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