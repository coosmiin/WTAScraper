using System.Collections.Generic;
using WTAScraping.Players;

namespace WTAScraping.Data
{
	public interface IPlayerRepository
	{
		void SavePlayers(IEnumerable<Player> players);
		IEnumerable<Player> GetPlayers();
	}
}
