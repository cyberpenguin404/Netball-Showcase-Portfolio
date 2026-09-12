using System;
using System.Numerics;

namespace Dodgeball.Model
{
	public class PlayerIdEventArgs : EventArgs
	{
		public PlayerIdEventArgs(ulong playerId)
		{
			PlayerId = playerId;
		}

		public ulong PlayerId { get; }
	}
	public class BallSpawnedEventArgs : EventArgs
	{
		public BallSpawnedEventArgs(BallModel ball)
		{
			Ball = ball;
		}

		public BallModel Ball { get; }
		public int SpawnLocationId { get; }
	}
	public class ThrowBallEventArgs : EventArgs
	{
		public ThrowBallEventArgs(BallModel ball, Vector3 velocity)
		{
			Ball = ball;
			Velocity = velocity;
		}

		public BallModel Ball { get; }
		public Vector3 Velocity { get; }
	}
	public class BallEventArgs : EventArgs
	{
		public BallEventArgs(BallModel ball)
		{
			Ball = ball;
		}

		public BallModel Ball { get; }
	}
	public class PlayerHitEventArgs : EventArgs
	{
		public PlayerModel TargetPlayer { get; }
		public BallModel Ball { get; }
		public PlayerColor SourcePlayerColor { get; }
		public PlayerHitEventArgs(PlayerModel targetPlayer, BallModel ball, PlayerColor sourcePlayerColor)
		{
			TargetPlayer = targetPlayer;
			Ball = ball;
			SourcePlayerColor = sourcePlayerColor;
		}
	}
	public class PlayerBallTouchEventArgs : EventArgs
	{
		public PlayerBallTouchEventArgs(PlayerModel player, BallModel ball)
		{
			Player = player;
			Ball = ball;
		}

		public PlayerModel Player { get; }
		public BallModel Ball { get; }
	}
	public class EventArgs<T> : EventArgs
	{
		public EventArgs(T value)
		{
			Value = value;
		}

		public T Value { get; }
	}
	public class EventArgs<T1, T2> : EventArgs
	{
		public EventArgs(T1 value1, T2 value2)
		{
			Value1 = value1;
			Value2 = value2;
		}

		public T1 Value1 { get; }
		public T2 Value2 { get; }
	}
}
