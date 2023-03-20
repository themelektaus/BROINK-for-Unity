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

        Player player1;
        Player player2;
        
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
                if (player1.isHuman || player2.isHuman)
                {
                    if (player1.isHuman && player1.ball.input.sqrMagnitude != 0)
                        return true;

                    if (player2.isHuman && player2.ball.input.sqrMagnitude != 0)
                        return true;

                    return false;
                }

                return true;
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

            if (player1.isHuman ^ player2.isHuman && blackBallLost ^ whiteBallLost)
            {
                if (player1.isHuman)
                {
                    if (!blackBallLost)
                        GameSettings.active.Win();
                    else
                        GameSettings.active.Lose();
                }
                else
                {
                    if (!whiteBallLost)
                        GameSettings.active.Win();
                    else
                        GameSettings.active.Lose();
                }
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

            blackBall.RemovePlayer();
            whiteBall.RemovePlayer();

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
                player1 = blackBall.AddPlayerHuman(1);
                player2 = whiteBall.AddPlayerHuman(2);
            }
            else if (playAsWhite)
            {
                player1 = blackBall.AddPlayerBot(GameSettings.active.level);
                player2 = whiteBall.AddPlayerHuman(2);
            }
            else
            {
                player1 = blackBall.AddPlayerHuman(1);
                player2 = whiteBall.AddPlayerBot(GameSettings.active.level);
            }

            player1.playingField = playingField;
            player1.opponentBall = player2.ball;

            player2.playingField = playingField;
            player2.opponentBall = player1.ball;
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