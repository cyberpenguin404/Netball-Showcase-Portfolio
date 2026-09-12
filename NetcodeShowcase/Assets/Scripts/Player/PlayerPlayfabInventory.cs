using Dodgeball.Model;
using Dodgeball.Presenter;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerPlayfabInventory : NetworkBehaviour
    {
        private PlayerPresenter _playerPresenter;
        private MatchModel _matchModel;

        private const string _goldCurrencyCode = "GD";
        private const string _extraBallItemId = "extra_ball";

        private const int BALL_PRICE_GOLD = 5;

        private int _currentGoldBalance;

        private bool _isProcessingPurchase;

        [SerializeField]
        private TextMeshProUGUI _goldBalanceText;

        private ArenaPresenter _arena;
        private void Awake()
        {
            _playerPresenter = GetComponent<PlayerPresenter>();
            _arena = FindAnyObjectByType<ArenaPresenter>();
        }
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            if (!IsOwner)
            {
                _goldBalanceText.transform.parent.gameObject.SetActive(false);
                return;
            }

            RefreshGoldBalance();

            MatchPresenter matchPresenter = FindAnyObjectByType<MatchPresenter>();
            if (matchPresenter != null && matchPresenter.Model != null)
            {
                _matchModel = matchPresenter.Model;
                _matchModel.PropertyChanged += _matchModel_PropertyChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && _matchModel != null)
            {
                _matchModel.PropertyChanged -= _matchModel_PropertyChanged;
            }
            base.OnNetworkDespawn();
        }

        private void _matchModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatchModel.ScoreRed) && _playerPresenter.Color == PlayerColor.Red)
            {
                AwardGoldToken();
            }
            else if (e.PropertyName == nameof(MatchModel.ScoreBlue) && _playerPresenter.Color == PlayerColor.Blue)
            {
                AwardGoldToken();
            }
        }
        private void AwardGoldToken()
        {
            AddUserVirtualCurrencyRequest request = new()
            {
                VirtualCurrency = _goldCurrencyCode,
                Amount = 1
            };

            PlayFabClientAPI.AddUserVirtualCurrency(request,
                result => {
                    _currentGoldBalance = result.Balance;
                    UpdateGoldDisplay();
                },
                error => Debug.LogError($"Failed to award gold: {error.GenerateErrorReport()}")
            );
        }
        private void RefreshGoldBalance()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                if (result.VirtualCurrency.TryGetValue("GD", out int balance))
                {
                    _currentGoldBalance = balance;
                    UpdateGoldDisplay();
                }

                //_itemsList.itemsSource = result.Inventory;
                //_itemsList.Rebuild();
            },
            error => Debug.LogError(error.GenerateErrorReport())
        );
        }

        private void UpdateGoldDisplay()
        {
            _goldBalanceText.text = "Current Gold: " + _currentGoldBalance.ToString();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer || _isProcessingPurchase || !other.CompareTag("BallSpawner")) return;


            if (other.TryGetComponent<BallSpawner>(out BallSpawner ballSpawner))
            {
                InitiatePurchaseClientRpc(ballSpawner.SpawnLocationIndex);
            }
        }
        [ClientRpc]
        private void InitiatePurchaseClientRpc(int spawnLocationIndex)
        {
            if (!IsOwner || _isProcessingPurchase) return;

            if (_currentGoldBalance < BALL_PRICE_GOLD)
            {
                Debug.LogWarning("[Shop] Not enough gold locally. PlayFab will block this anyway if cheated.");
                return;
            }

            PurchaseAndConsumeExtraBall(spawnLocationIndex);
        }

        private void PurchaseAndConsumeExtraBall(int spawnLocationIndex)
        {
            _isProcessingPurchase = true;

            PurchaseItemRequest purchaseRequest = new PurchaseItemRequest
            {
                ItemId = _extraBallItemId,
                VirtualCurrency = _goldCurrencyCode,
                Price = 5
            };

            PlayFabClientAPI.PurchaseItem(purchaseRequest,
            purchaseResult => {
                // Find the specific item instance we just bought in order to consume it
                if (purchaseResult.Items != null && purchaseResult.Items.Count > 0)
                {
                    ItemInstance itemToConsume = purchaseResult.Items[0];
                    ConsumeExtraBall(itemToConsume.ItemInstanceId, spawnLocationIndex);
                }
            },
            error => {
                _isProcessingPurchase = false;
                Debug.LogError($"Purchase failed: {error.GenerateErrorReport()}");
            }
        );
        }
        private void ConsumeExtraBall(string itemInstanceId, int spawnLocationIndex)
        {
            var consumeRequest = new ConsumeItemRequest
            {
                ItemInstanceId = itemInstanceId,
                ConsumeCount = 1
            };

            PlayFabClientAPI.ConsumeItem(consumeRequest,
                consumeResult => {
                    Debug.Log("Item consumed successfully! Spawning extra ball...");
                    _isProcessingPurchase = false;

                    RefreshGoldBalance();

                    _arena.RequestBallSpawnFromShop(spawnLocationIndex);
                },
                error => {
                    _isProcessingPurchase = false;
                    Debug.LogError($"Consumption failed: {error.GenerateErrorReport()}");
                }
            );
        }
    }
}
