using UnityEngine;

namespace BROINK
{
    public class BallMessageSpawner : MonoBehaviour
    {
        [SerializeField] BallMessage message;
        [SerializeField] float duration = 1;

        public bool visible => message.icon && 0 < time && time < duration;

        public Sprite icon;

        float time;

        void Update()
        {
            time += Time.deltaTime;

            UpdateIcon();

            message.visible = visible;
        }

        void UpdateIcon()
        {
            if (time < .05f)
                return;

            if (message.icon == icon)
                return;

            time = -.05f;
            message.icon = icon;
        }
    }
}