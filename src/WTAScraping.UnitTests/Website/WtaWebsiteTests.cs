using Moq;
using System;
using System.Linq;
using WTAScraping.Driver;
using WTAScraping.Tournaments;
using WTAScraping.Formatters;
using WTAScraping.Website;
using Xunit;
using WTAScraping.Players;
using System.Collections.Generic;

namespace WTAScraping.UnitTests.Website
{
	public class WtaWebsiteTests
	{
		[Fact]
		public void RefreshSeededPlayers_CurrentTournamentsWithoutDrawsBecomeInvalid()
		{
			var tournaments =
				new List<Tournament> { new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Current) };

			var driverStub = Mock.Of<IWtaDriver>(d => 
				d.GetTournamentPlayers(It.IsAny<string>()) == Enumerable.Empty<SeededPlayer>());

			var website = new WtaWebsite(driverStub, new UrlFormatter(), new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails());

			Assert.Equal(TournamentStatus.Invalid, tournamentsDetails.ToArray()[0].Status);
		}

		[Fact]
		public void RefreshSeededPlayers_UpcommingTournamentsWithoutDrawsDontChangeStatus()
		{
			var tournaments =
				new List<Tournament> { new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Upcomming) };

			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetTournamentPlayers(It.IsAny<string>()) == Enumerable.Empty<SeededPlayer>());

			var website = new WtaWebsite(driverStub, new UrlFormatter(), new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails());

			Assert.Equal(TournamentStatus.Upcomming, tournamentsDetails.ToArray()[0].Status);
		}

		[Fact]
		public void RefreshSeededPlayers_TournamentsWithDraws_SeededPlayersAreReturnedInCorrectOrder()
		{
			var tournaments =
				new List<Tournament>
				{
					new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Upcomming),
					new Tournament("T3", new DateTime(2017, 1, 1), TournamentStatus.Current)
				};

			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetTournamentPlayers("2017T1") == 
					new[] { new SeededPlayer("P2", 2), new SeededPlayer("P3", 3), new SeededPlayer("P1", 1) } &&
				d.GetTournamentPlayers("2017T3") == new[] { new SeededPlayer("P4", 1) });

			var urlFormatterMock = new Mock<IUrlFormatter>();
			urlFormatterMock
				.Setup(u => u.GetTournamentUrl(It.IsAny<string>(), It.IsAny<int>()))
				.Returns((string n, int y) => y + n);

			var website = new WtaWebsite(driverStub, urlFormatterMock.Object, new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails()).ToArray();

			Assert.Equal("P1", tournamentsDetails[0].SeededPlayerNames.ElementAt(0));
			Assert.Equal("P2", tournamentsDetails[0].SeededPlayerNames.ElementAt(1));
			Assert.Equal("P3", tournamentsDetails[0].SeededPlayerNames.ElementAt(2));
			Assert.Equal("P4", tournamentsDetails[1].SeededPlayerNames.ElementAt(0));
		}

		[Fact]
		public void RefreshSeededPlayers_TournamentsWithDraws_NonSeededPlayersAreNotReturned()
		{
			var tournaments =
				new List<Tournament> { new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Current) };

			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetTournamentPlayers(It.IsAny<string>()) ==
					new[] { new SeededPlayer("P2", 2), new SeededPlayer("P3", SeededPlayer.MAX_SEED), new SeededPlayer("P1", 1) });

			var website = new WtaWebsite(driverStub, new UrlFormatter(), new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails()).ToArray();

			Assert.Equal(new[] { "P1", "P2" }, tournamentsDetails[0].SeededPlayerNames);
		}
	}
}
