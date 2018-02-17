using Moq;
using System;
using System.Linq;
using WTAScraper.Driver;
using WTAData.Tournaments;
using WTAScraper.Formatters;
using WTAScraper.Website;
using Xunit;
using WTAData.Players;
using System.Collections.Generic;

namespace WTAScraper.UnitTests.Website
{
	public class WtaWebsiteTests
	{
		[Fact]
		public void RefreshSeededPlayers_CurrentTournamentsWithoutDrawsBecomeInvalid()
		{
			var tournaments =
				new List<Tournament> { new Tournament(1, "T1", new DateTime(2017, 1, 1), new DateTime(0), TournamentStatus.Current) };

			var driverStub = Mock.Of<IWtaDriver>(d => 
				d.GetTournamentPlayers(It.IsAny<string>()) == Enumerable.Empty<SeededPlayer>());

			var driverFactoryStub = Mock.Of<IWtaDriverFactory>(f => f.CreateDriver() == driverStub);

			var website = new WtaWebsite(driverFactoryStub, new UrlFormatter(), new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails());

			Assert.Equal(TournamentStatus.Invalid, tournamentsDetails.ToArray()[0].Status);
		}

		[Fact]
		public void RefreshSeededPlayers_UpcommingTournamentsWithoutDrawsDontChangeStatus()
		{
			var tournaments =
				new List<Tournament> { new Tournament(1, "T1", new DateTime(2017, 1, 1), new DateTime(0), TournamentStatus.Upcomming) };

			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetTournamentPlayers(It.IsAny<string>()) == Enumerable.Empty<SeededPlayer>());

			var driverFactoryStub = Mock.Of<IWtaDriverFactory>(f => f.CreateDriver() == driverStub);

			var website = new WtaWebsite(driverFactoryStub, new UrlFormatter(), new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails());

			Assert.Equal(TournamentStatus.Upcomming, tournamentsDetails.ToArray()[0].Status);
		}

		[Fact]
		public void RefreshSeededPlayers_TournamentsWithDraws_SeededPlayersAreReturnedInCorrectOrder()
		{
			var tournaments =
				new List<Tournament>
				{
					new Tournament(1, "T1", new DateTime(2017, 1, 1), new DateTime(0), TournamentStatus.Upcomming),
					new Tournament(3, "T3", new DateTime(2017, 1, 1), new DateTime(0), TournamentStatus.Current)
				};

			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetTournamentPlayers("2017T11") == 
					new[] { new SeededPlayer("P2", 2), new SeededPlayer("P3", 3), new SeededPlayer("P1", 1) } &&
				d.GetTournamentPlayers("2017T33") == new[] { new SeededPlayer("P4", 1) });

			var driverFactoryStub = Mock.Of<IWtaDriverFactory>(f => f.CreateDriver() == driverStub);

			var urlFormatterMock = new Mock<IUrlFormatter>();
			urlFormatterMock
				.Setup(u => u.GetTournamentUrl(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
				.Returns((int i, string n, DateTime d) => d.Year + n + i);

			var website = new WtaWebsite(driverFactoryStub, urlFormatterMock.Object, new PlayerNameFormatter());

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
				new List<Tournament> { new Tournament(1, "T1", new DateTime(2017, 1, 1), new DateTime(0), TournamentStatus.Current) };

			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetTournamentPlayers(It.IsAny<string>()) ==
					new[] { new SeededPlayer("P2", 2), new SeededPlayer("P3", SeededPlayer.MAX_SEED), new SeededPlayer("P1", 1) });

			var driverFactoryStub = Mock.Of<IWtaDriverFactory>(f => f.CreateDriver() == driverStub);

			var website = new WtaWebsite(driverFactoryStub, new UrlFormatter(), new PlayerNameFormatter());

			var tournamentsDetails = website.RefreshSeededPlayers(tournaments.AsTournamentDetails()).ToArray();

			Assert.Equal(new[] { "P1", "P2" }, tournamentsDetails[0].SeededPlayerNames);
		}
	}
}
