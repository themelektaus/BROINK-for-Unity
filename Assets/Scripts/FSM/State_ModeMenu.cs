using UnityEngine;

namespace BROINK
{
    public class State_ModeMenu : State<FSM_Menu>
    {
        [SerializeField] GameObject vsAiButton;
        [SerializeField] GameObject vsPlayerButton;

        public override void OnEnter(State previousState)
        {
            vsAiButton.SetActive(true);
            vsPlayerButton.SetActive(true);
        }

        public override void OnExit(State nextState)
        {
            vsAiButton.SetActive(false);
            vsPlayerButton.SetActive(false);
        }
    }
}