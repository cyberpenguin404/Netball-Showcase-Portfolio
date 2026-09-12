using System;

public interface IBallThrowingStrategy
{
	event EventHandler GrabBallRequested;
	event EventHandler ThrowBallRequested;

	void Update();
}
