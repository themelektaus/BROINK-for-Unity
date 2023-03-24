using UnityEngine;

namespace BROINK
{
    public abstract class FSM : MonoBehaviour
    {
        public State startState;

        State currentState;

        protected virtual void OnStart() { }
        protected virtual void OnExit() { }

        void Awake()
        {
            foreach (Transform child in transform)
                if (!child.GetComponentInChildren<State>())
                    child.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            OnStart();
            currentState = startState;
            currentState.OnEnter(null);
        }

        void OnDisable()
        {
            currentState.OnExit(null);
            currentState = null;
            OnExit();
        }

        void Update()
        {
            currentState.OnUpdate();
        }

        void FixedUpdate()
        {
            currentState.OnFixedUpdate();
        }

        public void Transition(State nextState)
        {
            var previousState = currentState;
            currentState.OnExit(nextState);
            currentState = nextState;
            currentState.OnEnter(previousState);
        }

        public void Restart()
        {
            OnDisable();
            OnEnable();
        }
    }
}