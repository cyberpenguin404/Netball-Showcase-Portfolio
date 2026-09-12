using Assets.Scripts.Dodgeball.Network;
using Assets.Scripts.Player;
using Assets.Scripts.Player.Strategies;
using Dodgeball.Model;
using MVP.Presenter;
using PD4.MVPBase.Presenter;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dodgeball.Presenter
{
    public class PlayerPresenter : PresenterMonobehaviour<PlayerModel>
    {
        //Inspector fields
        [SerializeField]
        private MeshRenderer[] _coloredParts;
        [SerializeField]
        private Material _redMaterial, _blueMaterial;
        [SerializeField]
        private LayerMask _jumpObstaclesMask;
        [SerializeField]
        private GameObject _hitParticlesPrefab;

        [SerializeField]
        private GameObject _aimVisual;

        [SerializeField]
        private TextMeshProUGUI _nameTag;

        //Properties
        public PlayerColor Color => Model.Color;
        public ArenaPresenter ArenaPresenter { get; set; }
        public IBallThrowingStrategy ThrowStrategy { get; set; }


        public ulong PlayerId { get; set; }

        protected override void Awake()
        {
            SetControlledByPlayer(false);
            base.Awake();
        }
        protected override void Start()
        {
            //	//Find Model from MatchModel
            //	ArenaPresenter = FindAnyObjectByType<ArenaPresenter>();

            //          PlayerId = GetComponent<NetworkObject>().OwnerClientId;

            //          Model = ArenaPresenter.Model.GetPlayer(PlayerId);
            //	ArenaPresenter.AddPlayerPresenter(this);

            //	_networkSync = GetComponent<PlayerSync>();
            //	_networkSync.Model = Model;
            //	_networkSync.Presenter = this;
            //	_networkSync.ArenaPresenter = ArenaPresenter;
            //          _networkSync.Initialize();

            //          SetControlledByPlayer(PlayerId == _networkSync.NetworkManager.LocalClientId);

            base.Start();

            GamePresenter presenter = FindAnyObjectByType<GamePresenter>();
            PlayerInputActions inputActions = presenter.InputActionConfig.InputActions;

            inputActions.ShownamesAction.Enable();


            inputActions.ShownamesAction.started += OnShowNames;
            inputActions.ShownamesAction.canceled += OnHideNames;
        }
        private void OnDestroy()
        {
            GamePresenter presenter = FindAnyObjectByType<GamePresenter>();
            PlayerInputActions inputActions = presenter.InputActionConfig.InputActions;

            inputActions.ShownamesAction.started -= OnShowNames;
            inputActions.ShownamesAction.canceled -= OnHideNames;
        }
        private void OnHideNames(InputAction.CallbackContext context)
        {
            _nameTag.gameObject.SetActive(false);
        }

        private void OnShowNames(InputAction.CallbackContext context)
        {
            _nameTag.text = Model.UserName;
            _nameTag.gameObject.SetActive(true);
        }

        protected override void OnModelUpdated(PlayerModel previousModel)
        {
            if (previousModel != null)
            {
                previousModel.HitByPlayerBall -= Model_HitByBall;
            }

            if (Model != null)
            {
                Model.HitByPlayerBall += Model_HitByBall;
                UpdatePlayerColor();
            }
        }

        private void Model_HitByBall(object sender, PlayerHitEventArgs e)
        {
            BallPresenter ball = ArenaPresenter.GetBallPresenter(e.Ball);
            if (ball == null) return;

            var particles = Instantiate(_hitParticlesPrefab).GetComponent<HitParticles>();
            particles.Spawn(e.SourcePlayerColor, ball.transform.position, this);
        }

        protected override void OnModelPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Model.Color))
            {
                UpdatePlayerColor();
            }
            if (propertyName == nameof(Model.GrabbedBall))
            {
                SetAimVisible(Model.GrabbedBall != null);
            }
            base.OnModelPropertyChanged(propertyName);
        }

        public void SetControlledByPlayer(bool controlled)
        {
            GamePresenter presenter = FindAnyObjectByType<GamePresenter>();
            PlayerInputActions inputActions = presenter.InputActionConfig.InputActions;

            if (controlled)
            {
                GetComponent<PlayerMovement>().MoveStrategy = new PlayerInputMoveStrategy(transform, Camera.main.transform, inputActions, presenter.InputActionConfig.RotationSettings);
                GetComponent<PlayerThrow>().ThrowingStrategy = new InputBallThrowStrategy(inputActions);
            }
            else
            {
                GetComponent<PlayerMovement>().MoveStrategy = new DummyMoveStrategy();
                GetComponent<PlayerThrow>().ThrowingStrategy = new DummyThrowStrategy();
            }

            GetComponent<PlayerCameraAttach>().enabled = controlled;
        }

        private void UpdatePlayerColor()
        {
            Material colorMat = Color == PlayerColor.Red ? _redMaterial : _blueMaterial;
            foreach (var renderer in _coloredParts)
            {
                renderer.sharedMaterial = colorMat;
            }
        }

        private void SetAimVisible(bool visible)
        {
            _aimVisual.SetActive(visible);
        }
        // Update is called once per frame
        protected override void Update()
        {
            ThrowStrategy?.Update();
            base.Update();
        }
    }
}
