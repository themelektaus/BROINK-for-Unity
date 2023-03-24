using UnityEngine;

namespace BROINK
{
    public class ResetTransformPosition : MonoBehaviour
    {
        void Awake()
        {
            transform.localPosition = new();
            Destroy(this);
        }
    }
}