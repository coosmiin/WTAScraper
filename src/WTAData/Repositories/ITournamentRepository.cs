using System;
using System.Collections.Generic;
using WTAData.Tournaments;

namespace WTAData.Repositories
{
	public interface ITournamentRepository
	{
		IEnumerable<TournamentDetails> GetTournaments_Deprecated(Func<TournamentDetails, bool> predicate = null);

		void AddOrUpdateNewTournaments(IEnumerable<TournamentDetails> tournaments);

		void AddOrUpdateNewTournaments_Deprecated(IEnumerable<TournamentDetails> tournaments);

		void UpdateTournaments(IEnumerable<TournamentDetails> tournamentsDetails);

		void UpdateTournaments_Deprecated(IEnumerable<TournamentDetails> tournamentsDetails);

		void CleanupFinishedTournaments();

		void CleanupFinishedTournaments_Deprecated(IEnumerable<TournamentDetails> tournaments);

		IEnumerable<TournamentDetails> GetFreshTournamentsWithoutPlayers();
	}
}