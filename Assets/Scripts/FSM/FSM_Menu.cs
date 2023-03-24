using UnityEngine;

namespace BROINK
{
    public class FSM_Menu : FSM
    {
        [SerializeField] GameObject logo;
        
        protected override void OnStart()
        {
            logo.SetActive(true);
        }

        protected override void OnExit()
        {
            logo.SetActive(false);
        }
    }
}