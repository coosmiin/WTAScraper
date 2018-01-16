using System.Collections.Generic;
using WTAScraper.Players;
using WTAScraper.Tournaments;

namespace WTAScraper.Driver
{
	public interface IWtaDriver
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();
		IEnumerable<SeededPlayer> GetTournamentPlayers(string tournamentNameUrl);
	}
}