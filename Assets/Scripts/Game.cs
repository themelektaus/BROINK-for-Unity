using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace BROINK
{
    public class Game : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;
        [SerializeField] AISettings aiSettings;

        [SerializeField] PlayingField playingField;

        public int wins;
        public int level = 1;

        [SerializeField] Ball ball;
        [SerializeField] List<Ball_Appearance> ballAppearances;
        [SerializeField] List<Player_Human> humans;
        [SerializeField] List<Player_Bot> bots;
        [SerializeField] Ball_Icons ballIcons;

        [System.Serializable]
        public class UI
        {
            public GameObject logo;
            public GameObject backButton;
        }
        [SerializeField] UI ui;

        [System.Serializable]
        public class SoundEffects
        {
            public SoundEffect reset;
            public SoundEffect ballHit;
            public SoundEffect matchOver;
        }
        [SerializeField] SoundEffects soundEffects;

        public bool playVsFriend { get; set; }
        public bool playAsWhite { get; set; }

        readonly List<Player> players = new();

        public enum State { Menu, Ready, Playing, GameOver }
        State state;

        public int GetBotsCount() => bots.Count;

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

            foreach (var player in players)
                Destroy(player.ball.gameObject);

            players.Clear();
        }

        public void StartGame()
        {
            ui.backButton.SetActive(true);

            state = State.Ready;

            if (playVsFriend)
            {
                //StartGame_PlayerVsPlayer();
                //StartGame_AIvsAI_1on1();
                StartGame_AIvsAI_2on2();
                return;
            }

            StartGame_PlayerVsAI();
        }

        void StartGame_PlayerVsPlayer()
        {
            ball.Spawn(new(-2, 0))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(humans[0], playingField)
                .AddTo(players);

            ball.Spawn(new(2, 0))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(humans[1], playingField)
                .AddTo(players);
        }

        Player_Bot GetRandomBot(params int[] numbers)
        {
            return bots[GameMakerFunctions.choose(numbers) - 1];
        }

        void StartGame_PlayerVsAI()
        {
            var human = humans[playAsWhite ? 1 : 0];
            var bot = bots[level - 1];

            var black = ball
                .Spawn(new(-2, 0))
                .AddAppearance(ballAppearances[0]);

            var white = ball
                .Spawn(new(2, 0))
                .AddAppearance(ballAppearances[1]);

            (playAsWhite ? white : black)
                .CreatePlayer(human, playingField)
                .AddTo(players);

            (playAsWhite ? black : white)
                .CreatePlayer(bot, playingField)
                .AddIcons(ballIcons)
                .AddTo(players);
        }

        void StartGame_AIvsAI_1on1()
        {
            ball.Spawn(new(-2, 0))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(bots[1], playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Spawn(new(2, 0))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(bots[1], playingField)
                .AddIcons(ballIcons)
                .AddTo(players);
        }

        void StartGame_AIvsAI_2on2()
        {
            ball.Spawn(new(-2, -1))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(GetRandomBot(2, 3, 4, 5, 6, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Spawn(new(-2, 1))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(GetRandomBot(2, 4, 5, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Spawn(new(2, -1))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(GetRandomBot(2, 3, 4, 5, 6, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Spawn(new(2, 1))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(GetRandomBot(2, 4, 5, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);
        }


        void FixedUpdate()
        {
            if (state != State.Playing && state != State.GameOver)
                return;

            PhysicsSystem.PhysicsUpdate(playingField.barrier);
        }

        void Update()
        {
            bool AnyPlayerInput()
            {
                if (!players.Any(x => x is Player_Human))
                    return true;

                foreach (var player in players)
                    if (player is Player_Human && player.ball.input.sqrMagnitude > 0)
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

            var playersOutside = players.Where(x => x.ball.IsOutside(playingField.radius)).ToList();

            foreach (var player in playersOutside)
                player.ball.Drop(state != State.GameOver);

            if (state == State.GameOver)
                return;

            Update_GameOver();
        }

        void Update_GameOver()
        {
            var ballColorsAlive = players
                .GroupBy(x => x.ball.color)
                .ToDictionary(x => x.Key, x => x.Where(x => !x.ball.hasDropped).Count())
                .Where(x => x.Value > 0)
                .Select(x => x.Key)
                .ToHashSet();

            if (ballColorsAlive.Count > 1)
                return;

            var winnerBallColor = ballColorsAlive.FirstOrDefault();

            var winners = players
                .Where(x => x.ball.color == winnerBallColor)
                .ToList();

            var winnerBallAppearance = winners
                .Select(x => x.ball.GetComponentInChildren<Ball_Appearance>())
                .FirstOrDefault();

            var hasHumanWinner = winners.Any(x => x is Player_Human);
            Score(hasHumanWinner);

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

                var winText = winnerBallAppearance
                    ? winnerBallAppearance.ShowWinText()
                    : null;
                yield return new WaitForSeconds(2);
                if (winText)
                    Destroy(winText);

                ResetGame();
                StartGame();
            }
            StartCoroutine(_());
        }

        void Score(bool hasHumanWinner)
        {
            if (!players.Any(x => x is Player_Human))
                return;

            if (!players.Any(x => x is Player_Bot))
                return;

            if (!hasHumanWinner)
            {
                wins = Mathf.Max(0, wins - 1);
                return;
            }

            wins = Mathf.Min(GameSettings.active.requiredWins, wins + 1);

            if (wins < GameSettings.active.requiredWins)
                return;

            if (level == GetBotsCount())
                return;

            wins = 0;
            level++;
        }
    }
}