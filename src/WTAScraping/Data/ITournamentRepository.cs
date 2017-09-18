using System.Collections.Generic;
using WTAScraping.Tournaments;

namespace WTAScraping.Data
{
	public interface ITournamentRepository
	{
		void AddTournaments(IEnumerable<TournamentDetails> tournaments);
	}
}