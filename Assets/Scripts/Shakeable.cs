using UnityEngine;

namespace BROINK
{
    public class Shakeable : MonoBehaviour
    {
        [SerializeField] float speed = 10;
        [SerializeField] float strength = 1;
        [SerializeField] float cooldown = 1;

        float amount;

        public void Shake() => amount++;
        
        void Update()
        {
            var position = transform.localPosition;
            var t = Time.time * speed;
            position.x = Mathf.PerlinNoise(position.x + t, position.y) - .5f;
            position.y = Mathf.PerlinNoise(position.x, position.y + t) - .5f;
            transform.localPosition = Vector3.Lerp(new(), position, amount * strength);

            amount = Mathf.Max(0, amount - Time.deltaTime * cooldown);
        }
    }
}