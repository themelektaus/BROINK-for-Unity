using System;
using System.Collections;

using UnityEngine;

namespace BROINK
{
    public class Game : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;
        [SerializeField] AISettings aiSettings;

        [SerializeField] Ball blackBall;
        [SerializeField] Ball whiteBall;
        [SerializeField] PlayingField playingField;

        [Serializable]
        public class UI
        {
            public GameObject logo;
            public GameObject blackWins;
            public GameObject whiteWins;
            public GameObject backButton;
        }
        [SerializeField] UI ui;

        [Serializable]
        public class SoundEffects
        {
            public SoundEffect reset;
            public SoundEffect ballHit;
            public SoundEffect matchOver;
            public SoundEffect blackWin;
            public SoundEffect whiteWin;
        }
        [SerializeField] SoundEffects soundEffects;

        public bool playVsFriend { get; set; }
        public bool playAsWhite { get; set; }

        Ball_Player player;

        public enum State { Menu, Ready, Playing, GameOver }
        State state;

        void Awake()
        {
            GameSettings.active = gameSettings;
            AISettings.active = aiSettings;
        }

        void Start()
        {
            SwitchToMenuState();
            ResetGame();
        }

        void FixedUpdate()
        {
            if (state == State.Playing || state == State.GameOver)
                PhysicsSystem.PhysicsUpdate(playingField.barrier);
        }

        void Update()
        {
            bool AnyPlayerInput()
            {
                if (player)
                    return player.ball.input.sqrMagnitude != 0;

                if (blackBall.input.sqrMagnitude != 0)
                    return true;

                if (whiteBall.input.sqrMagnitude != 0)
                    return true;

                return false;
            }

            PhysicsSystem.NormalUpdate();

            if (state == State.Ready)
            {
                if (AnyPlayerInput())
                {
                    playingField.enabled = true;
                    state = State.Playing;
                }
                return;
            }

            if (state == State.Menu)
                return;

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
                if (!blackBallLost && player.ball == blackBall)
                    GameSettings.active.Win();
                else if (!whiteBallLost && player.ball == whiteBall)
                    GameSettings.active.Win();
                else
                    GameSettings.active.Lose();
            }

            GameOver(
                blackWins: !blackBallLost,
                whiteWins: !whiteBallLost
            );
        }

        public void SwitchToMenuState()
        {
            state = State.Menu;
            playVsFriend = false;
            playAsWhite = false;
        }

        public void ResetGame()
        {
            PhysicsSystem.ResetTimeScale();

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
            ui.backButton.SetActive(true);
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

            switch (GameSettings.active.level)
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

        void GameOver(bool blackWins, bool whiteWins)
        {
            state = State.GameOver;

            ui.backButton.SetActive(false);

            soundEffects.matchOver.Play();

            if (playingField.TryGetComponent(out Shakeable shakeable))
                shakeable.Shake();

            playingField.enabled = false;

            IEnumerator _()
            {
                PhysicsSystem.StartSlowMotion();
                yield return new WaitForSeconds(.5f);
                PhysicsSystem.ExitSlowMotion();

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