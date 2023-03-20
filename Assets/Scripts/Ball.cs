using UnityEngine;

namespace BROINK
{
    public class Ball : MonoBehaviour
    {
        static float acceleration => GameSettings.active.ballAcceleration;
        static float radius => GameSettings.active.ballRadius;

        public Vector2 input;

        [SerializeField] SpriteRenderer sprite;
        [SerializeField] SoundEffect dropSoundEffect;

        AudioSource rollAudioSource;
        [SerializeField] Vector2 originPosition;
        Vector2? dropDirection;
        float dropDelay;

        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }

        public Vector2 GetPosition() => new(position.x * 100, position.y * -100);
        public Vector2 GetSpeed() => new Vector2(velocity.x, -velocity.y) / (Time.fixedDeltaTime / 1.2f);

        void Awake()
        {
            rollAudioSource = GetComponent<AudioSource>();
            originPosition = transform.localPosition;
        }

        public void CustomUpdate(float timeScale)
        {
            if (!dropDirection.HasValue)
            {
                var _velocity = velocity.magnitude;
                rollAudioSource.pitch = _velocity * 2 + .8f;
                rollAudioSource.volume = _velocity * 10 - .3f;
                transform.localScale = Vector3.one;
                return;
            }

            rollAudioSource.volume = 0;

            if (dropDelay > 0)
            {
                dropDelay -= Time.deltaTime;
                return;
            }

            var scale = transform.localScale;
            scale -= scale * (.95f * Time.deltaTime * timeScale * 3);
            transform.localScale = scale;
        }

        public void ResetBall()
        {
            dropDirection = null;
            dropDelay = .1f;
            position = originPosition;
            velocity = Vector2.zero;
        }

        public void UpdateTransformPosition()
        {
            transform.localPosition = position;
        }

        public void UpdatePhysics(float timeScale)
        {
            var deltaTime = Time.fixedDeltaTime * timeScale;

            if (dropDirection.HasValue)
            {
                position += dropDirection.Value * (deltaTime * transform.localScale.x * 2);
                return;
            }

            velocity += acceleration * deltaTime * input.normalized;
            position += velocity * timeScale;
        }

        public bool CollidesWith(Ball other)
        {
            if (dropDirection.HasValue)
                return false;

            if (other.dropDirection.HasValue)
                return false;

            return (other.position - position).magnitude < radius * 2;
        }

        public bool IsOutside(float radius)
        {
            return position.magnitude > radius + Ball.radius;
        }

        public void Drop(bool playSoundEffect)
        {
            if (playSoundEffect)
                dropSoundEffect.Play(position);

            if (!dropDirection.HasValue)
                dropDirection = velocity.normalized;
        }
    }
}