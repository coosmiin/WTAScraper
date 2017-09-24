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
				new TournamentDetails("T1", DateTime.Now, TournamentStatus.Finished, new[] { "P1" }),
				new TournamentDetails("T2", DateTime.Now, TournamentStatus.Current, new[] { "P2" })
			};
			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(new[] { expectedTournaments[0] });

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(
					tournaments => savedTournaments = tournaments.OrderBy(t => t.Date).ToArray())
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
				new TournamentDetails("T1", DateTime.Now, TournamentStatus.Finished, null),
				new TournamentDetails("T2", DateTime.Now, TournamentStatus.Current, null),
				new TournamentDetails("T3", DateTime.Now, TournamentStatus.Current, null)
			};
			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(new[] { expectedTournaments[0], expectedTournaments[1] });

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(
					tournaments => savedTournaments = tournaments.OrderBy(t => t.Date).ToArray())
				.Verifiable();

			repositoryMock.Object.AddTournaments(new[] { expectedTournaments[1], expectedTournaments[2] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_OverlappingTournaments_PreviousTournamentsDontLosePlayers()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Current, new[] { "P1" }),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P2" })
			};

			var newTournaments = new[]
			{
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Upcomming, new[] { "P3" })
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, new[] { "P1" }),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Current, new[] { "P2" }),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Upcomming, new[] { "P3" })
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(previousTournaments);

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(
					tournaments => savedTournaments = tournaments.ToArray())
				.Verifiable();

			repositoryMock.Object.AddTournaments(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_FinishedTournaments_StatusIsChangedToFinished()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null)
			};

			var newTournaments = new[]
			{
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
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
		public void AddTournaments_UpcommingChangedStatusToCurrent_StatusIsChangedToCurrent()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Upcomming, null)
			};

			var newTournaments = new[]
			{
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
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
		public void UpdateTournaments_TournamentsAreUpdated()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Upcomming, new[] { "P31", "P32" }),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, new[] { "P4" }),
				new TournamentDetails("T5", new DateTime(0), TournamentStatus.Current, new[] { "P5" })
			};

			var updatedTournaments = new[]
			{
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P21", "P22" }),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, new[] { "P31", "P32", "P33" }),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, new[] { "P4" })
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P21", "P22" }),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, new[] { "P31", "P32", "P33" }),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Current, new[] { "P4" }),
				new TournamentDetails("T5", new DateTime(0), TournamentStatus.Current, new[] { "P5" })
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(previousTournaments);

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(tournaments => savedTournaments = tournaments)
				.Verifiable();

			repositoryMock.Object.UpdateTournaments(updatedTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void GetTournaments_NoFilter_AllTournamentsAreReturned()
		{
			var existingTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, new[] { "P1" }),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Finished, new[] { "P2" }),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, new[] { "P3" }),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Upcomming, new[] { "P4" })
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(existingTournaments);

			var filteredTournaments = repositoryMock.Object.GetTournaments();

			Assert.Equal(existingTournaments, filteredTournaments);
		}

		[Fact]
		public void GetTournaments_FilterByStatus_CorrectTournamentsAreReturned()
		{
			var expectedTournaments = new[]
			{
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Upcomming, null)
			};

			var existingTournaments = new[]
			{
				new TournamentDetails("T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T2", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails("T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails("T4", new DateTime(0), TournamentStatus.Upcomming, null)
			};

			var repositoryMock = new Mock<TournamentRepository>(string.Empty) { CallBase = true };

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(existingTournaments);

			var filteredTournaments = 
				repositoryMock.Object.GetTournaments(t => t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming);

			Assert.Equal(expectedTournaments, filteredTournaments);
		}
	}
}
