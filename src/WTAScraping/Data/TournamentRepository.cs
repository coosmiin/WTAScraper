using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WTAScraping.Tournaments;

namespace WTAScraping.Data
{
	public class TournamentRepository : ITournamentRepository
	{
		private readonly string _filePath;

		public TournamentRepository(string filePath)
		{
			_filePath = filePath;
		}

		public void AddTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			List<TournamentDetails> allTournaments = GetTournaments().ToList();

			IEnumerable<string> tournamentsNames = tournaments.Select(t => t.Name).ToList();

			allTournaments.ForEach(t =>
			{
				if (!tournamentsNames.Contains(t.Name))
				{
					t.Status = TournamentStatus.Finished;
				}
			});

			allTournaments = 
				allTournaments.Concat(tournaments)
					.ToLookup(t => t.Name)
					.Select(g => g.Aggregate((t1, t2) => new TournamentDetails(t1.Name, t2.Date, t2.Status, t1.SeededPlayerNames)))
					.ToList();

			SaveTournaments(allTournaments.OrderByDescending(t => t.Date));
		}

		public void UpdateTournaments(IEnumerable<TournamentDetails> tournamentsDetails)
		{
			TournamentDetails[] tournaments = GetTournaments().ToArray();

			foreach (var tournamentDetails in tournamentsDetails)
			{
				tournaments[Array.FindIndex(tournaments, t => t.Name == tournamentDetails.Name)] = tournamentDetails;
			}

			SaveTournaments(tournaments.OrderByDescending(t => t.Date));
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
			return JsonConvert.DeserializeObject<IEnumerable<TournamentDetails>>(File.ReadAllText(_filePath));
		}

		protected virtual void SaveTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			File.WriteAllText(_filePath, JsonConvert.SerializeObject(tournaments));
		}

	}
}
