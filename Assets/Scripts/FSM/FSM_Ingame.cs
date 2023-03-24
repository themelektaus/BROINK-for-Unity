using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BROINK
{
    public class FSM_Ingame : FSM
    {
        [SerializeField] GlobalSettings globalSettings;

        public int wins = 0;
        public int level = 1;

        public PlayingField playingField;

        [SerializeField] GameObject[] ingamePanels;

        public GameObject backButton;

        [SerializeField] Ball ball;
        [SerializeField] List<Ball_Appearance> ballAppearances;
        [SerializeField] List<Player_Human> humans;
        [SerializeField] List<Player_Bot> bots;
        [SerializeField] Ball_Icons ballIcons;

        [SerializeField] SoundEffect resetSoundEffect;

        public bool playAsWhite { get; set; }
        public bool playVsFriend { get; set; }

        public int botsCount => bots.Count;

        readonly List<Player> players = new();

        protected override void Awake()
        {
            GlobalSettings.active = globalSettings;

            base.Awake();
        }

        protected override void OnStart()
        {
            PhysicsSystem.ResetTimeScale();

            playingField.enabled = false;
            playingField.barrier.enabled = false;
            playingField.ResetTransform();

            resetSoundEffect.Play();

            backButton.SetActive(true);

            if (playVsFriend)
            {
                SetupBotVsBot_2on2();
                return;
            }

            SetupHumanVsBot();

            foreach (var panel in ingamePanels)
                panel.SetActive(HasOneHumanTeam());
        }

        protected override void OnExit()
        {
            foreach (var player in players)
                Destroy(player.ball.gameObject);

            players.Clear();

            foreach (var panel in ingamePanels)
                panel.SetActive(false);
        }

        public void SetupHumanVsHuman()
        {
            ball.Instantiate(new(-2, 0))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(humans[0], playingField)
                .AddTo(players);

            ball.Instantiate(new(2, 0))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(humans[1], playingField)
                .AddTo(players);
        }

        public void SetupHumanVsBot()
        {
            var human = humans[playAsWhite ? 1 : 0];
            var bot = bots[level - 1];

            var black = ball
                .Instantiate(new(-2, 0))
                .AddAppearance(ballAppearances[0]);

            var white = ball
                .Instantiate(new(2, 0))
                .AddAppearance(ballAppearances[1]);

            (playAsWhite ? white : black)
                .CreatePlayer(human, playingField)
                .AddTo(players);

            (playAsWhite ? black : white)
                .CreatePlayer(bot, playingField)
                .AddIcons(ballIcons)
                .AddTo(players);
        }

        public void SetupBotVsBot_1on1()
        {
            ball.Instantiate(new(-2, 0))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(bots[1], playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Instantiate(new(2, 0))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(bots[1], playingField)
                .AddIcons(ballIcons)
                .AddTo(players);
        }

        public void SetupBotVsBot_2on2()
        {
            Player_Bot GetRandomBot(params int[] numbers)
            {
                return bots[GameMakerFunctions.choose(numbers) - 1];
            }

            ball.Instantiate(new(-2, -1))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(GetRandomBot(2, 3, 4, 5, 6, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Instantiate(new(-2, 1))
                .AddAppearance(ballAppearances[0])
                .CreatePlayer(GetRandomBot(2, 4, 5, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Instantiate(new(2, -1))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(GetRandomBot(2, 3, 4, 5, 6, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);

            ball.Instantiate(new(2, 1))
                .AddAppearance(ballAppearances[1])
                .CreatePlayer(GetRandomBot(2, 4, 5, 7), playingField)
                .AddIcons(ballIcons)
                .AddTo(players);
        }

        public bool HasOneHumanTeam()
        {
            var humanTeams = players
                .GroupBy(x => x.ball.color)
                .ToDictionary(x => x.Key, x => x.Where(x => x is Player_Human).Count())
                .Where(x => x.Value > 0)
                .Count();

            return humanTeams == 1;
        }

        public bool AnyPlayerInput()
        {
            if (!players.Any(x => x is Player_Human))
                return true;

            foreach (var player in players)
                if (player is Player_Human && player.ball.input.sqrMagnitude > 0)
                    return true;

            return false;
        }

        public List<Player> GetPlayersOutside()
        {
            return players.Where(x => x.ball.IsOutside(playingField.radius)).ToList();
        }

        public List<Player> GetWinners()
        {
            var ballColorsAlive = players
                .GroupBy(x => x.ball.color)
                .ToDictionary(x => x.Key, x => x.Where(x => !x.ball.hasDropped).Count())
                .Where(x => x.Value > 0)
                .Select(x => x.Key)
                .ToHashSet();

            if (ballColorsAlive.Count > 1)
                return null;

            var winnerBallColor = ballColorsAlive.FirstOrDefault();

            return players
                .Where(x => x.ball.color == winnerBallColor)
                .ToList();
        }

        public void Score(bool hasHumanWinner)
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
        
            wins = Mathf.Min(GlobalSettings.active.requiredWins, wins + 1);
        
            if (wins < GlobalSettings.active.requiredWins)
                return;
        
            if (level == botsCount)
                return;
        
            wins = 0;
            level++;
        }
    }
}