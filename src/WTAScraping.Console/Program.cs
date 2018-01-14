using System;
using WTAScraping.Data;
using WTAScraping.Driver;
using WTAScraping.Formatters;
using WTAScraping.Website;
using WTAScraping.Scraping;
using WTAScraping.Logging;

namespace WTAScraping.Console
{
	class Program
	{
		public const string APPLICATION_NAME = "WTA";

		static void Main(string[] args)
		{
			try
			{
				IWtaWebsite wtaWebsite = 
					new WtaWebsite(new WtaDriverFactory(AppContext.BaseDirectory), new UrlFormatter(), new PlayerNameFormatter());

				ILogger logger = new ConsoleLogger(System.Console.Out);

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
						new RefreshTournamentsScrapeCommand(wtaWebsite, new TournamentRepository(args[1]), logger);
					scraper.RefreshData();

					return;
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
		}
	}
}
