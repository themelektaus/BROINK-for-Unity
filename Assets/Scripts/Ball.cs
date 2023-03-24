using UnityEngine;

namespace BROINK
{
    public class Ball : MonoBehaviour
    {
        static float acceleration => GlobalSettings.active.ballAcceleration;
        static float radius => GlobalSettings.active.ballRadius;

        public Vector2 input;
        public SpriteRenderer spriteRenderer;

        [SerializeField] BallMessageSpawner messageSpawner;
        [SerializeField] SoundEffect hitSoundEffect;
        [SerializeField] SoundEffect dropSoundEffect;

        public BallColor color { get; private set; }

        public bool hasDropped { get; private set; }
        Vector2 dropDirection;
        float dropDelay = .1f;

        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        public Sprite icon { get; set; }

        public Vector2 GetPosition() => new(position.x * 100, position.y * -100);
        public Vector2 GetSpeed() => new Vector2(velocity.x, -velocity.y) / (Time.fixedDeltaTime / 1.2f);

        void Awake()
        {
            PhysicsSystem.balls.Add(this);
        }

        void OnDestroy()
        {
            PhysicsSystem.balls.Remove(this);
        }

        public Ball Instantiate(Vector2 position)
        {
            var ball = Instantiate(this, position, Quaternion.identity);
            ball.position = position;
            return ball;
        }

        public Ball AddAppearance(Ball_Appearance appearance)
        {
            var instance = Instantiate(appearance, transform);
            instance.ball = this;
            color = instance.color;
            return this;
        }

        public T CreatePlayer<T>(T player, PlayingField playingField) where T : Player
        {
            var instance = Instantiate(player, transform);
            instance.ball = this;
            instance.playingField = playingField;
            return instance;
        }

        public void NormalUpdate(float timeScale)
        {
            messageSpawner.icon = icon;

            if (!hasDropped)
            {
                transform.localScale = Vector3.one;
                return;
            }

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

            if (hasDropped)
            {
                position += dropDirection * (deltaTime * transform.localScale.x * 2);
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
            var ballRadius = GlobalSettings.active.ballRadius;
            var barrierWidth = GlobalSettings.active.barrierWidth;

            x = -ballRadius - barrierWidth;
            if (position.x < 0 && position.x > x)
                return true;

            x = ballRadius + barrierWidth;
            if (position.x > 0 && position.x < x)
                return true;

            x = 0;
            return false;
        }

        public void UpdateCollision(Ball other, bool transferImpact)
        {
            while (CollidesWith(other))
            {
                position -= velocity * Time.fixedDeltaTime;
                other.position -= other.velocity * Time.fixedDeltaTime;
            }

            if (!transferImpact)
                return;

            var impactDirection = (other.position - position).normalized;

            var energyTransfer = Vector2.Dot(velocity.normalized, impactDirection);
            var force = velocity.magnitude * energyTransfer * impactDirection;

            energyTransfer = Vector2.Dot(other.velocity.normalized, -impactDirection);
            force += other.velocity.magnitude * (energyTransfer + .1f) * impactDirection;

            velocity -= force;
            other.velocity += force;

            if (color != other.color)
            {
                if (!messageSpawner.visible)
                    icon = null;

                if (!other.messageSpawner.visible)
                    other.icon = null;
            }

            {
                var clipIndex = force.magnitude * 5;
                var position = this.position + impactDirection * GlobalSettings.active.ballRadius;
                var volume = clipIndex * 2;
                hitSoundEffect.Play(clipIndex, position, volume);
            }
        }

        public bool CollidesWith(Ball other)
        {
            if (hasDropped)
                return false;

            if (other.hasDropped)
                return false;

            return (other.position - position).magnitude < radius * 2;
        }

        public bool IsOutside(float radius)
        {
            return position.magnitude > radius + Ball.radius;
        }

        public void Drop(bool playSoundEffect)
        {
            if (hasDropped)
                return;

            hasDropped = true;
            dropDirection = velocity.normalized;

            if (playSoundEffect)
                dropSoundEffect.Play(position);            
        }
    }
}