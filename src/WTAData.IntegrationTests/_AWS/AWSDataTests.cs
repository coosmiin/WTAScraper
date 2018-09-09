using System;
using System.Linq;
using WTAData.Repositories;
using WTAData.Tournaments;
using WTAData.Tournaments.DataAccess;
using Xunit;
using SecretStore;
using Amazon.DynamoDBv2;
using Amazon;

namespace WTAScraper.IntegrationTests._AWS
{
	public class AWSDataTests
	{
		ITournamentRepository _tournamentRepository;
		ITournamentDataAccess _tournamentDataAccess;

		public AWSDataTests()
		{
			var secretStore = new LocalSecretStore<AWSDataTests>();

			_tournamentRepository = new TournamentRepository(null, @"d:\Work\_Data\tournaments.json", DateTime.Now);
			_tournamentDataAccess =
				new AWSTournamentDataAccess(
					new AmazonDynamoDBClient(
						secretStore.GetSecret("AWSDbAccessKey"), secretStore.GetSecret("AWSDbSecretKey"),
						RegionEndpoint.EUCentral1),
					DateTime.Now);
		}

		[Fact]
		public void AddTournament()
		{
			var tournament = _tournamentRepository.GetTournaments_Deprecated(t => t.Id == 1090).Single();

			_tournamentDataAccess.TryAddTournament(tournament);
		}

		[Fact]
		public void UpdateTournamentStatus()
		{
			var tournament = _tournamentRepository.GetTournaments_Deprecated(t => t.Id == 822).Single();

			var newTournament =
				new TournamentDetails(
					tournament.Id, tournament.Name, tournament.StartDate, tournament.EndDate, 
					TournamentStatus.Invalid, tournament.SeededPlayerNames, tournament.Rounds);

			_tournamentDataAccess.TryUpdateTournamentStatus(newTournament.Id, newTournament.StartDate, newTournament.Status);
		}

		[Fact]
		public void UpdateTournament()
		{
			var tournament = _tournamentRepository.GetTournaments_Deprecated(t => t.Id == 1091).Single();

			_tournamentDataAccess.UpdateTournament(tournament.AsTournamentDetails(new[] { "AAA", "BBB" }, 25));
		}

		[Fact]
		public void GetFreshTournamentsWithoutPlayers()
		{
			var tournaments = _tournamentDataAccess.GetFreshTournamentsWithoutPlayers().ToList();
		}

		[Fact]
		public void GetOldUnfinishedTournaments()
		{
			var tournaments = _tournamentDataAccess.GetOldUnfinishedTournaments().ToList();
		}

		[Fact]
		public void MigrateAllTournamentsToAWS()
		{
			var tournaments =
				_tournamentRepository.GetTournaments_Deprecated(t => t.Id != 0 && t.Rounds != 0 && t.StartDate != new DateTime()).Reverse();

			foreach (TournamentDetails tournament in tournaments)
			{
				_tournamentDataAccess.TryAddTournament(tournament);
			}
		}
	}
}
