namespace BROINK
{
    public class WinsPanelItem : ReadonlyCheckbox
    {
        public WinsPanel panel { get; set; }

        public int wins;

        public override bool IsChecked()
        {
            return panel.ingameFSM.wins >= wins;
        }
    }
}