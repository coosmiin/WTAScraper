using System;
using System.Collections.Generic;

namespace WTAData.Tournaments.DataAccess
{
	public interface ITournamentDataAccess
	{
		/// <summary>
		/// Tries to add a new tournament. It does not update and returns 'false' if the tournament exists.
		/// </summary>
		bool TryAddTournament(TournamentDetails tournament);

		/// <summary>
		/// Tries to update tournament status. It does not add new and returns 'false' if the tournament does not exist.
		/// </summary>
		bool TryUpdateTournamentStatus(int tournamentId, DateTime startDate, TournamentStatus status);

		void UpdateTournament(TournamentDetails tournament);

		bool TryFindTournamentId(string name, DateTime startDate, out int tournamentId);

		/// <summary>
		/// Gets fresh (Current or Upcomming) tournaments without players
		/// </summary>
		IEnumerable<TournamentDetails> GetFreshTournamentsWithoutPlayers();

		/// <summary>
		/// Gets finished tournaments in invalid state
		/// </summary>
		IEnumerable<TournamentDetails> GetOldUnfinishedTournaments();
	}
}
