using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputConfig", menuName = "Custom/PlayerInputActionSettings")]
public class PlayerInputActionConfig : ScriptableObject
{
    //Input actions
    public PlayerInputActions InputActions =>
        new PlayerInputActions()
        {
            MoveAction = _moveAction.action,
            LookAction = _lookAction.action,
            CrouchAction = _crouchAction.action,
            JumpAction  = _jumpAction.action,
            GrabBallAction = _grabBallAction.action,
            ThrowBallAction = _throwBallAction.action,
            ShownamesAction = _shownamesAction.action
        };

    [SerializeField]
    InputActionReference _moveAction, _lookAction, _jumpAction, _crouchAction, _grabBallAction, _throwBallAction, _shownamesAction;


    //Rotation settings
    public PlayerRotationSettings RotationSettings => _rotationSettings;

    [SerializeField]
	private PlayerRotationSettings _rotationSettings;
}

public struct PlayerInputActions
{
	public InputAction JumpAction { get; set; }
	public InputAction CrouchAction { get; set; }
	public InputAction MoveAction { get; set; }
	public InputAction LookAction { get; set; }
	public InputAction GrabBallAction { get; set; }
	public InputAction ThrowBallAction { get; set; }
    public InputAction ShownamesAction { get; set; }
}
