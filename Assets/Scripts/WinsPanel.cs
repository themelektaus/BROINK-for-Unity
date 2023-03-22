using UnityEngine;

namespace BROINK
{
    public class WinsPanel : Panel
    {
        [SerializeField] Game game;
        [SerializeField] WinsPanelItem item;

        public override int GetItemCount()
        {
            return GameSettings.active.requiredWins;
        }

        public override GameObject CreateItem(int index)
        {
            var item = Instantiate(this.item);
            item.game = game;
            item.wins = index + 1;
            return item.gameObject;
        }
    }
}