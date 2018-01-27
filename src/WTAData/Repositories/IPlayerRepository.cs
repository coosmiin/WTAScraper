using System.Collections.Generic;
using WTAData.Players;

namespace WTAData.Repositories
{
	public interface IPlayerRepository
	{
		void SavePlayers(IEnumerable<Player> players);
		IEnumerable<Player> GetPlayers();
	}
}
