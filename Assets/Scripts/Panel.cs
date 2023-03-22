using UnityEngine;

namespace BROINK
{
    public abstract class Panel : MonoBehaviour
    {
        public abstract int GetItemCount();
        public abstract GameObject CreateItem(int index);

        int itemCount;

        void Update()
        {
            Rebuild();
        }

        void Rebuild()
        {
            var itemCount = GetItemCount();
            if (this.itemCount == itemCount)
                return;

            foreach (Transform child in transform)
                Destroy(child.gameObject);

            this.itemCount = itemCount;
            for (int i = 0; i < itemCount; i++)
            {
                var item = CreateItem(i);
                item.transform.SetParent(transform);
                item.transform.localPosition = new(0, -i + (itemCount - 1) / 2f);
            }
        }
    }
}