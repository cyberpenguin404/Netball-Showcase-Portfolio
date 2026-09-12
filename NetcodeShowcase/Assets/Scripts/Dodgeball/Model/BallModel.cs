
using MVP.Model;

namespace Dodgeball.Model
{

	/// <summary>
	/// Model for a ball.
	/// Keeps track of the Last grabbed player color to award a point on direct hit.
	/// When ball hits any object, IsPlayerBall must be set back to false so no points are awarded.
	/// </summary>
	public class BallModel : ModelBase
	{
		public ulong Id { get; set; }

		private bool _isGrabbed;
		public bool IsGrabbed
		{
			get => _isGrabbed;
			set
			{
				if (_isGrabbed == value)
					return;
				_isGrabbed = value;
				OnPropertyChanged();
			}
		}

		//IsPlayerBall means that this ball is "active" and can hit a player of the opposing color.
		//The value needs to be set to false as soon as the ball has collision.
		private bool _isPlayerBall;

		public bool IsPlayerBall
		{
			get => _isPlayerBall;
			set
			{
				if (_isPlayerBall == value)
					return;
				_isPlayerBall = value;
				OnPropertyChanged();
			}
		}

		private PlayerColor _lastGrabbedPlayer;

		public PlayerColor LastGrabbedPlayerColor
		{
			get => _lastGrabbedPlayer;
			set
			{
				//if(_lastGrabbedPlayer.Equals(value))
				if (_lastGrabbedPlayer == value)
					return;
				_lastGrabbedPlayer = value;
				OnPropertyChanged();
			}
		}


	}
}
