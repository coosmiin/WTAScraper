using System.Collections.Generic;
using WTAData.Players;
using WTAData.Tournaments;

namespace WTAScraper.Driver
{
	public interface IWtaDriver
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();
		IEnumerable<SeededPlayer> GetTournamentPlayers(string tournamentNameUrl);
	}
}