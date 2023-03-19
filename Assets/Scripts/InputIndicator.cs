using UnityEngine;

namespace BROINK
{
    public class InputIndicator : MonoBehaviour
    {
        [SerializeField] Ball ball;
        [SerializeField] SpriteRenderer _renderer;

        float velocity;

        void Update()
        {
            var rotation = transform.localEulerAngles;
            var targetRotation = ball.input.ToAngle();

            if (ball.input == Vector2.zero)
            {
                _renderer.enabled = false;
            }
            else
            {
                if (_renderer.enabled)
                {
                    rotation.z = Mathf.SmoothDampAngle(rotation.z, targetRotation, ref velocity, .05f);
                }
                else
                {
                    _renderer.enabled = true;
                    rotation.z = targetRotation;
                    velocity = 0;
                }
            }

            transform.localEulerAngles = rotation;
        }
    }
}