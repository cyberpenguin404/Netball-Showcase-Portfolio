using FSM;

namespace Dodgeball.Model.GameStates
{
	public abstract class BaseGameState : IState
	{
		public GameModel GameModel => FSM.Context;
		public GameStatesFSM FSM { get; }
		protected BaseGameState(GameStatesFSM fsm)
		{
			FSM = fsm;
		}

		//GameState Controls

		public virtual void RequestJoinTeam(PlayerColor color) { }
		public virtual void OtherPlayerJoined() { }
		public virtual void OtherPlayerLeft() { }
		public virtual void GameOver() { }
		public virtual void RequestNewGame() { }
		


		//IState methods
		public virtual void OnEnter()
		{
			
		}

		public virtual void Update(float deltaTime)
		{
			
		}

		public virtual void OnExit()
		{
			
		}

		
	}
}
