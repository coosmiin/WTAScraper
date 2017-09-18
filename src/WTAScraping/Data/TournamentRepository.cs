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
			List<TournamentDetails> allTournaments = GetAllTournaments().ToList();

			IEnumerable<string> tournamentsNames = tournaments.Select(t => t.Name).ToList();

			allTournaments.ForEach(t =>
			{
				if (!tournamentsNames.Contains(t.Name))
				{
					t.Status = TournamentStatus.Finished;
				}
			});

			allTournaments.RemoveAll(t => tournamentsNames.Contains(t.Name));

			allTournaments.AddRange(tournaments);

			SaveTournaments(allTournaments);
		}

		protected virtual IEnumerable<TournamentDetails> GetAllTournaments()
		{
			return JsonConvert.DeserializeObject<IEnumerable<TournamentDetails>>(File.ReadAllText(_filePath));
		}

		protected virtual void SaveTournaments(IEnumerable<TournamentDetails> tournaments)
		{
			File.WriteAllText(_filePath, JsonConvert.SerializeObject(tournaments));
		}

	}
}
