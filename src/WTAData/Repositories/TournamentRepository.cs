using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WTAData.Tournaments;
using WTAData.Tournaments.DataAccess;

namespace WTAData.Repositories
{
	public class TournamentRepository : ITournamentRepository
	{
		private readonly ITournamentDataAccess _dataAccess;
		private readonly string _filePath;
		private readonly DateTime _currentDate;

		public TournamentRepository(ITournamentDataAccess dataAccess, string filePath, DateTime currentDate)
		{
			_dataAccess = dataAccess;
			_filePath = filePath;
			_currentDate = currentDate;
		}

		public void CleanupFinishedTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			List<TournamentDetails> allTournaments = GetTournaments().ToList();

			IEnumerable<string> tournamentsNames = tournaments.Select(t => t.Name).ToList();

			allTournaments.ForEach(t =>
			{
				if (!tournamentsNames.Contains(t.Name) && t.StartDate.AddDays(1) < _currentDate)
				{
					t.Status = TournamentStatus.Finished;
				}
			});

			SaveTournaments(allTournaments.OrderByDescending(t => t.StartDate));
		}

		public void AddOrUpdateNewTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			foreach (TournamentDetails tournament in tournaments)
			{
				if (!_dataAccess.TryUpdateTournamentStatus(tournament))
				{
					_dataAccess.TryAddTournament(tournament);
				}
			}
		}

		public void AddOrUpdateNewTournaments_Deprecated(IEnumerable<TournamentDetails> tournaments)
		{
			List<TournamentDetails> allTournaments = GetTournaments().ToList();

			allTournaments = 
				allTournaments.Concat(tournaments)
					.ToLookup(t => t.Name)
					.Select(
						g => g.Aggregate(
							(t1, t2) => 
								new TournamentDetails(
									t1.Id, t1.Name, t2.StartDate, t2.EndDate, t2.Status, t1.SeededPlayerNames, t1.Rounds)))
					.ToList();

			SaveTournaments(allTournaments.OrderByDescending(t => t.StartDate));
		}

		public void UpdateTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			TournamentDetails[] allTournaments = GetTournaments().ToArray();

			foreach (var tournamentDetails in tournaments)
			{
				int index = Array.FindIndex(allTournaments, t => t.Name == tournamentDetails.Name);

				if (index < 0)
					throw new Exception($"Tournament '{tournamentDetails.Name}' was not found and therefore cannot be updated.");

				allTournaments[index] = tournamentDetails;
			}

			SaveTournaments(allTournaments.OrderByDescending(t => t.StartDate));
		}

		public IEnumerable<TournamentDetails> GetTournaments(Func<TournamentDetails, bool> predicate = null)
		{
			IEnumerable<TournamentDetails> tournaments = GetTournaments();

			if (predicate == null)
				return tournaments;

			return tournaments.Where(predicate);
		}

		protected virtual IEnumerable<TournamentDetails> GetTournaments()
		{
			if (!File.Exists(_filePath))
				return Enumerable.Empty<TournamentDetails>();

			return JsonConvert.DeserializeObject<IEnumerable<TournamentDetails>>(File.ReadAllText(_filePath));
		}

		protected virtual void SaveTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			File.WriteAllText(_filePath, JsonConvert.SerializeObject(tournaments));
		}
	}
}
