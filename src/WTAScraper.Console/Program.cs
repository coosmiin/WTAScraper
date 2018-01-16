using System;
using WTAScraper.Data;
using WTAScraper.Driver;
using WTAScraper.Formatters;
using WTAScraper.Website;
using WTAScraper.Scraping;
using WTAScraper.Logging;

namespace WTAScraper.Console
{
	class Program
	{
		public const string APPLICATION_NAME = "WTA";

		static void Main(string[] args)
		{
			ILogger logger = new ConsoleLogger(System.Console.Out);
			WtaDriverFactory wtaDriverFactory = null;
			try
			{
				wtaDriverFactory = new WtaDriverFactory(AppContext.BaseDirectory);
				IWtaWebsite wtaWebsite =
					new WtaWebsite(wtaDriverFactory, new UrlFormatter(), new PlayerNameFormatter());

				if (args[2] == "-l")
				{
					switch (args[3])
					{
						case IftttLogger.LOGGER_NAME:
							logger = new IftttLogger(APPLICATION_NAME);
							break;
						case EmailLogger<Program>.LOGGER_NAME:
							logger = new EmailLogger<Program>(APPLICATION_NAME);
							break;
					}
				}

				if (args[0] == $"--{RefreshPlayersScrapeCommand.REFRESH_PLAYERS_COMMAND}")
				{
					IScrapeCommand scraper = new RefreshPlayersScrapeCommand(wtaWebsite, new PlayerRepository(args[1]), logger);

					scraper.RefreshData();

					return;
				}

				if (args[0] == $"--{RefreshTournamentsScrapeCommand.REFRESH_TOURNAMENTS_COMMAND}")
				{
					IScrapeCommand scraper =
						new RefreshTournamentsScrapeCommand(wtaWebsite, new TournamentRepository(args[1]), logger, DateTime.Now);
					scraper.RefreshData();

					return;
				}
			}
			catch (Exception ex)
			{
				logger.Log("Exception", ex.Message);
			}
			finally
			{
				if (wtaDriverFactory != null)
					wtaDriverFactory.CloseDriver();
			}
		}
	}
}
