using System;
using System.Collections.Generic;
using WTAScraping.Tournaments;

namespace WTAScraping.Data
{
	public interface ITournamentRepository
	{
		void AddTournaments(IEnumerable<TournamentDetails> tournaments);

		void UpdateTournaments(IEnumerable<TournamentDetails> tournamentsDetails);

		IEnumerable<TournamentDetails> GetTournaments(Func<TournamentDetails, bool> predicate = null);
	}
}