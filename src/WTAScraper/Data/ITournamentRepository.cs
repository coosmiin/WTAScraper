using System;
using System.Collections.Generic;
using WTAScraper.Tournaments;

namespace WTAScraper.Data
{
	public interface ITournamentRepository
	{
		void AddTournaments(IEnumerable<TournamentDetails> tournaments);

		void UpdateTournaments(IEnumerable<TournamentDetails> tournamentsDetails);

		IEnumerable<TournamentDetails> GetTournaments(Func<TournamentDetails, bool> predicate = null);
	}
}