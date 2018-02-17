using System;
using WTAData.Tournaments;
using Xunit;

namespace WTAData.UnitTests.Tournaments
{
	public class TournamentExtensionTests
	{
		[Fact]
		public void AsTournamentDetails_GivenValidTournament_ValuesArePreservedInDetails()
		{
			var tournamentId = 1;
			var tournamentName = "Some Name";
			var tournamentStartDate = DateTime.Now;
			var tournamentEndDate = DateTime.Now.AddDays(1);
			var tournamentStatus = TournamentStatus.Current;

			var tournament = new Tournament(tournamentId, tournamentName, tournamentStartDate, tournamentEndDate, tournamentStatus);
			var tournamentDetails = tournament.AsTournamentDetails();

			Assert.Equal(tournamentId, tournamentDetails.Id);
			Assert.Equal(tournamentName, tournamentDetails.Name);
			Assert.Equal(tournamentStartDate, tournamentDetails.StartDate);
			Assert.Equal(tournamentEndDate, tournamentDetails.EndDate);
			Assert.Equal(tournamentStatus, tournamentDetails.Status);
		}
	}
}
