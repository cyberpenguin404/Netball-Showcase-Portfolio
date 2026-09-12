using UnityEngine;

namespace FSM
{
	public interface IState
	{
		public void OnEnter();
		public void Update(float deltaTime);
		public void OnExit();
	}
}