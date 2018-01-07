using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraping.Data;
using WTAScraping.Logging;
using WTAScraping.Players;
using WTAScraping.Website;

namespace WTAScraping.Scraping
{
	public class Scraper : IScraper
	{
		public const string REFRESH_PLAYERS_COMMAND = "refresh-players";

		private readonly IWtaWebsite _wtaWebsite;
		private readonly IPlayerRepository _playerRepository;
		private readonly ILogger _logger;

		public Scraper(IWtaWebsite wtaWebsite, IPlayerRepository playerRepository, ILogger logger)
		{
			_wtaWebsite = wtaWebsite;
			_playerRepository = playerRepository;
			_logger = logger;
		}

		public void RefreshPlayers()
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
