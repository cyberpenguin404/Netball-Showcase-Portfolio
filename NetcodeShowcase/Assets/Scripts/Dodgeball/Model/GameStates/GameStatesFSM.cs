using FSM;

namespace Dodgeball.Model.GameStates
{

	public class GameStatesFSM : FiniteStateMachine
	{

		public SelectTeamState SelectTeamState { get; private set; }
		public WaitingForOtherPlayerState WaitingForOtherPlayerState { get; private set; }
		public CountdownState CountdownState { get; private set; }
		public MatchPlayingState MatchPlayingState { get; private set; }
		public GameOverState GameOverState { get; private set; }


		public GameModel Context { get; }

		public GameStatesFSM(GameModel context)
		{
			WaitingForOtherPlayerState = new WaitingForOtherPlayerState(this);
			SelectTeamState = new SelectTeamState(this);
			CountdownState = new CountdownState(this);
			MatchPlayingState = new MatchPlayingState(this);
			GameOverState = new GameOverState(this);

			Context = context;

			TransitionTo(WaitingForOtherPlayerState);
		}

		public new BaseGameState CurrentState => base.CurrentState as BaseGameState;
	}
}
