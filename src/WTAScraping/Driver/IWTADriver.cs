using System.Collections.Generic;
using WTAScraping.Players;
using WTAScraping.Tournaments;

namespace WTAScraping.Driver
{
	public interface IWtaDriver
	{
		IEnumerable<Tournament> GetCurrentAndUpcomingTournaments();
		IEnumerable<SeededPlayer> GetTournamentPlayers(string tournamentNameUrl);
	}
}