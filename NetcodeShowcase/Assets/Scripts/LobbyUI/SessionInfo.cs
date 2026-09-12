using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Assets.Scripts.LobbyUI
{
    public class SessionInfo
    {
        private const float _HEARTBEAT_INTERVAL = 15f;
        public event EventHandler SessionEnded;
        private bool _isHeartbeating = false;

        public Lobby Lobby { get; private set; }
        public string LocalPlayerID { get; private set; }
        public RelayServerData RelayServerData { get; private set; }
        public bool IsHost { get; }

        private LobbyEventCallbacks _callbacks;
        private float _heartbeatTimer = _HEARTBEAT_INTERVAL;

        public SessionInfo(Lobby lobby, string playerID, RelayServerData relayServerData) 
        {
            Lobby = lobby;
            LocalPlayerID = playerID;
            RelayServerData = relayServerData;
            IsHost = Lobby.HostId == playerID;
        }
        public async Task InitializeAsync()
        {
            await UpdateLobbyInfoAsync();
            await RegisterCallbacksAsync();
        }
        public async Task UpdateLobbyInfoAsync()
        {
            Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);
        }
        public async Task RegisterCallbacksAsync()
        {
            _callbacks = new LobbyEventCallbacks();
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, _callbacks);
            _callbacks.LobbyDeleted += OnSessionEnded; ; //should invoke SessionEnded event
            _callbacks.KickedFromLobby += OnSessionEnded; //should invoke SessionEnded event
            _callbacks.PlayerJoined += OnPlayerJoinedAsync; ; //should call UpdateLobbyInfo
            _callbacks.PlayerLeft += OnPlayerLeftAsync; //should call UpdateLobbyInfo
        }

        private async void OnPlayerLeftAsync(List<int> obj)
        {
            await UpdateLobbyInfoAsync();
        }

        private async void OnPlayerJoinedAsync(List<LobbyPlayerJoined> obj)
        {
            await UpdateLobbyInfoAsync();
        }

        private void OnSessionEnded()
        {
            SessionEnded?.Invoke(this, new EventArgs());
        }
        public async Task UpdateSessionAsync()
        {
            if (!IsHost) return;
            if (_isHeartbeating) return;

            _heartbeatTimer += Time.deltaTime;
            if (_heartbeatTimer > _HEARTBEAT_INTERVAL)
            {
                _isHeartbeating = true;
                _heartbeatTimer -= _HEARTBEAT_INTERVAL;
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(Lobby.Id);
                }
                finally
                {
                    _isHeartbeating = false;
                }
            }
        }
        public async Task Leave()
        {
            string playerID = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, playerID);

            OnSessionEnded();
        }
    }
}
