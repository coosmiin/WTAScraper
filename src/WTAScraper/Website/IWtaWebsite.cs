using System.Collections.Generic;
using WTAScraper.Players;
using WTAScraper.Tournaments;

namespace WTAScraper.Website
{
	public interface IWtaWebsite
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();

		IEnumerable<Player> GetPlayers();

		IEnumerable<TournamentDetails> RefreshSeededPlayers(IEnumerable<TournamentDetails> tournamentsDetails);
	}
}