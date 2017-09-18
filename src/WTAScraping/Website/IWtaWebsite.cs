using System.Collections.Generic;
using WTAScraping.Tournaments;

namespace WTAScraping.Website
{
	public interface IWtaWebsite
	{
		IEnumerable<TournamentDetails> GetTournamentsDetails();
	}
}