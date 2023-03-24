using UnityEngine;

namespace BROINK
{
    public class State_ReadyToPlay : State<FSM_Ingame>
    {
        [SerializeField] State nextState;
        
        public override void OnExit(State nextState)
        {
            fsm.playingField.enabled = true;
            fsm.playingField.barrier.enabled = true;
        }

        public override void OnUpdate()
        {
            if (fsm.AnyPlayerInput())
                Transition(nextState);
        }
    }
}