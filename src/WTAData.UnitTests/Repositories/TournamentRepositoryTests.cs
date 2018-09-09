using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAData.Repositories;
using WTAData.Tournaments;
using WTAData.Tournaments.DataAccess;
using Xunit;

namespace WTAData.UnitTests.Repositories
{
	public class TournamentRepositoryTests
	{
		[Fact]
		public void AddOrUpdateNewTournaments_Deprecated_NoOverlappingTournaments_NewTournamentsAreSaved()
		{
			var expectedTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished, new[] { "P1" }, DateTime.Now),
				GetTournamentDetails(2, TournamentStatus.Current, new[] { "P2" }, DateTime.Now)
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(new[] { expectedTournaments[0] });

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(
					tournaments => savedTournaments = tournaments.OrderBy(t => t.StartDate).ToArray())
				.Verifiable();

			repositoryMock.Object.AddOrUpdateNewTournaments_Deprecated(new[] { expectedTournaments[1] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddOrUpdateNewTournaments_Deprecated_OverlappingTournaments_NewTournamentsAreSaved()
		{
			var expectedTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished, null, DateTime.Now),
				GetTournamentDetails(2, TournamentStatus.Current, null, DateTime.Now),
				GetTournamentDetails(3, TournamentStatus.Current, null, DateTime.Now)
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(new[] { expectedTournaments[0], expectedTournaments[1] });

			var savedTournaments = Enumerable.Empty<TournamentDetails>();

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Callback<IEnumerable<TournamentDetails>>(
					tournaments => savedTournaments = tournaments.OrderBy(t => t.StartDate).ToArray())
				.Verifiable();

			repositoryMock.Object.AddOrUpdateNewTournaments_Deprecated(new[] { expectedTournaments[1], expectedTournaments[2] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddOrUpdateNewTournaments_Deprecated_OverlappingTournaments_PreviousTournamentsDontLosePlayers()
		{
			var previousTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Current, new[] { "P1" }),
				GetTournamentDetails(2, TournamentStatus.Upcomming, new[] { "P2" })
			};

			var newTournaments = new[]
			{
				GetTournamentDetails(2, TournamentStatus.Current),
				GetTournamentDetails(3, TournamentStatus.Upcomming, new[] { "P3" })
			};

			var expectedTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Current, new[] { "P1" }),
				GetTournamentDetails(2, TournamentStatus.Current, new[] { "P2" }),
				GetTournamentDetails(3, TournamentStatus.Upcomming, new[] { "P3" })
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

			repositoryMock.Object.AddOrUpdateNewTournaments_Deprecated(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddOrUpdateNewTournaments_Deprecated_UpcommingChangedStatusToCurrent_StatusIsChangedToCurrent()
		{
			var previousTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Current),
				GetTournamentDetails(2, TournamentStatus.Upcomming),
				GetTournamentDetails(3, TournamentStatus.Upcomming)
			};

			var newTournaments = new[]
			{
				GetTournamentDetails(3, TournamentStatus.Current),
				GetTournamentDetails(4, TournamentStatus.Current)
			};

			var expectedTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Current),
				GetTournamentDetails(2, TournamentStatus.Upcomming),
				GetTournamentDetails(3, TournamentStatus.Current),
				GetTournamentDetails(4, TournamentStatus.Current)
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

			repositoryMock.Object.AddOrUpdateNewTournaments_Deprecated(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void UpdateTournaments_Deprecated_TournamentsAreUpdated()
		{
			var previousTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished),
				GetTournamentDetails(2, TournamentStatus.Upcomming),
				GetTournamentDetails(3, TournamentStatus.Upcomming, new[] { "P31", "P32" }),
				GetTournamentDetails(4, TournamentStatus.Current, new[] { "P4" }),
				GetTournamentDetails(5, TournamentStatus.Current, new[] { "P5" })
			};

			var updatedTournaments = new[]
			{
				GetTournamentDetails(2, TournamentStatus.Upcomming, new[] { "P21", "P22" }),
				GetTournamentDetails(3, TournamentStatus.Current, new[] { "P31", "P32", "P33" }),
				GetTournamentDetails(4, TournamentStatus.Current, new[] { "P4" })
			};

			var expectedTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished),
				GetTournamentDetails(2, TournamentStatus.Upcomming, new[] { "P21", "P22" }),
				GetTournamentDetails(3, TournamentStatus.Current, new[] { "P31", "P32", "P33" }),
				GetTournamentDetails(4, TournamentStatus.Current, new[] { "P4" }),
				GetTournamentDetails(5, TournamentStatus.Current, new[] { "P5" })
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

