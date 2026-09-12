namespace Dodgeball.Model.GameStates
{
	public class MatchPlayingState : BaseGameState
	{
		private MatchModel _currentMatch = null;
		public MatchPlayingState(GameStatesFSM fsm) : base(fsm)
		{
		}

		public override void OnEnter()
		{
			_currentMatch = GameModel.StartNewMatch();
			base.OnEnter();
		}

		public override void Update(float deltaTime)
		{
			_currentMatch.UpdateTime(deltaTime);
			if (!_currentMatch.MatchPlaying)
			{
				FSM.TransitionTo(FSM.GameOverState);
			}
			base.Update(deltaTime);
		}
	}
}
