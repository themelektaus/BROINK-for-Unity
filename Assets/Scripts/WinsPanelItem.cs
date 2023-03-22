namespace BROINK
{
    public class WinsPanelItem : ReadonlyCheckbox
    {
        public Game game { get; set; }
        public int wins;

        public override bool IsChecked()
        {
            return game.wins >= wins;
        }
    }
}