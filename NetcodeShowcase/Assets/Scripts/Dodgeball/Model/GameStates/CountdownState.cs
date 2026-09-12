namespace Dodgeball.Model.GameStates
{
	public class CountdownState : BaseGameState
	{
		const float _countdownTime = 4f;
		const float _speedModifier = 2f; //make countdown feel snappier
		public CountdownState(GameStatesFSM fsm) : base(fsm)
		{
		}

		public override void OnEnter()
		{
			GameModel.CountDownTime = _countdownTime;
			base.OnEnter();
		}

		public override void Update(float deltaTime)
		{
			GameModel.CountDownTime -= deltaTime * _speedModifier;
			if (GameModel.CountDownTime <= 0f)
			{
				FSM.TransitionTo(FSM.MatchPlayingState);
			}
			base.Update(deltaTime);
		}
	}
}
