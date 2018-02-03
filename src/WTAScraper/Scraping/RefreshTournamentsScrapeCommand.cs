using System;
using System.Collections.Generic;
using System.Linq;
using WTAData.Repositories;
using Logging;
using WTAData.Tournaments;
using WTAScraper.Website;

namespace WTAScraper.Scraping
{
	public class RefreshTournamentsScrapeCommand : IScrapeCommand
	{
		public const string REFRESH_TOURNAMENTS_COMMAND = "refresh-tournaments";

		private readonly IWtaWebsite _wtaWebsite;
		private readonly ITournamentRepository _tournamentRepository;
		private readonly ILogger _logger;
		private readonly DateTime _currentDate;

		public RefreshTournamentsScrapeCommand(
			IWtaWebsite wtaWebsite, ITournamentRepository tournamentRepository, ILogger logger, DateTime currentDate)
		{
			_wtaWebsite = wtaWebsite;
			_tournamentRepository = tournamentRepository;
			_logger = logger;
			_currentDate = currentDate;
		}

		public void RefreshData()
		{
			try
			{
				IEnumerable<Tournament> newTournaments = _wtaWebsite.GetCurrentAndUpcomingTournaments();

				_tournamentRepository.AddTournaments(newTournaments.AsTournamentDetails());

				IEnumerable<TournamentDetails> tournamentsDetails =
					_tournamentRepository
						.GetTournaments(
							t => (t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming)
								&& (t.SeededPlayerNames == null || !t.SeededPlayerNames.Any()));

				tournamentsDetails = _wtaWebsite.RefreshSeededPlayers(tournamentsDetails);

				_tournamentRepository.UpdateTournaments(tournamentsDetails);

				string logMessage = BuildLogMessage();

				if (!string.IsNullOrEmpty(logMessage))
				{
					_logger.Log(REFRESH_TOURNAMENTS_COMMAND, logMessage);
				}
			}
			catch (Exception ex)
			{
				_logger.Log(REFRESH_TOURNAMENTS_COMMAND, $"Error - {ex.Message}");
			}
		}

		private string BuildLogMessage()
		{
			bool twoDaysBeforeOrAfter(TournamentDetails t) 
				=> t.Date < _currentDate.AddDays(2) && t.Date > _currentDate.AddDays(-2);

			bool currentUpcomingOrInvalid(TournamentDetails t)
				=> t.Status == TournamentStatus.Current 
					|| t.Status == TournamentStatus.Upcomming 
					|| t.Status == TournamentStatus.Invalid;

			IEnumerable<TournamentDetails> tournamentsDetails = 
			_tournamentRepository
				.GetTournaments(t => (currentUpcomingOrInvalid(t)) && twoDaysBeforeOrAfter(t));

			if (!tournamentsDetails.Any())
				return null;

			return
				string.Join(", ", 
					tournamentsDetails
						.OrderBy(t => t.Status)
						.Select(t => $"{t.Name} [{t.Status.ToString().Substring(0, 1)}{GetPlayerStatusAcronym(t.SeededPlayerNames)}P]"));
		}

		private string GetPlayerStatusAcronym(IEnumerable<string> players)
		{
			return players == null || !players.Any() ? "no" : "w";
		}
	}
}
