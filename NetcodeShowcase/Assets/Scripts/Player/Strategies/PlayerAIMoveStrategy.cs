using Dodgeball.Presenter;
using System;
using UnityEngine;

public class PlayerAIMoveStrategy : IPlayerMoveStrategy
{
	private const float _targetingDistance = 10f;

	private readonly ArenaPresenter _arena;
	private readonly PlayerPresenter _owningPlayer;
	private readonly ITargetProvider _targetProvider;
	private readonly LayerMask _jumpObstacleLayers;

	public event EventHandler CrouchStarted;
	public event EventHandler CrouchEnded;
	public event EventHandler JumpRequested;

	private Transform _target = null;

	public Vector3 CalculateLookDirection()
	{
		UpdateTarget();

		if (_target == null)
		{
			return _owningPlayer.transform.forward;
		}
		//look at targeting player
		return (_target.position - _owningPlayer.transform.position).normalized;
	}

	public Vector3 CalculateMovement()
	{
		UpdateTarget();
		DetectObstacleToJump();
		if (_target == null)
		{
			return Vector3.zero;
		}


		//move towards target
		Vector3 toTarget = (_target.position - _owningPlayer.transform.position).normalized;
		return toTarget;


	}


	void DetectObstacleToJump()
	{
		Vector3 forward = _owningPlayer.transform.forward;
		if (Physics.Raycast(_owningPlayer.transform.position + Vector3.up * 0.5f + forward * 0.5f, forward, 1f, _jumpObstacleLayers))
		{
			OnJumpRequested();
		}
	}

	//This can be optimized, gets called twice per frame
	private void UpdateTarget()
	{

		//first find a free ball
		if (_owningPlayer.Model.GrabbedBall == null)
		{
			FindBallTarget();
		}
		//once a ball is grabbed, find a player to target
		else
		{
			FindPlayerTarget();
		}
	}

	private void FindBallTarget()
	{
		_target = _targetProvider.TargetBall;
	}
	private void FindPlayerTarget()
	{
		_target = _targetProvider.TargetPlayer;
	}


	public PlayerAIMoveStrategy(ArenaPresenter arena, PlayerPresenter owningPlayer, ITargetProvider targetProvider, LayerMask jumpObstacleLayers)
	{
		_arena = arena;
		_owningPlayer = owningPlayer;
		_targetProvider = targetProvider;
		_jumpObstacleLayers = jumpObstacleLayers;
	}


	protected virtual void OnJumpRequested()
	{
		JumpRequested?.Invoke(this, EventArgs.Empty);
	}
}
