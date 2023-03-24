using UnityEngine;

namespace BROINK
{
    public class PositionOffset : MonoBehaviour
    {
        [SerializeField] Vector3 offset;

        void Awake()
        {
            transform.localPosition -= offset;
            Destroy(this);
        }
    }
}