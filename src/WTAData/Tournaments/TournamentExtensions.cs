using System.Collections.Generic;
using System.Linq;

namespace WTAData.Tournaments
{
	public static class TournamentExtensions
	{
		public static TournamentDetails AsTournamentDetails(this Tournament tournament, IEnumerable<string> seededPlayerNames = null)
		{
			return 
				new TournamentDetails(
					tournament.Id, tournament.Name, tournament.StartDate, tournament.EndDate, tournament.Status, seededPlayerNames);
		}

		public static IEnumerable<TournamentDetails> AsTournamentDetails(
			this IEnumerable<Tournament> tournaments)
		{
			return tournaments.Select(t => t.AsTournamentDetails(null));
		}

	}
}
