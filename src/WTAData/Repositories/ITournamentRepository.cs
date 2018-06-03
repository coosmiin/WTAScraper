using System;
using System.Collections.Generic;
using WTAData.Tournaments;

namespace WTAData.Repositories
{
	public interface ITournamentRepository
	{
		void AddOrUpdateNewTournaments(IEnumerable<TournamentDetails> tournaments);
		void AddOrUpdateNewTournaments_Deprecated(IEnumerable<TournamentDetails> tournaments);

		void CleanupFinishedTournaments(IEnumerable<TournamentDetails> tournaments);

		void UpdateTournaments(IEnumerable<TournamentDetails> tournamentsDetails);

		IEnumerable<TournamentDetails> GetTournaments(Func<TournamentDetails, bool> predicate = null);
	}
}