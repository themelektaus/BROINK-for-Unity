using UnityEngine;

namespace BROINK
{
    public class Framerate : MonoBehaviour
    {
        void OnEnable()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
        }

        void OnDisable()
        {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 0;
        }
    }
}