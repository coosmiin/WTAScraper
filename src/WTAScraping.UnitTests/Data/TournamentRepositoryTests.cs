using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraping.Data;
using WTAScraping.Tournaments;
using Xunit;

namespace WTAScraping.UnitTests.Data
{
	public class TournamentRepositoryTests
	{
		[Fact]
		public void AddTournaments_NoOverlappingTournaments_NewTournamentsAreSaved()
		{
			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", DateTime.Now, TournamentStatus.Current, "P1"),
				new TournamentDetails("T2", DateTime.Now, TournamentStatus.Current, "P2")
			};
			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetAllTournaments")
				.Returns(new[] { expectedTournaments[0] });

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(tournaments => savedTournaments = tournaments)
				.Verifiable();

			repositoryMock.Object.AddTournaments(new[] { expectedTournaments[1] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_OverlappingTournaments_NewTournamentsAreSaved()
		{
			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", DateTime.Now, TournamentStatus.Current, "P1"),
				new TournamentDetails("T2", DateTime.Now, TournamentStatus.Current, "P2"),
				new TournamentDetails("T3", DateTime.Now, TournamentStatus.Current, "P3")
			};
			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetAllTournaments")
				.Returns(new[] { expectedTournaments[0], expectedTournaments[1] });

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(tournaments => savedTournaments = tournaments)
				.Verifiable();

			repositoryMock.Object.AddTournaments(new[] { expectedTournaments[1], expectedTournaments[2] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_FinishedTournamentsChangeStatusToFinished()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Current, "P1"),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, "P2"),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, "P3")
			};

			var newTournaments = new[]
			{
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, "P3"),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, "P4")
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, "P1"),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Finished, "P2"),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, "P3"),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, "P4")
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetAllTournaments")
				.Returns(previousTournaments);

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(tournaments => savedTournaments = tournaments)
				.Verifiable();

			repositoryMock.Object.AddTournaments(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_CurrentTournamentsChangeStatusToCurrent()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Current, "P1"),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, "P2"),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Upcomming, "P3")
			};

			var newTournaments = new[]
			{
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, "P3"),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, "P4")
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, "P1"),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Finished, "P2"),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, "P3"),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, "P4")
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetAllTournaments")
				.Returns(previousTournaments);

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(tournaments => savedTournaments = tournaments)
				.Verifiable();

			repositoryMock.Object.AddTournaments(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}
	}
}
