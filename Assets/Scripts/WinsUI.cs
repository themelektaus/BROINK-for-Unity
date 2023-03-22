using UnityEngine;

namespace BROINK
{
    public class WinsPanel : Panel
    {
        [SerializeField] Game game;
        [SerializeField] WinUI winUI;

        public int wins;

        public override int GetItemCount()
        {
            return GameSettings.active.requiredWins;
        }

        public override GameObject CreateItem(int index)
        {
            var winUI = Instantiate(this.winUI);
            winUI.game = game;
            winUI.wins = wins;
            return winUI.gameObject;
        }
    }
}