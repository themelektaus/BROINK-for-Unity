using UnityEngine;

namespace BROINK
{
    public class BallMessageSpawner : MonoBehaviour
    {
        [SerializeField] BallMessage message;
        [SerializeField] float duration = 1;
        [SerializeField] float repeatTime = 3;

        public Sprite icon;

        float time;

        void Update()
        {
            time += Time.deltaTime;

            UpdateIcon();

            message.visible = message.icon && 0 < time && time < duration;
        }

        void UpdateIcon()
        {
            if (time < repeatTime)
            {
                if (time < .2f)
                    return;

                if (message.icon == icon)
                    return;
            }

            time = -.05f;
            message.icon = icon;
        }
    }
}