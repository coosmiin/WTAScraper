using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraping.Driver;
using WTAScraping.Tournaments;
using WTAScraping.UrlFormatters;

namespace WTAScraping.Website
{
	public class WtaWebsite : IWtaWebsite
	{
		private readonly IWtaDriver _driver;
		private readonly IUrlFormatter _urlFormatter;

		public WtaWebsite(IWtaDriver driver, IUrlFormatter urlFormatter)
		{
			_driver = driver;
			_urlFormatter = urlFormatter;
		}

		public IEnumerable<TournamentDetails> GetTournamentsDetails()
		{
			IEnumerable<Tournament> tournaments = _driver.GetCurrentAndUpcomingTournaments();

			IList<TournamentDetails> tournamentsDetails = new List<TournamentDetails>();

			foreach (Tournament tournament in tournaments)
			{
				string firstSeed = null;

				IEnumerable<Player> players =
					_driver.GetTournamentPlayers(_urlFormatter.GetTournamentUrl(tournament.Name, tournament.Date.Year));

				if (!players.Any() && tournament.Status == TournamentStatus.Current)
				{
					tournament.Status = TournamentStatus.Invalid;
				}

				foreach (Player player in players)
				{
					if (player.Rank == 1)
					{
						// TODO: Create tests for a new IPlayerNameFormatter
						firstSeed = string.Join(" ", player.Name.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Reverse());
						break;
					}
				}

				yield return tournament.AsTournamentDetails(firstSeed);
			}
		}
	}
}