			repositoryMock.Object.UpdateTournaments_Deprecated(updatedTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void UpdateTournaments_Deprecated_TournamentDoesNotExist_ExceptionIsThrown()
		{
			var previousTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished)
			};

			var updatedTournaments = new[]
			{
				GetTournamentDetails(4, TournamentStatus.Upcomming, new[] { "P21", "P22" })
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(previousTournaments);

			repositoryMock.Protected()
				.Setup("SaveTournaments", ItExpr.IsAny<IEnumerable<TournamentDetails>>())
				.Verifiable();

			Assert.Throws<Exception>(() => repositoryMock.Object.UpdateTournaments_Deprecated(updatedTournaments));
		}

		[Fact]
		public void AddOrUpdateNewTournaments_NoOverlappingTournaments_UpdateAndAddAreBothCalled()
		{
			var tournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Upcomming, new[] { "P1" }, DateTime.Now),
				GetTournamentDetails(2, TournamentStatus.Current, new[] { "P2" }, DateTime.Now)
			};

			var dataAccessMock = GetTournamentDataAccessMock(tryUpdateTournamentStatusResult: false);

			var repository = new TournamentRepository(dataAccessMock.Object, string.Empty, DateTime.Now);

			repository.AddOrUpdateNewTournaments(tournaments);

