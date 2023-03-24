using System.Collections;
using System.Linq;

using UnityEngine;

namespace BROINK
{
    public class State_GameOver : State<FSM_Ingame>
    {
        public override void OnEnter(State previousState)
        {
            var winners = fsm.GetWinners();

            var winnerBallAppearance = winners
                .Select(x => x.ball.GetComponentInChildren<Ball_Appearance>())
                .FirstOrDefault();

            IEnumerator _()
            {
                PhysicsSystem.StartSlowMotion();
                yield return new WaitForSeconds(.5f);
                PhysicsSystem.ExitSlowMotion();

                var winText = winnerBallAppearance
                    ? winnerBallAppearance.ShowWinText()
                    : null;

                yield return new WaitForSeconds(2);

                if (winText)
                    Destroy(winText);

                fsm.Restart();
            }
            StartCoroutine(_());
        }

        public override void OnUpdate()
        {
            PhysicsSystem.NormalUpdate();

            foreach (var player in fsm.GetPlayersOutside())
                player.ball.Drop(false);
        }

        public override void OnFixedUpdate()
        {
            PhysicsSystem.PhysicsUpdate(fsm.playingField.barrier);
        }
    }
}