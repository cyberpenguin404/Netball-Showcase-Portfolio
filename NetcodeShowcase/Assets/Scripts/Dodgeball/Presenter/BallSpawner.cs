using Dodgeball.Model;
using UnityEngine;

namespace Dodgeball.Presenter
{
	public class BallSpawner : MonoBehaviour //No model to present
	{


		//Properties
		public Transform SpawnLocation => _spawnLocation;
		public int SpawnLocationIndex { get; set; }


		//Inspector fields
		[SerializeField] private Transform _spawnLocation;

		//Private fields
		private ArenaPresenter _arena;

		private void Awake()
		{
			_arena = FindAnyObjectByType<ArenaPresenter>();
		}

		public void SpawnPurchasedBall()
		{
			_arena.RequestBallSpawnFromShop(SpawnLocationIndex);
		}

		//private void OnTriggerEnter(Collider other)
		//{
		//	if (other.CompareTag("Player"))
		//	{
		//		PlayerModel playerModel = other.GetComponent<PlayerPresenter>()?.Model;
		//		SpawnBall();
		//	}
		//}
	}
}