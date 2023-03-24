using UnityEngine;

namespace BROINK
{
    public class Deactivate : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);
            Destroy(this);
        }
    }
}