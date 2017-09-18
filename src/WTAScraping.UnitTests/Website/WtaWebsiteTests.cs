using Moq;
using System;
using System.Linq;
using WTAScraping.Driver;
using WTAScraping.Tournaments;
using WTAScraping.UrlFormatters;
using WTAScraping.Website;
using Xunit;

namespace WTAScraping.UnitTests.Website
{
	public class WtaWebsiteTests
	{
		[Fact]
		public void GetTournamentsDetails_CurrentTournamentsWithoutDrawsBecomeInvalid()
		{
			var driverStub = Mock.Of<IWtaDriver>(d => 
				d.GetCurrentAndUpcomingTournaments() 
				== new[] 
				{
					new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Current),
					new Tournament("T2", new DateTime(2017, 1, 1), TournamentStatus.Current),
					new Tournament("T3", new DateTime(2017, 1, 1), TournamentStatus.Current)
				} &&
				d.GetTournamentPlayers("2017T1") == new [] { new Player("P1", 1), new Player("P2", 2) } &&
				d.GetTournamentPlayers("2017T2") == Enumerable.Empty<Player>() &&
				d.GetTournamentPlayers("2017T3") == new[] { new Player("P3", 1) });

			var urlFormatterMock = new Mock<IUrlFormatter>();
			urlFormatterMock
				.Setup(u => u.GetTournamentUrl(It.IsAny<string>(), It.IsAny<int>()))
				.Returns((string n, int y) => y + n);

			var website = new WtaWebsite(driverStub, urlFormatterMock.Object);

			Assert.Equal(TournamentStatus.Invalid, website.GetTournamentsDetails().ToArray()[1].Status);
		}

		[Fact]
		public void GetTournamentsDetails_UpcommingTournamentsWithoutDrawsDontChangeStatus()
		{
			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetCurrentAndUpcomingTournaments()
				== new[]
				{
					new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Upcomming),
					new Tournament("T2", new DateTime(2017, 1, 1), TournamentStatus.Upcomming),
					new Tournament("T3", new DateTime(2017, 1, 1), TournamentStatus.Upcomming)
				} &&
				d.GetTournamentPlayers("2017T1") == new[] { new Player("P1", 1), new Player("P2", 2) } &&
				d.GetTournamentPlayers("2017T2") == Enumerable.Empty<Player>() &&
				d.GetTournamentPlayers("2017T3") == new[] { new Player("P3", 1) });

			var urlFormatterMock = new Mock<IUrlFormatter>();
			urlFormatterMock
				.Setup(u => u.GetTournamentUrl(It.IsAny<string>(), It.IsAny<int>()))
				.Returns((string n, int y) => y + n);

			var website = new WtaWebsite(driverStub, urlFormatterMock.Object);

			Assert.Equal(TournamentStatus.Upcomming, website.GetTournamentsDetails().ToArray()[1].Status);
		}

		[Fact]
		public void GetTournamentsDetails_TournamentsWithDraws_FirstSeedPlayersAreReturned()
		{
			var driverStub = Mock.Of<IWtaDriver>(d =>
				d.GetCurrentAndUpcomingTournaments()
				== new[]
				{
					new Tournament("T1", new DateTime(2017, 1, 1), TournamentStatus.Upcomming),
					new Tournament("T2", new DateTime(2017, 1, 1), TournamentStatus.Current),
					new Tournament("T3", new DateTime(2017, 1, 1), TournamentStatus.Current)
				} &&
				d.GetTournamentPlayers("2017T1") == new[] { new Player("P1", 1), new Player("P2", 2) } &&
				d.GetTournamentPlayers("2017T2") == Enumerable.Empty<Player>() &&
				d.GetTournamentPlayers("2017T3") == new[] { new Player("P3", 1) });

			var urlFormatterMock = new Mock<IUrlFormatter>();
			urlFormatterMock
				.Setup(u => u.GetTournamentUrl(It.IsAny<string>(), It.IsAny<int>()))
				.Returns((string n, int y) => y + n);

			var website = new WtaWebsite(driverStub, urlFormatterMock.Object);

			var tournaments = website.GetTournamentsDetails().ToArray();

			Assert.Equal("P1", tournaments[0].FirstSeed);
			Assert.Null(tournaments[1].FirstSeed);
			Assert.Equal("P3", tournaments[2].FirstSeed);
		}
	}
}
