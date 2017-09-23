using System;
using Xunit;

namespace WTAScraping.Tournaments.UnitTests
{
	public class TournamentExtensionTests
	{
		[Fact]
		public void AsTournamentDetails_GivenValidTournament_ValuesArePreservedInDetails()
		{
			var tournamentName = "Some Name";
			var tournamentDate = DateTime.Now;
			var tournamentStatus = TournamentStatus.Current;

			var tournament = new Tournament(tournamentName, tournamentDate, tournamentStatus);
			var tournamentDetails = tournament.AsTournamentDetails();

			Assert.Equal(tournamentName, tournamentDetails.Name);
			Assert.Equal(tournamentDate, tournamentDetails.Date);
			Assert.Equal(tournamentStatus, tournamentDetails.Status);
		}
	}
}
