using System.Collections.Generic;
using WTAScraping.Tournaments;

namespace WTAScraping.Driver
{
	public interface IWtaDriver
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();
		IEnumerable<string> GetCurrentTournamentNameUrls();
		IEnumerable<Player> GetTournamentPlayers(string tournamentNameUrl);
	}
}