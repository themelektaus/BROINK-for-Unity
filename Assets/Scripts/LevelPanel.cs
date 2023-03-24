using UnityEngine;

namespace BROINK
{
    public class LevelPanel : Panel
    {
        [SerializeField] LevelPanelItem item;

        public FSM_Ingame ingameFSM { get; private set; }

        void Awake()
        {
            ingameFSM = GetComponentInParent<FSM_Ingame>();
        }

        public override int GetItemCount()
        {
            return ingameFSM.botsCount;
        }

        public override GameObject CreateItem(int index)
        {
            var item = Instantiate(this.item);
            item.panel = this;
            item.level = index + 1;
            return item.gameObject;
        }
    }
}