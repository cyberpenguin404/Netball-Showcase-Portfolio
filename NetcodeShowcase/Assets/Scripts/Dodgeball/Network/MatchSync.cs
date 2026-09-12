using Assets.Scripts.Backend;
using Dodgeball.Model;
using Dodgeball.Presenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Network
{
    public class MatchSync : NetworkBehaviour
    {
        public MatchPresenter Presenter { get; set; }
        public MatchModel Model { get; set; }

        public NetworkVariable<int> ScoreBlue;
        public NetworkVariable<int> ScoreRed;
        public NetworkVariable<float> SecondsLeft;

        public NetworkVariable<int> MatchID;

        private bool _matchCreatedInDB;

        private void Awake()
        {
            ScoreBlue = new NetworkVariable<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            ScoreRed = new NetworkVariable<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            SecondsLeft = new(MatchModel.MATCH_DURATION, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            MatchID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        }
        public override async void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            await InitializeAsync();
        }
        public async Task InitializeAsync()
        {
            if (!IsSpawned || Model == null) return;

            if (IsServer)
            {
                ScoreRed.Value = Model.ScoreRed;
                ScoreBlue.Value = Model.ScoreBlue;
                SecondsLeft.Value = Model.RawTimeLeft;

                Model.PropertyChanged += Model_PropertyChanged;

                Model.PlayerRed.PropertyChanged += OnPlayerModelPropertyChangedAsync;
                Model.PlayerBlue.PropertyChanged += OnPlayerModelPropertyChangedAsync;

                Model.PlayerRed.HitByPlayerBall += OnPlayerHitByBallAsync;
                Model.PlayerBlue.HitByPlayerBall += OnPlayerHitByBallAsync;

                await PostMatchAsync();
            }
            else
            {
                Model.ScoreRed = ScoreRed.Value;
                Model.ScoreBlue = ScoreBlue.Value;
                Model.UpdateClientTimeNetworked(SecondsLeft.Value);
            }

            ScoreRed.OnValueChanged += OnScoreRedChanged;
            ScoreBlue.OnValueChanged += OnScoreBlueChanged;
            MatchID.OnValueChanged += OnMatchIdSynced;
            Model.MatchEnded += OnMatchTimerExpired;
        }

        private async void OnPlayerModelPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlayerModel.PlayFabId))
            {
                await PostMatchAsync();
            }
        }

        private void OnMatchIdSynced(int previousValue, int newValue)
        {
            Model.MatchId = newValue;
        }
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Model.ScoreRed):
                    ScoreRed.Value = Model.ScoreRed;
                    break;
                case nameof(Model.ScoreBlue):
                    ScoreBlue.Value = Model.ScoreBlue;
                    break;
                case nameof(Model.MatchId):
                    MatchID.Value = Model.MatchId;
                    break;
            }
        }
        private void OnScoreRedChanged(int oldValue, int newValue)
        {
            if (!IsServer && Model != null) Model.ScoreRed = newValue;
        }

        private void OnScoreBlueChanged(int oldValue, int newValue)
        {
            if (!IsServer && Model != null) Model.ScoreBlue = newValue;
        }
        public async Task PostMatchAsync()
        {
            if (!IsServer || _matchCreatedInDB) return;


            string hostPlayFabId = PlayfabPlayer.PlayFabID == Model.PlayerRed.PlayFabId ? Model.PlayerRed.PlayFabId : Model.PlayerBlue.PlayFabId;
            string opponentPlayFabId = PlayfabPlayer.PlayFabID == Model.PlayerRed.PlayFabId ? Model.PlayerBlue.PlayFabId : Model.PlayerRed.PlayFabId;


            if (string.IsNullOrEmpty(hostPlayFabId) || string.IsNullOrEmpty(opponentPlayFabId)) return;

            _matchCreatedInDB = true;

            Debug.Log("[MatchSync] Triggering cloud match initialization ledger row...");
            var result = await BackendConnection.CreateMatchAsync(hostPlayFabId, opponentPlayFabId);

            if (result != null)
            {
                Model.MatchId = result.MatchID;
                Debug.Log($"[MatchSync] Database Registration Successful. Global ID Assigned: {Model.MatchId}");
            }
            else
            {
                _matchCreatedInDB = false;
                Debug.LogError("[MatchSync] Failed to register match layout instance inside Azure repository.");
            }
        }
        private async void OnPlayerHitByBallAsync(object sender, PlayerHitEventArgs e)
        {
            int currentMatchId = MatchID.Value;
            if (currentMatchId <= 0) return;

            string victimId = e.TargetPlayer.PlayFabId;
            string attackerId = (e.TargetPlayer.Color == PlayerColor.Blue) ? Model.PlayerRed.PlayFabId : Model.PlayerBlue.PlayFabId;


            await BackendConnection.RecordPlayerHitAsync(currentMatchId, attackerId, victimId);
        }
        private void OnMatchTimerExpired(object sender, EventArgs e)
        {
            if (!IsServer) return;
            TriggerGameOverClientRpc(MatchID.Value);
        }

        [Rpc(SendTo.Everyone)]
        private void TriggerGameOverClientRpc(int completedMatchId)
        {
            Model.GameOver();
        }
        public override void OnNetworkDespawn()
        {
            if (IsServer && Model != null)
            {
                Model.PlayerRed.HitByPlayerBall -= OnPlayerHitByBallAsync;
                Model.PlayerBlue.HitByPlayerBall -= OnPlayerHitByBallAsync;
                Model.MatchEnded -= OnMatchTimerExpired;
            }
            MatchID.OnValueChanged -= OnMatchIdSynced;
            base.OnNetworkDespawn();
        }
    }
}
