using System.Linq;
using Unity.Netcode;

namespace Dodgeball.Model.GameStates
{
	public class SelectTeamState : BaseGameState
	{

		public SelectTeamState(GameStatesFSM fsm) : base(fsm)
		{

		}

		public override void OnEnter()
		{
			base.OnEnter();

            ulong localId = NetworkManager.Singleton.LocalClientId;

            ulong otherId = NetworkManager.Singleton.ConnectedClientsIds.First(id => id != localId);

            ulong player1Id = NetworkManager.Singleton.IsHost ? localId : otherId;
            ulong player2Id = NetworkManager.Singleton.IsHost ? otherId : localId;

            GameModel.TeamSelection = new SelectTeamModel(player1Id, player2Id);
			GameModel.TeamSelection.ReadinessChanged += TeamSelection_ReadinessChanged;
		}

		private void TeamSelection_ReadinessChanged(object sender, System.EventArgs e)
		{
			if (GameModel.TeamSelection.ReadyToPlay)
			{
				FSM.TransitionTo(FSM.CountdownState);
			}
		}
	}
}
