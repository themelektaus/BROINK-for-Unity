using UnityEngine;

namespace BROINK
{
    public class WinsPanel : Panel
    {
        [SerializeField] WinsPanelItem item;

        public FSM_Ingame ingameFSM { get; private set; }

        void Awake()
        {
            ingameFSM = GetComponentInParent<FSM_Ingame>();
        }

        public override int GetItemCount()
        {
            return GlobalSettings.active.requiredWins;
        }

        public override GameObject CreateItem(int index)
        {
            var item = Instantiate(this.item);
            item.panel = this;
            item.wins = index + 1;
            return item.gameObject;
        }
    }
}