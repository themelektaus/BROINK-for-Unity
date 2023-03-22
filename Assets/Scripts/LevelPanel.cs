using UnityEngine;

namespace BROINK
{
    public class LevelPanel : Panel
    {
        [SerializeField] Game game;
        [SerializeField] LevelPanelItem item;

        public override int GetItemCount()
        {
            return game.GetBotsCount();
        }

        public override GameObject CreateItem(int index)
        {
            var item = Instantiate(this.item);
            item.game = game;
            item.level = index + 1;
            return item.gameObject;
        }
    }
}