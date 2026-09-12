using MVP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Dodgeball.Model
{
	public enum PlayerColor
	{
		Blue,
		Red
	}
	/// <summary>
	/// Model for the player.
	/// Mainly handles balls.
	/// Should also handle player specific upgrades
	/// </summary>
	public class PlayerModel : ModelBase
	{
		public event EventHandler<ThrowBallEventArgs> BallThrown;
		public event EventHandler<PlayerBallTouchEventArgs> TouchedByBall;
		public event EventHandler<PlayerHitEventArgs> HitByPlayerBall;
		public PlayerColor Color
		{
			get => _color;
			set
			{
				if (_color == value)
					return;
				_color = value;
				OnPropertyChanged();
			}
		}

		public BallModel GrabbedBall
		{
			get => _grabbedBall;
			set
			{
				if (_grabbedBall == value)
					return;
				_grabbedBall = value;
				OnPropertyChanged();
			}
		}

		public List<BallModel> BallsInGrabRange = new List<BallModel>();
		private BallModel _grabbedBall;
		private PlayerColor _color;

		public ulong PlayerId { get; }
		private string _playFabId;
        public string PlayFabId
		{
			get => _playFabId;
			set
			{
				if (_playFabId == value)
					return;
                _playFabId = value;
				OnPropertyChanged();
			}
		}
        public string UserName { get; set; }
		public PlayerModel(ulong playerId)
		{
			PlayerId = playerId;
		}

		public void SetBallInGrabRange(BallModel ball, bool inRange)
		{
			if (inRange && !BallsInGrabRange.Contains(ball))
			{
				BallsInGrabRange.Add(ball);
			}
			else if (!inRange && BallsInGrabRange.Contains(ball))
			{
				BallsInGrabRange.Remove(ball);
            }
		}

		public BallModel TryGrabBall()
		{
			if (_grabbedBall != null) return null; // only allow to grab 1 ball
			if (!BallsInGrabRange.Any(b => !b.IsPlayerBall)) return null;


			GrabbedBall = BallsInGrabRange.First(b => !b.IsPlayerBall);
			BallsInGrabRange.Remove(GrabbedBall);

			GrabbedBall.IsGrabbed = true;
			GrabbedBall.LastGrabbedPlayerColor = Color;
			GrabbedBall.IsPlayerBall = true;

			return GrabbedBall;
		}
        public void NetworkSetGrabbedBall(BallModel ball)
        {
            if (ball == null)
            {
                ReleaseBall();
                return;
            }

            GrabbedBall = ball;
            GrabbedBall.IsGrabbed = true;
            GrabbedBall.LastGrabbedPlayerColor = Color;
            GrabbedBall.IsPlayerBall = true;
        }
        public bool TryThrowBall(Vector3 velocity)
		{
			if (GrabbedBall != null)
			{
				BallModel ball = GrabbedBall;
				ReleaseBall();
				OnThrowBall(ball, velocity);
				return true;
			}
			return false;
		}
		public void ReleaseBall()
		{
			if (_grabbedBall == null) return;
			_grabbedBall.IsGrabbed = false;
			GrabbedBall = null;
		}

		public virtual void OnThrowBall(BallModel ball, Vector3 velocity)
		{
			BallThrown(this, new ThrowBallEventArgs(ball, velocity));
		}
		public virtual void OnTouchedByBall(BallModel ball)
		{
			TouchedByBall?.Invoke(this, new PlayerBallTouchEventArgs(this, ball));
		}
		public virtual void OnHitByBall(BallModel ball, PlayerColor sourcePlayerColor)
		{
			HitByPlayerBall?.Invoke(this, new PlayerHitEventArgs(this, ball, sourcePlayerColor));
		}
	}
}
