using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraper.Data;
using WTAScraper.Tournaments;
using Xunit;

namespace WTAScraper.UnitTests.Data
{
	public class TournamentRepositoryTests
	{
		[Fact]
		public void GetTournaments_FileDoesNotExist_ReturnsEmptyCollection()
		{
			var repository = new TournamentRepository("does_not_exist.json", new DateTime(2000, 1, 20));

			Assert.Empty(repository.GetTournaments());
		}

		[Fact]
		public void AddTournaments_NoOverlappingTournaments_NewTournamentsAreSaved()
		{
			var expectedTournaments = new[]
			{
				new TournamentDetails(1, "T1", DateTime.Now, TournamentStatus.Finished, new[] { "P1" }),
				new TournamentDetails(2, "T2", DateTime.Now, TournamentStatus.Current, new[] { "P2" })
			};
			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
				new TournamentDetails(1, "T1", DateTime.Now, TournamentStatus.Finished, null),
				new TournamentDetails(2, "T2", DateTime.Now, TournamentStatus.Current, null),
				new TournamentDetails(3, "T3", DateTime.Now, TournamentStatus.Current, null)
			};
			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Current, new[] { "P1" }),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P2" })
			};

			var newTournaments = new[]
			{
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Upcomming, new[] { "P3" })
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, new[] { "P1" }),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Current, new[] { "P2" }),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Upcomming, new[] { "P3" })
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
		public void AddTournaments_TournamentNotFound_DateIsBeforeCurrent_StatusIsChangedToFinished()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(2000, 1, 18), TournamentStatus.Current, null),
				new TournamentDetails(2, "T2", new DateTime(2000, 1, 17), TournamentStatus.Upcomming, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null)
			};

			var newTournaments = new[]
			{
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(2000, 1, 18), TournamentStatus.Finished, null),
				new TournamentDetails(2, "T2", new DateTime(2000, 1, 17), TournamentStatus.Finished, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, null)
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
		public void AddTournaments_TournamentNotFound_DateIsAfterCurrent_StatusIsNotChangedToFinished()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(2000, 1, 19), TournamentStatus.Current, null),
				new TournamentDetails(2, "T2", new DateTime(2000, 1, 21), TournamentStatus.Upcomming, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null)
			};

			var newTournaments = new[]
			{
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails(1, "T2", new DateTime(2000, 1, 21), TournamentStatus.Upcomming, null),
				new TournamentDetails(2, "T1", new DateTime(2000, 1, 19), TournamentStatus.Current, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, null)
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Upcomming, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Upcomming, null)
			};

			var newTournaments = new[]
			{
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, null)
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Upcomming, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Upcomming, new[] { "P31", "P32" }),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, new[] { "P4" }),
				new TournamentDetails(5, "T5", new DateTime(0), TournamentStatus.Current, new[] { "P5" })
			};

			var updatedTournaments = new[]
			{
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P21", "P22" }),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, new[] { "P31", "P32", "P33" }),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, new[] { "P4" })
			};

			var expectedTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P21", "P22" }),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, new[] { "P31", "P32", "P33" }),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Current, new[] { "P4" }),
				new TournamentDetails(5, "T5", new DateTime(0), TournamentStatus.Current, new[] { "P5" })
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
		public void UpdateTournaments_TournamentDoesNotExist_ExceptionIsThrown()
		{
			var previousTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, null)
			};

			var updatedTournaments = new[]
			{
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Upcomming, new[] { "P21", "P22" })
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(previousTournaments);

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Verifiable();

			Assert.Throws<Exception>(() => repositoryMock.Object.UpdateTournaments(updatedTournaments));
		}

		[Fact]
		public void GetTournaments_NoFilter_AllTournamentsAreReturned()
		{
			var existingTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, new[] { "P1" }),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Finished, new[] { "P2" }),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, new[] { "P3" }),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Upcomming, new[] { "P4" })
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

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
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Upcomming, null)
			};

			var existingTournaments = new[]
			{
				new TournamentDetails(1, "T1", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails(2, "T2", new DateTime(0), TournamentStatus.Finished, null),
				new TournamentDetails(3, "T3", new DateTime(0), TournamentStatus.Current, null),
				new TournamentDetails(4, "T4", new DateTime(0), TournamentStatus.Upcomming, null)
			};
			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(existingTournaments);

			var filteredTournaments =
				repositoryMock.Object.GetTournaments(t => t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming);

			Assert.Equal(expectedTournaments, filteredTournaments);
		}

		private static Mock<TournamentRepository> GetTournamentRepositoryMock()
		{
			return new Mock<TournamentRepository>(string.Empty, new DateTime(2000, 1, 20)) { CallBase = true };
		}
	}
}
