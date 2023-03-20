using UnityEngine;

namespace BROINK
{
    public class Ball : MonoBehaviour
    {
        static float acceleration => GameSettings.active.ballAcceleration;
        static float radius => GameSettings.active.ballRadius;

        public Vector2 input;

        [SerializeField] SpriteRenderer sprite;
        [SerializeField] SoundEffect hitSoundEffect;
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
            PhysicsSystem.balls.Add(this);

            rollAudioSource = GetComponent<AudioSource>();
            originPosition = transform.localPosition;
        }

        void OnDestroy()
        {
            PhysicsSystem.balls.Remove(this);
        }

        public Player_Human AddPlayerHuman(int number) => number switch
        {
            1 => gameObject.AddComponent<Player_Human1>(),
            2 => gameObject.AddComponent<Player_Human2>(),
            _ => gameObject.AddComponent<Player_Human>()
        };

        public Player_Bot AddPlayerBot(int level) => level switch
        {
            1 => gameObject.AddComponent<Player_Bot1>(),
            2 => gameObject.AddComponent<Player_Bot2>(),
            3 => gameObject.AddComponent<Player_Bot3>(),
            4 => gameObject.AddComponent<Player_Bot4>(),
            5 => gameObject.AddComponent<Player_Bot5>(),
            6 => gameObject.AddComponent<Player_Bot6>(),
            7 => gameObject.AddComponent<Player_Bot7>(),
            _ => gameObject.AddComponent<Player_Bot>()
        };

        public void RemovePlayer()
        {
            if (gameObject.TryGetComponent(out Player player))
                Destroy(player);
        }

        public void ResetBall()
        {
            dropDirection = null;
            dropDelay = .1f;
            position = originPosition;
            velocity = Vector2.zero;
        }

        public void NormalUpdate(float timeScale)
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

        public void PhysicsUpdate(float timeScale)
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

        public void UpdateTransformPosition()
        {
            transform.localPosition = position;
        }

        public void UpdateCollision(Barrier barrier)
        {
            if (!CollidesWithBarrier(out var x))
                return;

            var position = this.position;
            position.x = x;
            this.position = position;

            var velocity = this.velocity;
            velocity.x = -velocity.x;
            this.velocity = velocity;

            barrier.BallBounce();
        }

        bool CollidesWithBarrier(out float x)
        {
            var ballRadius = GameSettings.active.ballRadius;
            var barrierWidth = GameSettings.active.barrierWidth;

            x = -ballRadius - barrierWidth;
            if (originPosition.x < 0 && position.x > x)
                return true;

            x = ballRadius + barrierWidth;
            if (originPosition.x > 0 && position.x < x)
                return true;

            x = 0;
            return false;
        }

        public void UpdateCollision(Ball other)
        {
            while (CollidesWith(other))
            {
                position -= velocity * Time.fixedDeltaTime;
                other.position -= other.velocity * Time.fixedDeltaTime;
            }

            var impactDirection = (other.position - position).normalized;

            var energyTransfer = Vector2.Dot(velocity.normalized, impactDirection);
            var force = velocity.magnitude * energyTransfer * impactDirection;

            energyTransfer = Vector2.Dot(other.velocity.normalized, -impactDirection);
            force += other.velocity.magnitude * (energyTransfer + .1f) * impactDirection;

            velocity -= force;
            other.velocity += force;

            hitSoundEffect.Play(force.magnitude * 5, position + impactDirection * GameSettings.active.ballRadius);
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