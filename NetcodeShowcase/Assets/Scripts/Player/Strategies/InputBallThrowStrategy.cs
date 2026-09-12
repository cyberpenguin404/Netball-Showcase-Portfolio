using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface ITargetProvider
{
	public Transform TargetPlayer { get; }
	public Transform TargetBall { get; }
}
public class InputBallThrowStrategy : IBallThrowingStrategy
{
	public event EventHandler GrabBallRequested;
	public event EventHandler ThrowBallRequested;

	private readonly InputAction _grabAction;
	private readonly InputAction _throwAction;


	public InputBallThrowStrategy(PlayerInputActions inputActions)
	{

		_grabAction = inputActions.GrabBallAction;
		_throwAction = inputActions.ThrowBallAction;

		_grabAction.Enable();
		_throwAction.Enable();

		_grabAction.performed += _grabAction_performed;
		_throwAction.performed += _throwAction_performed;
	}
	public void Update()
	{

	}

	private void _throwAction_performed(InputAction.CallbackContext obj)
	{
		OnRequestThrowBall();
	}

	private void _grabAction_performed(InputAction.CallbackContext obj)
	{
		OnRequestGrabBall();
	}

	protected virtual void OnRequestGrabBall()
	{
		GrabBallRequested?.Invoke(this, EventArgs.Empty);
	}
	protected virtual void OnRequestThrowBall()
	{
		ThrowBallRequested?.Invoke(this, EventArgs.Empty);
	}


}
