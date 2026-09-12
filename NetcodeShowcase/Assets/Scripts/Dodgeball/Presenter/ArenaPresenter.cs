using Assets.Scripts.Dodgeball.Network;
using Dodgeball.Model;
using MVP.Presenter;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Dodgeball.Presenter
{
	public class ArenaPresenter : PresenterMonobehaviour<ArenaModel>
	{
		[SerializeField]
		private SpawnArea[] _spawns;
		[SerializeField]
		private List<Transform> _ballSpawnLocations = new List<Transform>();

		[SerializeField]
		private GameObject _ballPrefab;

		private List<PlayerPresenter> _spawnedPlayers = new List<PlayerPresenter>();
		private List<BallPresenter> _spawnedBalls = new List<BallPresenter>();

		public ReadOnlyCollection<PlayerPresenter> SpawnedPlayers => _spawnedPlayers.AsReadOnly();
		public ReadOnlyCollection<BallPresenter> SpawnedBalls => _spawnedBalls.AsReadOnly();

		[SerializeField]
		private BallSpawner[] _ballSpawners;

		private ArenaSync _networkSync;

		protected override void Awake()
		{
			base.Awake();
			Model = new ArenaModel();

			foreach (var spawner in _ballSpawners)
			{
				int index = RegisterSpawner(spawner.SpawnLocation);
				spawner.SpawnLocationIndex = index;
			}

			_networkSync = GetComponent<ArenaSync>();
			_networkSync.Model = Model;
			_networkSync.Presenter = this;
		}

		//Adds a spawnlocation and stores the index
		public int RegisterSpawner(Transform transform)
		{
			_ballSpawnLocations.Add(transform);
			return _ballSpawnLocations.Count - 1;
		}

		public void SpawnBall(int spawnLocationIndex)
		{
            if (!_networkSync.IsServer) return;

            if (spawnLocationIndex == -1)
			{
				spawnLocationIndex = Random.Range(0, _ballSpawnLocations.Count);
			}
			Transform spawnLocation = _ballSpawnLocations[spawnLocationIndex];

			_networkSync.SpawnBallOverNetwork(spawnLocation);
		}
        public void RequestBallSpawnFromShop(int spawnLocationIndex)
        {
            if (_networkSync.IsServer)
            {
                SpawnBall(spawnLocationIndex);
            }
            else
            {
                _networkSync.RequestSpawnBallServerRpc(spawnLocationIndex);
            }
        }
        public void SpawnPlayer(ulong playerId)
		{
			_networkSync.SpawnPlayerOverNetwork(playerId);
		}

		public void AddPlayerPresenter(PlayerPresenter presenter)
		{
			//Don't add model to ArenaModel. Model already exists in Arena Model at this point (created by game)
			_spawnedPlayers.Add(presenter);
		}

		public void AddBall(BallPresenter presenter)
		{
			Model.AddBall(presenter.Model);
			_spawnedBalls.Add(presenter);
		}

		public BallPresenter GetBallPresenter(ulong ballId)
		{
			return _spawnedBalls.FirstOrDefault(b => b.Model.Id == ballId);
		}
		public BallPresenter GetBallPresenter(BallModel model)
		{
			return _spawnedBalls.FirstOrDefault(b => b.Model == model);
		}
		public PlayerPresenter GetPlayerPresenter(ulong playerId)
		{
			return _spawnedPlayers.FirstOrDefault(b => b.PlayerId == playerId);
		}
		public PlayerPresenter GetPlayerPresenter(PlayerModel model)
		{
			return _spawnedPlayers.FirstOrDefault(p => p.Model == model);
		}

		//Player spawn randomization
		public (Vector3 pos, Quaternion rot) GetRandomPlayerSpawn(PlayerColor color)
		{
			var spawns = _spawns.Where(s => s.Color == color).ToList();
			SpawnArea spawn = spawns.Any() ?
				(spawns.Count == 1 ? spawns[0] : spawns[Random.Range(0, spawns.Count)])
				: null;

			Quaternion rotation = Quaternion.identity;
			if (spawn != null)
			{
				rotation = spawn.transform.rotation;
			}
			return (spawn?.GetRandomVector() ?? Vector3.zero, rotation);
		}

	}
}
