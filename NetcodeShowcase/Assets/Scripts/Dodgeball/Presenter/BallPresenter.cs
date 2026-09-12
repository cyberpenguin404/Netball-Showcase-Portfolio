using Dodgeball.Model;
using MVP.Presenter;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Dodgeball.Presenter
{
    public class BallPresenter : PresenterMonobehaviour<BallModel>
    {
        int _ignorePlayerLayer = 0, _defaultLayer;
        [SerializeField]
        private Rigidbody _rigidBody;

        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Material _blueInfluenceMat, _redInfluenceMat;

        private Material _defaultMat;

        private BallSync _networkSync;

        protected override void Awake()
        {
            _defaultMat = _renderer.sharedMaterial;
            _defaultLayer = gameObject.layer;
            _ignorePlayerLayer = LayerMask.NameToLayer("IgnorePlayer");
            Model = new BallModel();

            _networkSync = GetComponent<BallSync>();
            _networkSync.Model = Model;
            _networkSync.Presenter = this;

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            //Add self to arenapresenter
            FindAnyObjectByType<ArenaPresenter>().AddBall(this);
        }

        protected override void OnModelPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Model.IsGrabbed))
            {
                _rigidBody.isKinematic = Model.IsGrabbed;
                if (!Model.IsGrabbed) //delayed set layer to avoid collison with throwing player
                {
                    StartCoroutine(SwitchLayersDelayed(_defaultLayer, .1f));
                }
                else
                {
                    gameObject.layer = _ignorePlayerLayer;
                }
            }
            if (propertyName == nameof(Model.IsPlayerBall))
            {
                UpdatePlayerInfluenceMaterial();
            }

            base.OnModelPropertyChanged(propertyName);

        }

        private void UpdatePlayerInfluenceMaterial()
        {
            if (!Model.IsPlayerBall)
            {
                _renderer.sharedMaterial = _defaultMat;
            }
            else if (Model.LastGrabbedPlayerColor == PlayerColor.Red)
            {
                _renderer.sharedMaterial = _redInfluenceMat;
            }
            else
            {
                _renderer.sharedMaterial = _blueInfluenceMat;
            }
        }

        public void Throw(Vector3 velocity)
        {
            if (Model.IsGrabbed)
            {
                throw new InvalidOperationException("Cannot throw a ball while it's still grabbed! Set Model IsGrabbed to false first");
            }
            _rigidBody.linearVelocity = velocity;
        }

        IEnumerator SwitchLayersDelayed(int layer, float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.layer = layer;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerPresenter hitPlayer = collision.gameObject.GetComponent<PlayerPresenter>();
                hitPlayer.Model.OnTouchedByBall(Model);
            }
            //no longer player ball
            Model.IsPlayerBall = false;

        }
    //    void OnTriggerEnter(Collider other)
    //    {
    //        if (!other.CompareTag("Player")) return;

    //        PlayerPresenter player = other.GetComponent<PlayerPresenter>();

    //        Debug.Log(
    //    $"SERVER:{NetworkManager.Singleton.IsServer} " +
    //    $"Ball:{Model.Id} " +
    //    $"Player:{player.PlayerId} " +
    //    $"Owner:{player.GetComponent<NetworkObject>().OwnerClientId}"
    //);
    //        if (player.Model != null)
    //            player.Model.SetBallInGrabRange(Model, true);
    //    }

    //    private void OnTriggerExit(Collider other)
    //    {
    //        if (!other.CompareTag("Player")) return;

    //        PlayerPresenter player = other.GetComponent<PlayerPresenter>();
    //        if (player.Model != null)
    //            player.Model.SetBallInGrabRange(Model, false);
    //    }
    }
}
