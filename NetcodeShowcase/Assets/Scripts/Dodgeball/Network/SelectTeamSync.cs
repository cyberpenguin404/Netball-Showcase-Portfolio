using Dodgeball.Model;
using Dodgeball.Presenter;
using System;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
    public class SelectTeamSync : NetworkBehaviour
    {
        NetworkVariable<PlayerColor> Player1Color;
        NetworkVariable<bool> Player1IsReady;
        NetworkVariable<PlayerColor> Player2Color;
        NetworkVariable<bool> Player2IsReady;

        public SelectTeamModel Model { get; set; }
        public SelectTeamPresenter Presenter { get; set; }

        private ulong player1Id;
        private ulong player2Id;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
                
            Initialize();
        }

        private void Model_ReadinessChanged(object sender, EventArgs e)
        {
            Player1IsReady.Value = Model.Player1Selection.IsReady;
            Player2IsReady.Value = Model.Player2Selection.IsReady;
        }

        private void Model_SelectionChanged(object sender, PlayerIdEventArgs e)
        {
            if (e.PlayerId == player1Id)
            {
                Player1Color.Value = Model.Player1Selection.SelectedColor;
            }
            else if (e.PlayerId == player2Id)
            {
                Player2Color.Value = Model.Player2Selection.SelectedColor;
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer && Model != null)
            {
                Model.SelectionChanged -= Model_SelectionChanged;
                Model.ReadinessChanged -= Model_ReadinessChanged;
            }
        }
        private void Awake()
        {
            Player1Color = new NetworkVariable<PlayerColor>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            Player2Color = new NetworkVariable<PlayerColor>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            Player1IsReady = new NetworkVariable<bool>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            Player2IsReady = new NetworkVariable<bool>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
        }
        public void Initialize()
        {
            if (!IsSpawned || Model == null) return;

            if (IsServer)
            {
                Model.SelectionChanged += Model_SelectionChanged;
                Model.ReadinessChanged += Model_ReadinessChanged;
            }

            player1Id = Model.Player1Selection.PlayerId; 
            player2Id = Model.Player2Selection.PlayerId;

            Model.Player1Selection.SelectedColor = Player1Color.Value;
            Model.Player2Selection.SelectedColor = Player2Color.Value;
            Model.Player1Selection.IsReady = Player1IsReady.Value;
            Model.Player2Selection.IsReady = Player2IsReady.Value;

            Player1Color.OnValueChanged += (old, value) => Model.SetSelection(player1Id, value);
            Player1IsReady.OnValueChanged += (old, value) => Model.SetReady(player1Id, value);
            Player2Color.OnValueChanged += (old, value) => Model.SetSelection(player2Id, value);
            Player2IsReady.OnValueChanged += (old, value) => Model.SetReady(player2Id, value);
        }
        [Rpc(SendTo.Server)]
        public void SetSelectedColorRPC(ulong clientID, PlayerColor color)
        {
            Model.SetSelection(clientID, color);
        }
        [Rpc(SendTo.Server)]
        public void SetReadyRPC(ulong clientID, bool ready)
        {
            Model.SetReady(clientID, ready);
        }
    }
}
