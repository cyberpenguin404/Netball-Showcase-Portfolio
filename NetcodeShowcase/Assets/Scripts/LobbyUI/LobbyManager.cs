using PD4.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace Assets.Scripts.LobbyUI
{
    public class LobbyManager : MonobehaviourSingleton<LobbyManager>
    {
        private SessionInfo _activeSession;

        public SessionInfo ActiveSession
        {
            get { return _activeSession; }
            set
            {
                if (_activeSession == value) return;

                SessionInfo oldSession = _activeSession;
                _activeSession = value;

                if (oldSession != null)
                    oldSession.SessionEnded -= SessionEnded;

                if (_activeSession != null)
                    _activeSession.SessionEnded += SessionEnded;
            }
        }
        public event EventHandler SessionJoined;
        public event EventHandler SessionLeft;

        private const int _MAX_PLAYERS = 2;

        private const string _RELAYCONNECTIONTYPE = "wss"; //set to "wss" for web support

        protected override async void Awake()
        {
            base.Awake();
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        private async void Update()
        {
            if (ActiveSession != null)
            {
                await ActiveSession.UpdateSessionAsync();
            }
        }
        public async Task<List<Lobby>> QueryLobbiesAsync()
        {
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
            List<Lobby> lobbies = response.Results;
            return lobbies;
        }
        public async Task<Lobby> CreateLobbyAsync(string name)
        {
            //create the relay connection as host
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(_MAX_PLAYERS, "europe-west4");
            RelayServerData serverdata = alloc.ToRelayServerData(_RELAYCONNECTIONTYPE);

            //request the join code of the created relay connection
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            //create the lobby and store the joincode inside
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>() { { "relayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) } };
            Lobby createdLobby = await LobbyService.Instance.CreateLobbyAsync(name, _MAX_PLAYERS, options);

            //store the created session as the active session for this instance
            string playerID = AuthenticationService.Instance.PlayerId;
            SessionInfo newSession = new SessionInfo(createdLobby, playerID, serverdata);
            await newSession.InitializeAsync();
            ActiveSession = newSession;

            JoinSession();
            return createdLobby;
        }
        private void JoinSession()
        {
            if (ActiveSession == null) return;
            var transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            transport.SetRelayServerData(ActiveSession.RelayServerData);
            if (ActiveSession.IsHost)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }

            SessionJoined?.Invoke(this, EventArgs.Empty);
        }
        public async Task<Lobby> JoinLobbyAsync(Lobby lobby)
        {
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

            string relayJoinCode = joinedLobby.Data["relayJoinCode"].Value;
            JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            RelayServerData serverData = alloc.ToRelayServerData(_RELAYCONNECTIONTYPE);

            string playerID = AuthenticationService.Instance.PlayerId;
            SessionInfo newSession = new SessionInfo(joinedLobby, playerID, serverData);
            await newSession.InitializeAsync();
            ActiveSession = newSession;

            JoinSession();

            return joinedLobby;
        }
        public async Task LeaveLobbyAsync()
        {
            string playerID = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(ActiveSession.Lobby.Id, playerID);
            SessionLeft?.Invoke(this, EventArgs.Empty);
        }
        private void SessionEnded(object sender, EventArgs e)
        {
            if (_activeSession != null)
            {
                _activeSession = null;
            }
        }
    }
}
