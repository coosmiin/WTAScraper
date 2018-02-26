using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAData.Repositories;
using WTAData.Tournaments;
using Xunit;

namespace WTAData.UnitTests.Repositories
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

			repositoryMock.Object.AddTournaments(new[] { expectedTournaments[1] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_OverlappingTournaments_NewTournamentsAreSaved()
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

			repositoryMock.Object.AddTournaments(new[] { expectedTournaments[1], expectedTournaments[2] });

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_OverlappingTournaments_PreviousTournamentsDontLosePlayers()
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
				GetTournamentDetails(1, TournamentStatus.Finished, new[] { "P1" }),
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

			repositoryMock.Object.AddTournaments(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void AddTournaments_TournamentNotFound_DateIsBeforeCurrent_StatusIsChangedToFinished()
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
				GetTournamentDetails(3, TournamentStatus.Current, null),
				GetTournamentDetails(4, TournamentStatus.Current, null)
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
				GetTournamentDetails(3, TournamentStatus.Current, null),
				GetTournamentDetails(4, TournamentStatus.Current, null)
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
				GetTournamentDetails(1, TournamentStatus.Finished),
				GetTournamentDetails(2, TournamentStatus.Finished),
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

			repositoryMock.Object.AddTournaments(newTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void UpdateTournaments_TournamentsAreUpdated()
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

			repositoryMock.Object.UpdateTournaments(updatedTournaments);

			repositoryMock.Protected().Verify("SaveTournaments", Times.Once(), ItExpr.IsAny<IEnumerable<TournamentDetails>>());
			Assert.Equal(expectedTournaments, savedTournaments);
		}

		[Fact]
		public void UpdateTournaments_TournamentDoesNotExist_ExceptionIsThrown()
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

			Assert.Throws<Exception>(() => repositoryMock.Object.UpdateTournaments(updatedTournaments));
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

			var filteredTournaments = repositoryMock.Object.GetTournaments();

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
				repositoryMock.Object.GetTournaments(t => t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming);

			Assert.Equal(expectedTournaments, filteredTournaments);
		}

		private static Mock<TournamentRepository> GetTournamentRepositoryMock()
		{
			return new Mock<TournamentRepository>(string.Empty, new DateTime(2000, 1, 20)) { CallBase = true };
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
