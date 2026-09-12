using System;
using UnityEngine;
public class PlayerInputMoveStrategy : IPlayerMoveStrategy
{
	private readonly Transform _playerTransform;
	private readonly Transform _cameraTransform;

	private readonly PlayerInputActions _inputActions;
	private readonly PlayerRotationSettings _rotationSettings;

	public event EventHandler CrouchStarted;
	public event EventHandler CrouchEnded;
	public event EventHandler JumpRequested;

	private float _lookRotX = 0f;
	private float _lookRotY = 0f;

	public PlayerInputMoveStrategy(Transform playerTransform, Transform cameraTransform,
		PlayerInputActions inputActions, PlayerRotationSettings rotSettings)
	{
		inputActions.JumpAction.Enable();
		inputActions.CrouchAction.Enable();
		inputActions.MoveAction.Enable();
		inputActions.LookAction.Enable();

		inputActions.JumpAction.performed += (e) => OnJumpRequested();
		inputActions.CrouchAction.started += (e) => OnCrouchStarted();
		inputActions.CrouchAction.canceled += (e) => OnCrouchEnded();


        _playerTransform = playerTransform;
		_cameraTransform = cameraTransform;
		_inputActions = inputActions;
		_rotationSettings = rotSettings;

		_lookRotY = _playerTransform.rotation.eulerAngles.y;
	}

    public Vector3 CalculateMovement()
	{
		Vector2 moveInput = _inputActions.MoveAction.ReadValue<Vector2>();
		Vector3 forward = _cameraTransform.forward;
		Vector3 right = _cameraTransform.right;
		forward.y = 0f;
		right.y = 0f;

		forward.Normalize();
		right.Normalize();

		Vector3 moveLocal = right * moveInput.x + forward * moveInput.y;
		moveLocal = Vector3.ClampMagnitude(moveLocal, 1f);


		return moveLocal;
	}

	public Vector3 CalculateLookDirection()
	{
		Vector2 lookInput = _inputActions.LookAction.ReadValue<Vector2>();


		//horizontal rotation
		_lookRotY += lookInput.x * Time.deltaTime * _rotationSettings.LookRotSpeedHor;

		//vertical rotation
		_lookRotX += lookInput.y * Time.deltaTime * _rotationSettings.LookRotSpeedVert;
		_lookRotX = Mathf.Clamp(_lookRotX, _rotationSettings.MinLookAngleX, _rotationSettings.MaxLookAngleX);
		//_lookRotPivot.localRotation = Quaternion.Euler(_lookRotX, 0f, 0f);


		return Quaternion.Euler(_lookRotX, _lookRotY, 0f) * Vector3.forward;
	}


	protected virtual void OnCrouchStarted()
	{
		CrouchStarted?.Invoke(this, EventArgs.Empty);
	}
	protected virtual void OnCrouchEnded()
	{
		CrouchEnded?.Invoke(this, EventArgs.Empty);
	}
	protected virtual void OnJumpRequested()
	{
		JumpRequested?.Invoke(this, EventArgs.Empty);
	}
}
