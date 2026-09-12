using MVP.Model;
using System;

namespace Dodgeball.Model
{
	/// <summary>
	/// MatchModel keeps track of the scores and the time.
	/// MatchModel creates the Player Models
	/// </summary>
	public class MatchModel : ModelBase
	{
		public event EventHandler MatchEnded; //Gets invoked when time runs out
        public event EventHandler OnGameOver; //Gets invoked when game over get's called from the server

        public int SecondsLeft
		{
			get => _timeLeft.Seconds;
		}
        public float RawTimeLeft => (float)_timeLeft.TotalSeconds;
        public int MinutesLeft
		{
			get => _timeLeft.Minutes;
		}

		public int ScoreRed
		{
			get => _scoreRed;
			set
			{

				if (_scoreRed == value)
					return;
				_scoreRed = value;
				OnPropertyChanged();
			}
		}

		public int ScoreBlue
		{
			get => _scoreBlue;
			set
			{
				if (_scoreBlue == value)
					return;
				_scoreBlue = value;
				OnPropertyChanged();
			}
		}
        public int MatchId
        {
            get => _matchID;
            set
            {
                if (_matchID == value)
                    return;
                _matchID = value;
                OnPropertyChanged();
            }
        }
		public bool MatchPlaying { get; private set; } = true;

		public PlayerModel PlayerRed { get; }
		public PlayerModel PlayerBlue { get; }


		private int _scoreRed;
		private int _scoreBlue;
		private int _matchID;

		public const int MATCH_DURATION = 60; //seconds
		private TimeSpan _timeLeft = TimeSpan.Zero;

		public MatchModel(ulong redPlayerId, ulong bluePlayerId)
		{
			PlayerBlue = new PlayerModel(bluePlayerId) { Color = PlayerColor.Blue };
			PlayerRed = new PlayerModel(redPlayerId) { Color = PlayerColor.Red };
		}
		public void StartMatch(ArenaModel arena)
		{
			arena.AddPlayer(PlayerBlue);
			arena.AddPlayer(PlayerRed);

			PlayerBlue.TouchedByBall += _player_HitByBall;
			PlayerRed.TouchedByBall += _player_HitByBall;

			_scoreBlue = 0;
			_scoreRed = 0;

			_timeLeft = TimeSpan.FromSeconds(MATCH_DURATION);
		}

		private void _player_HitByBall(object sender, PlayerBallTouchEventArgs e)
		{
			if (e.Player.Color == e.Ball.LastGrabbedPlayerColor) return;
			if (!e.Ball.IsPlayerBall) return;

			e.Player.OnHitByBall(e.Ball, e.Ball.LastGrabbedPlayerColor);

			if (e.Ball.LastGrabbedPlayerColor == PlayerColor.Blue)
			{
				ScoreBlue++;
			}
			else
			{
				ScoreRed++;
			}

		}

		public void UpdateTime(float deltaTime)
		{
			if (!MatchPlaying) { return; }

			int prevSeconds = SecondsLeft;
			int prevMinutes = MinutesLeft;
			_timeLeft -= TimeSpan.FromSeconds(deltaTime);

			if (SecondsLeft != prevSeconds)
			{
				OnPropertyChanged(nameof(SecondsLeft));
			}
			if (MinutesLeft != prevMinutes)
			{
				OnPropertyChanged(nameof(MinutesLeft));
			}

			if (_timeLeft <= TimeSpan.Zero)
			{
				OnMatchEnded();
			}
		}
        public void UpdateClientTimeNetworked(float serverTimeSeconds)
        {
            int prevSeconds = SecondsLeft;
            int prevMinutes = MinutesLeft;

            _timeLeft = TimeSpan.FromSeconds(serverTimeSeconds);

            if (SecondsLeft != prevSeconds) OnPropertyChanged(nameof(SecondsLeft));
            if (MinutesLeft != prevMinutes) OnPropertyChanged(nameof(MinutesLeft));
        }
        protected virtual void OnMatchEnded()
		{
			MatchEnded?.Invoke(this, EventArgs.Empty);
		}
		public void GameOver()
		{
			MatchPlaying = false;
			OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }
}