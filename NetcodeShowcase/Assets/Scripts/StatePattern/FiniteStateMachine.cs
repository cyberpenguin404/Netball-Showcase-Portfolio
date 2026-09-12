using System;

namespace FSM
{
	public abstract class FiniteStateMachine
	{
		public event EventHandler StateChanged;
		private IState _currentState;

		public IState CurrentState
		{
			get { return _currentState; }
			set
			{
				if (_currentState == value) return;
				_currentState = value;
			}
		}

		public virtual void Update(float deltaTime)
		{
			CurrentState.Update(deltaTime);
		}

		public virtual void TransitionTo(IState newState)
		{
			if (newState == null) return;
			if (newState == CurrentState) return;

			if (CurrentState != null) 
				CurrentState.OnExit();

			CurrentState = newState;
			CurrentState.OnEnter();

			OnStateChanged();
		}

		protected virtual void OnStateChanged()
		{
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}