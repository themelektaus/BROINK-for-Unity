using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace BROINK
{
    public class Game : MonoBehaviour
    {
        // TODO: Game.MAX_LEVEL = 7
        public const int MAX_LEVEL = 3;

        [Range(0, 3)] public int wins;
        [Range(1, MAX_LEVEL)] public int level = 1;
        public bool mastered;

        [SerializeField] Ball blackBall;
        [SerializeField] Ball whiteBall;
        [SerializeField] PlayingField playingField;

        [Serializable]
        public class UI
        {
            public GameObject logo;
            public GameObject modeMenu;
            public GameObject colorMenu;
            public GameObject blackWins;
            public GameObject whiteWins;
        }
        [SerializeField] UI ui;

        [Serializable]
        public class SoundEffects
        {
            public SoundEffect reset;
            public SoundEffect ballHit;
            public SoundEffect ballBounce;
            public SoundEffect matchOver;
            public SoundEffect blackWin;
            public SoundEffect whiteWin;
        }
        [SerializeField] SoundEffects soundEffects;

        [SerializeField] UnityEvent onEscapeKey;

        public bool playVsFriend { get; set; }
        public bool playAsWhite { get; set; }

        Ball_Player player;

        public enum State { Menu, Ready, Playing, GameOver }
        State state;

        (float current, float target, float velocity, float smoothTime) timeScale;

        void Start()
        {
            SwitchToMenuState();
            ResetGame();
        }

        public void SwitchToMenuState()
        {
            state = State.Menu;
            playVsFriend = false;
            playAsWhite = false;
        }

        public void ResetGame()
        {
            timeScale = (1, 1, 0, .1f);

            soundEffects.reset.Play();

            playingField.enabled = false;
            playingField.barrier.enabled = false;
            playingField.ResetTransform();

            DestroyAdditionalComponents(blackBall);
            DestroyAdditionalComponents(whiteBall);

            blackBall.ResetBall();
            blackBall.UpdateTransformPosition();

            whiteBall.ResetBall();
            whiteBall.UpdateTransformPosition();

            ui.blackWins.SetActive(false);
            ui.whiteWins.SetActive(false);
        }

        public void StartGame()
        {
            state = State.Ready;

            if (playVsFriend)
            {
                player = null;
                blackBall.gameObject.AddComponent<Ball_Player1>();
                whiteBall.gameObject.AddComponent<Ball_Player2>();
                return;
            }

            Ball botBall;

            if (playAsWhite)
            {
                player = whiteBall.gameObject.AddComponent<Ball_Player2>();
                botBall = blackBall;
            }
            else
            {
                player = blackBall.gameObject.AddComponent<Ball_Player1>();
                botBall = whiteBall;
            }

            Ball_Bot bot;

            switch (level)
            {
                case 1: bot = botBall.gameObject.AddComponent<Ball_Bot1>(); break;
                case 2: bot = botBall.gameObject.AddComponent<Ball_Bot2>(); break;
                case 3: bot = botBall.gameObject.AddComponent<Ball_Bot3>(); break;
                case 4: bot = botBall.gameObject.AddComponent<Ball_Bot4>(); break;
                case 5: bot = botBall.gameObject.AddComponent<Ball_Bot5>(); break;
                case 6: bot = botBall.gameObject.AddComponent<Ball_Bot6>(); break;
                case 7: bot = botBall.gameObject.AddComponent<Ball_Bot7>(); break;
                default: return;
            }

            bot.opponentBall = player.ball;
            bot.playingField = playingField;
        }

        void DestroyAdditionalComponents(Ball ball)
        {
            if (ball.gameObject.TryGetComponent(out Ball_Player player))
                Destroy(player);

            if (ball.gameObject.TryGetComponent(out Ball_Bot bot))
                Destroy(bot);
        }

        void Update()
        {
            timeScale.current = Mathf.SmoothDamp(
                timeScale.current,
                timeScale.target,
                ref timeScale.velocity,
                timeScale.smoothTime
            );

            blackBall.CustomUpdate(timeScale.current);
            whiteBall.CustomUpdate(timeScale.current);

            if (state == State.Ready || state == State.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    onEscapeKey.Invoke();
                }
            }

            if (state == State.Ready)
            {
                if (Input.anyKeyDown)
                {
                    playingField.enabled = true;
                    state = State.Playing;
                }
            }
        }

        void FixedUpdate()
        {
            if (state == State.Menu || state == State.Ready)
                return;

            blackBall.UpdatePhysics(timeScale.current);
            whiteBall.UpdatePhysics(timeScale.current);

            if (playingField.barrier.enabled)
                UpdateBarrierCollision();

            if (blackBall.CollidesWith(whiteBall))
                UpdateBallCollision();

            blackBall.UpdateTransformPosition();
            whiteBall.UpdateTransformPosition();

            var blackBallLost = blackBall.IsOutside(playingField.radius);
            var whiteBallLost = whiteBall.IsOutside(playingField.radius);

            if (!blackBallLost && !whiteBallLost)
                return;

            if (blackBallLost)
                blackBall.Drop(state != State.GameOver);

            if (whiteBallLost)
                whiteBall.Drop(state != State.GameOver);

            if (state == State.GameOver)
                return;

            if (player)
            {
                if (
                    (!blackBallLost && player.ball == blackBall) ||
                    (!whiteBallLost && player.ball == whiteBall)
                )
                {
                    wins++;
                }
                else
                {
                    wins--;
                    mastered = false;
                }

                wins = Mathf.Clamp(wins, 0, 3);

                if (wins == 3)
                {
                    if (level == MAX_LEVEL)
                    {
                        mastered = true;
                    }
                    else
                    {
                        wins = 0;
                        level++;
                    }
                }
            }

            GameOver(
                blackWins: !blackBallLost,
                whiteWins: !whiteBallLost
            );
        }

        void UpdateBarrierCollision()
        {
            void Bounce(Ball ball, float positionX)
            {
                soundEffects.ballBounce.Play();

                playingField.barrier.Fizzle();

                var position = ball.position;
                position.x = positionX;
                ball.position = position;

                var velocity = ball.velocity;
                velocity.x = -velocity.x;
                ball.velocity = velocity;
            }

            var w = playingField.barrier.width / 2;

            if (blackBall.position.x > -blackBall.radius - w)
                Bounce(blackBall, -blackBall.radius - w);

            if (whiteBall.position.x < blackBall.radius + w)
                Bounce(whiteBall, blackBall.radius + w);
        }

        void UpdateBallCollision()
        {
            while (blackBall.CollidesWith(whiteBall))
            {
                blackBall.position -= blackBall.velocity * Time.fixedDeltaTime;
                whiteBall.position -= whiteBall.velocity * Time.fixedDeltaTime;
            }

            var impactDirection = (whiteBall.position - blackBall.position).normalized;

            var energyTransfer = Vector2.Dot(blackBall.velocity.normalized, impactDirection);
            var force = blackBall.velocity.magnitude * energyTransfer * impactDirection;

            energyTransfer = Vector2.Dot(whiteBall.velocity.normalized, -impactDirection);
            force += whiteBall.velocity.magnitude * (energyTransfer + .1f) * impactDirection;

            blackBall.velocity -= force;
            whiteBall.velocity += force;

            soundEffects.ballHit.Play(force.magnitude * 5, blackBall.position + impactDirection * blackBall.radius);
        }

        void GameOver(bool blackWins, bool whiteWins)
        {
            state = State.GameOver;

            soundEffects.matchOver.Play();
            
            if (playingField.TryGetComponent(out Shakeable shakeable))
                shakeable.Shake();

            playingField.enabled = false;

            IEnumerator _()
            {
                timeScale.target = .1f;
                yield return new WaitForSeconds(.5f);

                timeScale.smoothTime = 1;
                timeScale.target = 1;

                if (blackWins)
                {
                    soundEffects.blackWin.Play();
                    ui.blackWins.SetActive(true);
                }

                if (whiteWins)
                {
                    soundEffects.whiteWin.Play();
                    ui.whiteWins.SetActive(true);
                }

                yield return new WaitForSeconds(2);
                ResetGame();
                StartGame();
            }
            StartCoroutine(_());
        }
    }
}