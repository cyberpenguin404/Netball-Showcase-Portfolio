using Assets.Scripts;
using Assets.Scripts.Dodgeball.Network;
using Dodgeball.Model;
using MVP.Presenter;
using PD4.MVPBase.Presenter;
using UnityEngine;

namespace Dodgeball.Presenter
{
	public class PlayerThrow : PresenterMonobehaviour<PlayerModel>
	{
		[SerializeField]
		private Transform _aimTransform;

		[SerializeField]
		private float _throwSpeed = 10f;

		[SerializeField]
		private FollowTransform _ballAnchor;

		private PlayerSync _networkSync;
		private PlayerPresenter _playerPresenter;
		private IBallThrowingStrategy _throwingStrategy;

		public IBallThrowingStrategy ThrowingStrategy
		{
			get => _throwingStrategy;
			set
			{
				if (_throwingStrategy == value) return;

				if (_throwingStrategy != null)
				{
					_throwingStrategy.ThrowBallRequested -= _throwingStrategy_ThrowBallRequested;
					_throwingStrategy.GrabBallRequested -= _throwingStrategy_GrabBallRequested;
				}

				_throwingStrategy = value;
				if (_throwingStrategy == null) return;

				_throwingStrategy.ThrowBallRequested += _throwingStrategy_ThrowBallRequested;
				_throwingStrategy.GrabBallRequested += _throwingStrategy_GrabBallRequested;
			}
		}

		private void OnDestroy()
		{
			ThrowingStrategy = null;
		}

		private void _throwingStrategy_GrabBallRequested(object sender, System.EventArgs e)
		{
            if (_networkSync == null || !_networkSync.IsSpawned || !_networkSync.IsOwner) return;
            _networkSync.TryGrabBallRpc();
			Debug.Log("Tried grabbing ball");
		}

		private void _throwingStrategy_ThrowBallRequested(object sender, System.EventArgs e)
		{
            if (_networkSync == null || !_networkSync.IsSpawned || !_networkSync.IsOwner) return;
            _networkSync.TryThrowBallRpc();
		}


		public bool HasBall => Model?.GrabbedBall != null;

		protected override void Awake()
		{
			base.Awake();
			_playerPresenter = GetComponent<PlayerPresenter>();
			_networkSync = GetComponent<PlayerSync>();
        }
		protected override void Start()
		{
			base.Start();
			Model = _playerPresenter.Model;
		}

		protected override void Update()
		{
			ThrowingStrategy?.Update();
			base.Update();
		}


		//Model changes

		protected override void OnModelUpdated(PlayerModel previousModel)
		{
			Model.BallThrown += Model_BallThrown;
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.GrabbedBall):
					SetGrabbedBall();
					break;
			}
		}

		private void SetGrabbedBall()
		{
			if (Model.GrabbedBall == null) return;
			BallPresenter presenter = _playerPresenter.ArenaPresenter.GetBallPresenter(Model.GrabbedBall);
			_ballAnchor.AddChild(presenter.transform);
			presenter.transform.localPosition = Vector3.zero;

		}

		//Gets invoked when the ball needs to be thrown
		private void Model_BallThrown(object sender, ThrowBallEventArgs e)
		{
			var ballPresenter = _playerPresenter.ArenaPresenter.GetBallPresenter(e.Ball);
            _ballAnchor.RemoveChild(ballPresenter.transform);
			ballPresenter.Throw(e.Velocity.ToUnityVector());
		}
		public Vector3 CalculateAimVelocity()
		{
			return _aimTransform.forward * _throwSpeed;
		}
		public Vector3 GetGrabPosition()
		{
			return _ballAnchor.transform.position;
		}
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Ball")) return;

            BallPresenter ball = other.GetComponent<BallPresenter>();
            Model.SetBallInGrabRange(ball.Model, true);

        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Ball")) return;

            BallPresenter ball = other.GetComponent<BallPresenter>();
            Model.SetBallInGrabRange(ball.Model, false);
        }
    }
}
