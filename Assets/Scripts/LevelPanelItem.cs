namespace BROINK
{
    public class LevelPanelItem : ReadonlyCheckbox
    {
        public Game game { get; set; }
        public int level;

        public override bool IsChecked()
        {
            var level = this.level;
            if (game.wins >= GameSettings.active.requiredWins)
                level--;

            return game.level > level;
        }
    }
}