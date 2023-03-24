using UnityEngine;

namespace BROINK
{
    public class State_ColorMenu : State<FSM_Menu>
    {
        [SerializeField] GameObject blackButton;
        [SerializeField] GameObject whiteButton;

        public override void OnEnter(State previousState)
        {
            blackButton.SetActive(true);
            whiteButton.SetActive(true);
        }

        public override void OnExit(State nextState)
        {
            blackButton.SetActive(false);
            whiteButton.SetActive(false);
        }
    }
}