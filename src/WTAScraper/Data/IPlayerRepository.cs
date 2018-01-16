using System.Collections.Generic;
using WTAScraper.Players;

namespace WTAScraper.Data
{
	public interface IPlayerRepository
	{
		void SavePlayers(IEnumerable<Player> players);
		IEnumerable<Player> GetPlayers();
	}
}
