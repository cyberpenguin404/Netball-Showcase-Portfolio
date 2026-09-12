using Assets.Scripts.Backend;
using Dodgeball.Model;
using Dodgeball.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Network
{
    public class ArenaSync : NetworkBehaviour
    {
        public ArenaPresenter Presenter { get; set; }
        public ArenaModel Model { get; set; }

        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private GameObject _ballPrefab;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public void SpawnPlayerOverNetwork(ulong playerId)
        {
            if (!IsServer) return;

            PlayerModel model = Presenter.Model.GetPlayer(playerId);
            var spawnLocation = Presenter.GetRandomPlayerSpawn(model.Color);

            GameObject playerGo = Instantiate(_playerPrefab, spawnLocation.pos, spawnLocation.rot);

            var playerPresenter = playerGo.GetComponent<PlayerPresenter>();
            playerPresenter.PlayerId = playerId;

            var networkObject = playerGo.GetComponent<NetworkObject>();

            networkObject.SpawnWithOwnership(playerId);
        }
        public void SpawnBallOverNetwork(Transform spawnLocation)
        {
            if (!IsServer) return;

            GameObject ballGO = Instantiate(_ballPrefab, spawnLocation.position, Quaternion.identity);

            NetworkObject networkObject = ballGO.GetComponent<NetworkObject>();

            networkObject.Spawn(true);
        }
        [Rpc(SendTo.Server)]
        public void RequestSpawnBallServerRpc(int spawnLocationIndex)
        {
            Presenter.SpawnBall(spawnLocationIndex);
        }
    }
}
