namespace BROINK
{
    public class LevelPanelItem : ReadonlyCheckbox
    {
        public LevelPanel panel { get; set; }

        public int level;

        public override bool IsChecked()
        {
            var level = this.level;

            if (panel.ingameFSM.wins >= Settings.active.requiredWins)
                level--;

            return panel.ingameFSM.level > level;
        }
    }
}