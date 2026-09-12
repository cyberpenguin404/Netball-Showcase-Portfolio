using Unity.Netcode;

namespace Dodgeball.Model.GameStates
{
	public class WaitingForOtherPlayerState : BaseGameState
	{
        private int _connectedPlayers = 0;
		private float _timer;//TODO: remove timer
		public WaitingForOtherPlayerState(GameStatesFSM fsm) : base(fsm)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected; 
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;

            // Initialize with whoever is already here (the host themselves)
            UpdatePlayerCount();
            //TODO: detect when players join -> 
            //When enough players are joined (2), go to the next state
            //FSM.TransitionTo(FSM.SelectTeamState);

            //FAKE waiting: wait 2 seconds
            //_timer = 2f;
        }
        private void HandleClientConnected(ulong clientId)
        {
            UpdatePlayerCount();
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            UpdatePlayerCount();
        }

        private void UpdatePlayerCount()
        {
            if (NetworkManager.Singleton != null)
            {
                _connectedPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
                if (_connectedPlayers >= 2)
                {
                    FSM.TransitionTo(FSM.SelectTeamState);
                }
            }
        }

    }
}
