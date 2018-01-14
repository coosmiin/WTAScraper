using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraping.Data;
using WTAScraping.Logging;
using WTAScraping.Tournaments;
using WTAScraping.Website;

namespace WTAScraping.Scraping
{
	public class RefreshTournamentsScrapeCommand : IScrapeCommand
	{
		public const string REFRESH_TOURNAMENTS_COMMAND = "refresh-tournaments";

		private readonly IWtaWebsite _wtaWebsite;
		private readonly ITournamentRepository _tournamentRepository;
		private readonly ILogger _logger;

		public RefreshTournamentsScrapeCommand(IWtaWebsite wtaWebsite, ITournamentRepository tournamentRepository, ILogger logger)
		{
			_wtaWebsite = wtaWebsite;
			_tournamentRepository = tournamentRepository;
			_logger = logger;
		}

		public void RefreshData()
		{
			try
			{
				IEnumerable<Tournament> newTournaments = _wtaWebsite.GetCurrentAndUpcomingTournaments();

				_tournamentRepository.AddTournaments(newTournaments.AsTournamentDetails());

				_logger.Log(REFRESH_TOURNAMENTS_COMMAND, string.Join(", ", newTournaments.Select(t => t.Name)));

				IEnumerable<TournamentDetails> tournamentsDetails =
					_tournamentRepository
						.GetTournaments(
							t => (t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming)
								&& (t.SeededPlayerNames == null || !t.SeededPlayerNames.Any()));

				tournamentsDetails = _wtaWebsite.RefreshSeededPlayers(tournamentsDetails);

				_tournamentRepository.UpdateTournaments(tournamentsDetails);
			}
			catch (Exception ex)
			{
				_logger.Log(REFRESH_TOURNAMENTS_COMMAND, $"Error - {ex.Message}");
			}
		}
	}
}
