using System.Collections.Generic;
using WTAData.Players;
using WTAData.Tournaments;

namespace WTAScraper.Website
{
	public interface IWtaWebsite
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();

		IEnumerable<Player> GetPlayers();

		IEnumerable<TournamentDetails> RefreshSeededPlayers(IEnumerable<TournamentDetails> tournamentsDetails);
	}
}