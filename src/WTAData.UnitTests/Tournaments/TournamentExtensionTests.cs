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
			var tournamentDate = DateTime.Now;
			var tournamentStatus = TournamentStatus.Current;

			var tournament = new Tournament(tournamentId, tournamentName, tournamentDate, tournamentStatus);
			var tournamentDetails = tournament.AsTournamentDetails();

			Assert.Equal(tournamentId, tournamentDetails.Id);
			Assert.Equal(tournamentName, tournamentDetails.Name);
			Assert.Equal(tournamentDate, tournamentDetails.Date);
			Assert.Equal(tournamentStatus, tournamentDetails.Status);
		}
	}
}
