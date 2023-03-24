using UnityEngine;

namespace BROINK
{
    public abstract class State<T> : State where T : FSM
    {
        protected new T fsm => base.fsm as T;
    }

    public abstract class State : MonoBehaviour
    {
        protected FSM fsm { get; private set; }

        protected virtual void Awake()
        {
            fsm = GetComponentInParent<FSM>();
        }

        public virtual void OnEnter(State previousState) { }
        public virtual void OnExit(State nextState) { }

        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }

        protected void Transition(State otherState)
        {
            fsm.Transition(otherState);
        }
    }
}