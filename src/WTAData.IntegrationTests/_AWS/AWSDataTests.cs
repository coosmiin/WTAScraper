using System;
using System.Linq;
using WTAData.Repositories;
using WTAData.Tournaments;
using WTAData.Tournaments.DataAccess;
using Xunit;
using SecretStore;

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
					secretStore.GetSecret("AWSDbAccessKey"), secretStore.GetSecret("AWSDbSecretKey"));
		}

		[Fact]
		public void AddItem()
		{
			var tournament = _tournamentRepository.GetTournaments(t => t.Id == 822).Single();

			_tournamentDataAccess.TryAddTournament(tournament);
		}

		[Fact]
		public void UpdateItem()
		{
			var tournament = _tournamentRepository.GetTournaments(t => t.Id == 822).Single();

			var newTournament =
				new TournamentDetails(
					tournament.Id, tournament.Name, tournament.StartDate, tournament.EndDate, 
					TournamentStatus.Invalid, tournament.SeededPlayerNames, tournament.Rounds);

			_tournamentDataAccess.TryUpdateTournamentStatus(newTournament);
		}

		[Fact]
		public void MigrateAllTournamentsToAWS()
		{
			var tournaments =
				_tournamentRepository.GetTournaments(t => t.Id != 0 && t.Rounds != 0 && t.StartDate != new DateTime()).Reverse();

			foreach (TournamentDetails tournament in tournaments)
			{
				_tournamentDataAccess.TryAddTournament(tournament);
			}
		}
	}
}
