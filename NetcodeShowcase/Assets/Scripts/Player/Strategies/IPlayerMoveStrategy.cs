using System;
using UnityEngine;

public interface IPlayerMoveStrategy
{
	event EventHandler CrouchStarted;
	event EventHandler CrouchEnded;
	event EventHandler JumpRequested;

	Vector3 CalculateMovement();
	Vector3 CalculateLookDirection();
}
