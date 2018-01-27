using System;
using WTAScraper.Data;
using WTAScraper.Driver;
using WTAScraper.Formatters;
using WTAScraper.Website;
using WTAScraper.Scraping;
using Logging;
using WTAScraper.Console.Configuration;
using SecretStore;

namespace WTAScraper.Console
{
	class Program
	{
		public const string APPLICATION_NAME = "WTA";
		public const string LOGGER_COMMAND_PARAMETER = "-l";

		public const string SMTP_HOST = "smtp.live.com";
		public const int SMTP_PORT = 587;

		static void Main(string[] args)
		{
			ILogger logger = new ConsoleLogger(System.Console.Out);
			WtaDriverFactory wtaDriverFactory = null;
			try
			{
				wtaDriverFactory = new WtaDriverFactory(AppContext.BaseDirectory);
				IWtaWebsite wtaWebsite =
					new WtaWebsite(wtaDriverFactory, new UrlFormatter(), new PlayerNameFormatter());

				ISecretStore secretStore = new LocalSecretStore<Program>();

				if (args[2] == LOGGER_COMMAND_PARAMETER)
				{
					switch (args[3])
					{
						case IftttLogger.LOGGER_NAME:
							logger = new IftttLogger(APPLICATION_NAME, secretStore.GetSecret(SecretStoreKeys.IFTTT_KEY));
							break;

						case EmailLogger<Program>.LOGGER_NAME:
							var emailSettings =
								new EmailSettings(
									SMTP_HOST, SMTP_PORT,
									secretStore.GetSecret(SecretStoreKeys.OUTLOOK_USERNAME_KEY), 
									secretStore.GetSecret(SecretStoreKeys.OUTLOOK_PASSWORD_KEY), 
									secretStore.GetSecret(SecretStoreKeys.OUTLOOK_SENDER_ADDRESS_KEY), 
									secretStore.GetSecret(SecretStoreKeys.OUTLOOK_TO_ADDRESS_KEY));
									
							logger = new EmailLogger<Program>(APPLICATION_NAME, emailSettings);
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
						new RefreshTournamentsScrapeCommand(
							wtaWebsite, new TournamentRepository(args[1], DateTime.Now), logger, DateTime.Now);
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
