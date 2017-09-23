using System.Collections.Generic;
using WTAScraping.Players;
using WTAScraping.Tournaments;

namespace WTAScraping.Website
{
	public interface IWtaWebsite
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();

		IEnumerable<Player> GetPlayers();

		IEnumerable<TournamentDetails> RefreshSeededPlayers(IEnumerable<TournamentDetails> tournamentsDetails);
	}
}