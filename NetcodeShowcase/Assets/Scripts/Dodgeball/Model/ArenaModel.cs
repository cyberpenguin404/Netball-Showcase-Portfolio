using MVP.Model;
using System.Collections.Generic;
using System.Linq;

namespace Dodgeball.Model
{

	/// <summary>
	/// Arena keeps track of the entities on the field.
	/// </summary>
	public class ArenaModel : ModelBase
	{
		private List<PlayerModel> _players = new List<PlayerModel>();
		private List<BallModel> _balls = new List<BallModel>();

		public void AddPlayer(PlayerModel player)
		{
			_players.Add(player);
		}
		public void AddBall(BallModel ball)
		{
			_balls.Add(ball);
		}

		public PlayerModel GetPlayer(ulong playerId)
		{
			return _players.FirstOrDefault(p => p.PlayerId == playerId);
		}

	}
}
