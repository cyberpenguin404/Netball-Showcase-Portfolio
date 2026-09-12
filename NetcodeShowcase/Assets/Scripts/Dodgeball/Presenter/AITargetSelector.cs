using System.Linq;
using UnityEngine;

namespace Dodgeball.Presenter
{
	/// <summary>
	/// This class contains the logic of selecting and storing a target for the AI player.
	/// </summary>
	public class AITargetSelector : ITargetProvider
	{
		private const float _pickTargetDistance = 10f;
		private const float _loseTargetDistance = 15f;

		private Transform _targetPlayer;
		private BallPresenter _targetBall;
		private readonly PlayerPresenter _presenter;

		public Transform TargetPlayer
		{
			get
			{
				//Room for optimization: prevent this from happening multiple times each frame
				LosePlayerTarget();
				PickPlayerTarget();
				return _targetPlayer;

			}
			private set => _targetPlayer = value;
		}

		public Transform TargetBall
		{
			get
			{
				LoseTargetBall();
				PickTargetBall();
				return _targetBall?.transform;
			}
		}

		public AITargetSelector(PlayerPresenter presenter)
		{
			_presenter = presenter;
		}

		void LoseTargetBall()
		{
			if (_targetBall == null) return;
			if (_targetBall.Model.IsGrabbed || _targetBall.Model.IsPlayerBall) //don't target a grabbed ball,  don't target a ball thrown by a player
			{
				_targetBall = null;
			}

		}
		void LosePlayerTarget()
		{
			if (_targetPlayer == null) return;

			if ((_presenter.transform.position - _targetPlayer.position).sqrMagnitude > _loseTargetDistance * _loseTargetDistance)
				_targetPlayer = null;

		}

		void PickTargetBall()
		{
			if (_targetBall != null) return;
			_targetBall = GetNearestFreeBall();
		}

		void PickPlayerTarget()
		{
			if (_targetPlayer != null) return; //only pick a target if we don't have one still

			TargetPlayer = GetNearestOpposingPlayer();

		}
		BallPresenter GetNearestFreeBall()
		{
			BallPresenter nearest = null;

			var freeBalls = _presenter.ArenaPresenter.SpawnedBalls.Where(b => b.Model.IsPlayerBall == false);
			float closestDistSq = Mathf.Infinity;

			foreach (var ball in freeBalls)
			{
				float distSq = (ball.transform.position - _presenter.transform.position).sqrMagnitude;
				if (distSq <= closestDistSq)
				{
					nearest = ball;
					closestDistSq = distSq;
				}
			}

			return nearest;
		}
		Transform GetNearestOpposingPlayer()
		{
			Transform nearest = null;
			var opposingPlayers = _presenter.ArenaPresenter.SpawnedPlayers.Where(p => p.Color != _presenter.Color);
			float closestDistSq = _pickTargetDistance * _pickTargetDistance; //start from max distance

			foreach (var player in opposingPlayers)
			{
				float distSq = (_presenter.transform.position - player.transform.position).sqrMagnitude;
				if (distSq <= closestDistSq)
				{
					nearest = player.transform;
					closestDistSq = distSq;
				}
			}
			return nearest;
		}





	}
}
