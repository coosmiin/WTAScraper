namespace WTAScraping.Tournaments
{
	public static class TournamentExtensions
	{
		public static TournamentDetails AsTournamentDetails(this Tournament tournament, string firstSeed)
		{
			return new TournamentDetails(tournament.Name, tournament.Date, tournament.Status, firstSeed);
		}
	}
}
