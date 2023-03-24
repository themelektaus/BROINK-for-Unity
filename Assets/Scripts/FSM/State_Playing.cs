using System.Linq;

using UnityEngine;

namespace BROINK
{
    public class State_Playing : State<FSM_Ingame>
    {
        [SerializeField] State nextState;
        [SerializeField] SoundEffect matchOverSoundEffect;

        public override void OnExit(State nextState)
        {
            fsm.playingField.enabled = false;
        }

        public override void OnUpdate()
        {
            PhysicsSystem.NormalUpdate();

            foreach (var player in fsm.GetPlayersOutside())
                player.ball.Drop(true);

            var winners = fsm.GetWinners();
            if (winners is null)
                return;

            var hasHumanWinner = winners.Any(x => x is Player_Human);
            fsm.Score(hasHumanWinner);

            fsm.backButton.SetActive(false);

            matchOverSoundEffect.Play();

            if (fsm.playingField.TryGetComponent(out Shakeable shakeable))
                shakeable.Shake();

            Transition(nextState);
        }

        public override void OnFixedUpdate()
        {
            PhysicsSystem.PhysicsUpdate(fsm.playingField.barrier);
        }
    }
}