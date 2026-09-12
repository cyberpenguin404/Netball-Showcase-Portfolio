using Dodgeball.Presenter;
using System;
using UnityEngine;

public class AIBallThrowingStrategy : IBallThrowingStrategy
{
	const float _grabThrowCooldown = 2f;

	private readonly ITargetProvider _targetProvider;
	private readonly PlayerPresenter _player;

	public event EventHandler GrabBallRequested;
	public event EventHandler ThrowBallRequested;

	private float _cooldown = _grabThrowCooldown;

	public AIBallThrowingStrategy(ITargetProvider targetProvider, PlayerPresenter player)
	{
		_targetProvider = targetProvider;
		_player = player;
	}

	public void Update()
	{
		if (_player.Model == null) return;

		if (_cooldown > 0f)
		{
			_cooldown -= Time.deltaTime;
			return;
		}

		if (_player.Model.GrabbedBall == null)
			TryGrabBall();
		else
			ThrowBall();


	}

	void TryGrabBall()
	{
		if (_player.Model.GrabbedBall != null) return;

		OnRequestGrabBall();
		if (_player.Model.GrabbedBall != null)
		{
			_cooldown = _grabThrowCooldown;
		}
	}

	void ThrowBall()
	{
		if (_player.Model.GrabbedBall == null) return;
		if (_targetProvider.TargetPlayer == null)
			return;
		Transform target = _targetProvider.TargetPlayer;
		if (target == null) return;

		//TODO: decide when to throw
		OnRequestThrowBall();
		_cooldown = _grabThrowCooldown;
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
