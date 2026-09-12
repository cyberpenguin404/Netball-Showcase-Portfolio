using Dodgeball.Model;
using Dodgeball.Presenter;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
    public class PlayerSync : NetworkBehaviour
    {
        public PlayerModel Model { get; set; }
        public PlayerPresenter Presenter { get; set; }
        public PlayerThrow PlayerThrow { get; set; }
        public ArenaPresenter ArenaPresenter { get; set; }

        private const ulong NO_BALL_ID = 0;

        NetworkVariable<PlayerColor> PlayerColor;
        NetworkVariable<ulong> GrabbedBall;
        NetworkVariable<FixedString32Bytes> UserName;
        public NetworkVariable<FixedString32Bytes> PlayFabId { get; private set; }

        private bool _initialized;

        private void Awake()
        {
            PlayerColor = new NetworkVariable<PlayerColor>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            GrabbedBall = new NetworkVariable<ulong>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
            UserName = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
            PlayFabId = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialize();
        }
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            Presenter = GetComponent<PlayerPresenter>();
            PlayerThrow = GetComponent<PlayerThrow>();
            ArenaPresenter = FindAnyObjectByType<ArenaPresenter>();
            Presenter.ArenaPresenter = ArenaPresenter;
            Presenter.PlayerId = OwnerClientId;

            Model = ArenaPresenter.Model.GetPlayer(OwnerClientId);

            if (Model == null)
            {
                Model = new PlayerModel(OwnerClientId);
                ArenaPresenter.Model.AddPlayer(Model);
            }

            Presenter.Model = Model;
            ArenaPresenter.AddPlayerPresenter(Presenter);

            Initialize();

            Presenter.SetControlledByPlayer(OwnerClientId == NetworkManager.LocalClientId);

            _initialized = true;
        }
        public override void OnNetworkDespawn()
        {
            if (IsServer && Model != null)
            {
                Model.PropertyChanged -= Model_PropertyChanged;
            }

            PlayerColor.OnValueChanged -= OnPlayerColorChanged;
            GrabbedBall.OnValueChanged -= OnGrabbedBallNetworkChanged;

            base.OnNetworkDespawn();
        }
        public void Initialize()
        {
            if (!IsSpawned || Model == null) return;

            if (IsServer)
            {
                PlayerColor.Value = Model.Color;
                Model.PropertyChanged += Model_PropertyChanged;
            }

            PlayerColor.OnValueChanged += OnPlayerColorChanged;
            GrabbedBall.OnValueChanged += OnGrabbedBallNetworkChanged;

            if (!IsServer)
            {
                Model.Color = PlayerColor.Value;
                UpdateBallFromNetwork(GrabbedBall.Value);
            }

            UserName.OnValueChanged += UsernameChanged;
            PlayFabId.OnValueChanged += OnPlayFabIdNetworkChanged;

            if (IsOwner)
            {
                UserName.Value = PlayfabPlayer.Instance.DisplayName;
                PlayFabId.Value = PlayfabPlayer.PlayFabID;

                Model.UserName = PlayfabPlayer.Instance.DisplayName;
                Model.PlayFabId = PlayfabPlayer.PlayFabID;
            }
            else
            {
                Model.UserName = UserName.Value.ToString();
                Model.PlayFabId = PlayFabId.Value.ToString();
            }
        }

        private void OnPlayFabIdNetworkChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            Model.PlayFabId = newValue.ToString();
        }

        private void UsernameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            Model.UserName = newValue.ToString();
        }
        private void OnGrabbedBallNetworkChanged(ulong previousValue, ulong newValue)
        {
            UpdateBallFromNetwork(newValue);
        }
        private void UpdateBallFromNetwork(ulong ballId)
        {
            if (ballId == NO_BALL_ID)
            {
                Model.NetworkSetGrabbedBall(null);
            }
            else
            {
                BallPresenter ballPresenter = ArenaPresenter.GetBallPresenter(ballId);
                if (ballPresenter != null)
                {
                    Model.NetworkSetGrabbedBall(ballPresenter.Model);
                }
            }
        }
        private void OnPlayerColorChanged(PlayerColor previousValue, PlayerColor newValue)
        {
            Model.Color = newValue;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.Color))
            {
                PlayerColor.Value = Model.Color;
            }
            if (e.PropertyName == nameof(Model.GrabbedBall))
            {
                if (Model.GrabbedBall != null)
                    GrabbedBall.Value = Model.GrabbedBall.Id;
                else
                    GrabbedBall.Value = 0;
            }
        }

        [Rpc(SendTo.Server)]
        public void SetPlayerColorRpc(ulong clientID, PlayerColor color)
        {
            Model.Color = color;
        }
        [Rpc(SendTo.Server)]
        public void TryGrabBallRpc()
        {
            Model.TryGrabBall();
        }
        [Rpc(SendTo.Server)]
        public void TryThrowBallRpc()
        {
            if (!_initialized) return;

            UnityEngine.Vector3 unityVelocity = PlayerThrow.CalculateAimVelocity();

            if (Model.TryThrowBall(unityVelocity.ToNumericsVector()))
            {
                ThrowBallClientRpc(unityVelocity);
            }
        }
        [Rpc(SendTo.NotServer)]
        public void ThrowBallClientRpc(UnityEngine.Vector3 velocity)
        {
            Model.TryThrowBall(velocity.ToNumericsVector());
        }
    }
}