			dataAccessMock.Verify(da => da.TryAddTournament(It.IsAny<TournamentDetails>()), Times.Exactly(2));
			dataAccessMock.Verify(
				da => da.TryUpdateTournamentStatus(
					It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TournamentStatus>()), Times.Exactly(2));
		}

		[Fact]
		public void AddOrUpdateNewTournaments_OverlappingTournaments_OnlyUpdateIsCalled()
		{
			var tournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Upcomming, new[] { "P1" }, DateTime.Now),
				GetTournamentDetails(2, TournamentStatus.Current, new[] { "P2" }, DateTime.Now)
			};

			var dataAccessMock = GetTournamentDataAccessMock(tryUpdateTournamentStatusResult: true);

			var repository = new TournamentRepository(dataAccessMock.Object, string.Empty, DateTime.Now);

			repository.AddOrUpdateNewTournaments(tournaments);

			dataAccessMock.Verify(da => da.TryAddTournament(It.IsAny<TournamentDetails>()), Times.Never);
			dataAccessMock.Verify(
				da => da.TryUpdateTournamentStatus(
					It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TournamentStatus>()), Times.Exactly(2));
		}

		[Fact]
		public void CleanupFinishedTournaments_TournamentNotFound_DateIsBeforeCurrent_StatusIsChangedToFinished()
		{
			var previousTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Current, null, new DateTime(2000, 1, 18)),
				GetTournamentDetails(2, TournamentStatus.Upcomming, null, new DateTime(2000, 1, 17)),
				GetTournamentDetails(3, TournamentStatus.Current, null)
			};

			var newTournaments = new[]
			{
				GetTournamentDetails(3, TournamentStatus.Current, null),
				GetTournamentDetails(4, TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished, null, new DateTime(2000, 1, 18)),
				GetTournamentDetails(2, TournamentStatus.Finished, null, new DateTime(2000, 1, 17)),
				GetTournamentDetails(3, TournamentStatus.Current, null)
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

			repositoryMock.Object.CleanupFinishedTournaments_Deprecated(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void CleanupFinishedTournaments_TournamentNotFound_DateIsAfterCurrent_StatusIsNotChangedToFinished()
		{
			var previousTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Current, null, new DateTime(2000, 1, 19)),
				GetTournamentDetails(2, TournamentStatus.Upcomming, null, new DateTime(2000, 1, 21)),
				GetTournamentDetails(3, TournamentStatus.Current, null)
			};

			var newTournaments = new[]
			{
				GetTournamentDetails(3, TournamentStatus.Current, null),
				GetTournamentDetails(4, TournamentStatus.Current, null)
			};

			var expectedTournaments = new[]
			{
				GetTournamentDetails(2, TournamentStatus.Upcomming, null, new DateTime(2000, 1, 21)),
				GetTournamentDetails(1, TournamentStatus.Current, null, new DateTime(2000, 1, 19)),
				GetTournamentDetails(3, TournamentStatus.Current, null)
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

			repositoryMock.Object.CleanupFinishedTournaments_Deprecated(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void GetTournaments_NoFilter_AllTournamentsAreReturned()
		{
			var existingTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished, new[] { "P1" }),
				GetTournamentDetails(2, TournamentStatus.Finished, new[] { "P2" }),
				GetTournamentDetails(3, TournamentStatus.Current, new[] { "P3" }),
				GetTournamentDetails(4, TournamentStatus.Upcomming, new[] { "P4" })
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(existingTournaments);

			var filteredTournaments = repositoryMock.Object.GetTournaments_Deprecated();

			Assert.Equal(existingTournaments, filteredTournaments);
		}

		[Fact]
		public void GetTournaments_FilterByStatus_CorrectTournamentsAreReturned()
		{
			var expectedTournaments = new[]
			{
				GetTournamentDetails(3, TournamentStatus.Current),
				GetTournamentDetails(4, TournamentStatus.Upcomming)
			};

			var existingTournaments = new[]
			{
				GetTournamentDetails(1, TournamentStatus.Finished),
				GetTournamentDetails(2, TournamentStatus.Finished),
				GetTournamentDetails(3, TournamentStatus.Current),
				GetTournamentDetails(4, TournamentStatus.Upcomming)
			};

			Mock<TournamentRepository> repositoryMock = GetTournamentRepositoryMock();

			repositoryMock.Protected()
				.Setup<IEnumerable<TournamentDetails>>("GetTournaments")
				.Returns(existingTournaments);

			var filteredTournaments =
				repositoryMock.Object.GetTournaments_Deprecated(t => t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming);

			Assert.Equal(expectedTournaments, filteredTournaments);
		}

		[Fact]
		public void GetTournaments_FileDoesNotExist_ReturnsEmptyCollection()
		{
			var repository = new TournamentRepository(null, "does_not_exist.json", new DateTime(2000, 1, 20));

			Assert.Empty(repository.GetTournaments_Deprecated());
		}

		private static Mock<TournamentRepository> GetTournamentRepositoryMock()
		{
			return new Mock<TournamentRepository>(null, string.Empty, new DateTime(2000, 1, 20)) { CallBase = true };
		}

		private static Mock<AWSTournamentDataAccess> GetTournamentDataAccessMock(bool tryUpdateTournamentStatusResult)
		{
			var dataAccessMock = new Mock<AWSTournamentDataAccess>(null, DateTime.Now);

			dataAccessMock.Setup(da => da.TryAddTournament(It.IsAny<TournamentDetails>())).Returns(true);

			dataAccessMock
				.Setup(da => da.TryUpdateTournamentStatus(
					It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TournamentStatus>()))
				.Returns(tryUpdateTournamentStatusResult);

			int tournamentId;
			dataAccessMock
				.Setup(da => da.TryFindTournamentId(It.IsAny<string>(), It.IsAny<DateTime>(), out tournamentId))
				.Returns(false);

			return dataAccessMock;
		}

		private static TournamentDetails GetTournamentDetails(
			int index, TournamentStatus status, string[] players = null, DateTime? startDate = null)
		{
			return 
				new TournamentDetails(
					index, $"T{index}", startDate ?? new DateTime(0), new DateTime(0), status, players, 0);
		}
	}
}
