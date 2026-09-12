
using Assets.Scripts.Dodgeball.Network;
using Dodgeball.Model;
using MVP.Presenter;
using System.Threading.Tasks;
using UnityEngine;

namespace Dodgeball.Presenter
{
	public class MatchPresenter : PresenterMonobehaviour<MatchModel>
	{
		[SerializeField]
		private ArenaPresenter _arena;

		[SerializeField]
		private MatchSync _networkSync;

        public async Task StartMatch(MatchModel model)
		{
			Model = model;
			Model.StartMatch(_arena.Model);

            _networkSync.Model = Model;
            await _networkSync.InitializeAsync();

            //Spawn ball
            _arena.SpawnBall(0);

			_arena.SpawnPlayer(model.PlayerRed.PlayerId);
			_arena.SpawnPlayer(model.PlayerBlue.PlayerId);
        }
    }
}
