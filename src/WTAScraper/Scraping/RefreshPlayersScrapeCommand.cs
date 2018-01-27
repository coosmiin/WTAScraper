using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using WTAScraper.Data;
using WTAScraper.Players;
using WTAScraper.Website;

namespace WTAScraper.Scraping
{
	public class RefreshPlayersScrapeCommand : IScrapeCommand
	{
		public const string REFRESH_PLAYERS_COMMAND = "refresh-players";

		private readonly IWtaWebsite _wtaWebsite;
		private readonly IPlayerRepository _playerRepository;
		private readonly ILogger _logger;

		public RefreshPlayersScrapeCommand(IWtaWebsite wtaWebsite, IPlayerRepository playerRepository, ILogger logger)
		{
			_wtaWebsite = wtaWebsite;
			_playerRepository = playerRepository;
			_logger = logger;
		}

		public void RefreshData()
		{
			try
			{
				IEnumerable<Player> players = _wtaWebsite.GetPlayers();

				_playerRepository.SavePlayers(players);

				_logger.Log(REFRESH_PLAYERS_COMMAND, $"1st Rank: {players.First().Name}");
			}
			catch (Exception ex)
			{
				_logger.Log(REFRESH_PLAYERS_COMMAND, $"Error - {ex.Message}");
			}
		}
	}
}
