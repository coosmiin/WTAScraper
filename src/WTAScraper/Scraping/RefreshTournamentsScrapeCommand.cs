﻿using System;
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
				_tournamentRepository.CleanupFinishedTournaments();

				IEnumerable<TournamentDetails> newTournaments = _wtaWebsite.GetCurrentAndUpcomingTournaments().AsTournamentDetails();

				_tournamentRepository.CleanupFinishedTournaments_Deprecated(newTournaments);
				_tournamentRepository.AddOrUpdateNewTournaments(newTournaments);
				_tournamentRepository.AddOrUpdateNewTournaments_Deprecated(newTournaments);

				IEnumerable<TournamentDetails> tournamentsDetails = 
					_tournamentRepository.GetFreshTournamentsWithoutPlayers().ToList();
					//_tournamentRepository
					//	.GetTournaments_Deprecated(
					//		t => (t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming)
					//			&& (t.SeededPlayerNames == null || !t.SeededPlayerNames.Any()))
					//	.ToList();

				tournamentsDetails = _wtaWebsite.RefreshSeededPlayers(tournamentsDetails).ToList();

				_tournamentRepository.UpdateTournaments(tournamentsDetails);
				_tournamentRepository.UpdateTournaments_Deprecated(tournamentsDetails);

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
			bool inDaysInterval(TournamentDetails t) 
				=> t.StartDate < _currentDate.AddDays(2) && t.StartDate > _currentDate.AddDays(-1);

			bool currentUpcomingOrInvalid(TournamentDetails t)
				=> t.Status == TournamentStatus.Current 
					|| t.Status == TournamentStatus.Upcomming 
					|| t.Status == TournamentStatus.Invalid;

			IEnumerable<TournamentDetails> tournamentsDetails = 
			_tournamentRepository
				.GetTournaments_Deprecated(t => (currentUpcomingOrInvalid(t)) && inDaysInterval(t));

			if (!tournamentsDetails.Any())
				return null;

			return
				string.Join(", ", 
					tournamentsDetails
						.OrderBy(t => t.Status)
						.Select(t => $"{t.StartDate.ToString("dd-MM")}: {t.Name} [{t.Status.ToString().Substring(0, 1)}{GetPlayerStatusAcronym(t.SeededPlayerNames)}P]"));
		}

		private string GetPlayerStatusAcronym(IEnumerable<string> players)
		{
			return players == null || !players.Any() ? "no" : "w";
		}
	}
}
