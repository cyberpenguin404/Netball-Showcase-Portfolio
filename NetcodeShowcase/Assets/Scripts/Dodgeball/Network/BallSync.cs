using Dodgeball.Model;
using Dodgeball.Presenter;
using System;
using Unity.Netcode;

public class BallSync : NetworkBehaviour
{
    public BallModel Model { get; set; }
    public BallPresenter Presenter {  get; set; }

    private NetworkVariable<bool> IsGrabbed = new();
    private NetworkVariable<bool> IsPlayerBall = new();
    private NetworkVariable<PlayerColor> LastGrabbedPlayerColor = new();

    private void Awake()
    {
        Model = GetComponent<BallPresenter>().Model;

        IsGrabbed = new NetworkVariable<bool>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
        IsPlayerBall = new NetworkVariable<bool>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
        LastGrabbedPlayerColor = new NetworkVariable<PlayerColor>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Model.Id = NetworkObjectId;

        if (IsServer)
        Model.PropertyChanged += Model_PropertyChanged;

        IsGrabbed.OnValueChanged += OnIsGrabChanged;
        IsPlayerBall.OnValueChanged += OnIsPlayerBallChanged;
        LastGrabbedPlayerColor.OnValueChanged += OnLastGrabbedPlayerColorChanged;


        Model.IsGrabbed = IsGrabbed.Value;
        Model.IsPlayerBall = IsPlayerBall.Value;
        Model.LastGrabbedPlayerColor = LastGrabbedPlayerColor.Value;
    }

    private void OnLastGrabbedPlayerColorChanged(PlayerColor previousValue, PlayerColor newValue)
    {
        Model.LastGrabbedPlayerColor = newValue;
    }

    private void OnIsPlayerBallChanged(bool previousValue, bool newValue)
    {
        Model.IsPlayerBall = newValue;
    }

    private void OnIsGrabChanged(bool previousValue, bool newValue)
    {
        Model.IsGrabbed = newValue;
    }

    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Model.IsGrabbed):
                IsGrabbed.Value = Model.IsGrabbed;
                break;
            case nameof(Model.IsPlayerBall):
                IsPlayerBall.Value = Model.IsPlayerBall;
                break;
            case nameof(Model.LastGrabbedPlayerColor):
                LastGrabbedPlayerColor.Value = Model.LastGrabbedPlayerColor;
                break;
            default:
                break;
        }
    }

    //[Rpc(SendTo.Server)]
    //public void SetGrabbedBallRpc(bool isGrabbed)
    //{
    //    Model.IsGrabbed = isGrabbed;
    //}

    //[Rpc(SendTo.Server)]
    //public void SetPlayerBallRpc(bool isPlayerBall)
    //{
    //    Model.IsPlayerBall = isPlayerBall;
    //}

    //[Rpc(SendTo.Server)]
    //public void SetLastGrabbedPlayerColorRpc(PlayerColor color)
    //{
    //    Model.LastGrabbedPlayerColor = color;
    //}
}