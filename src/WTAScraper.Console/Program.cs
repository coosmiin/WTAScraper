using System;
using WTAData.Repositories;
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
			WtaDriverFactory wtaDriverFactory = null;
			ILogger logger = new ConsoleLogger(System.Console.Out); ;
			try
			{
				wtaDriverFactory = new WtaDriverFactory(AppContext.BaseDirectory);
				IWtaWebsite wtaWebsite =
					new WtaWebsite(wtaDriverFactory, new UrlFormatter(), new PlayerNameFormatter());

				ISecretStore secretStore = new LocalSecretStore<Program>();

				if (args.Length > 2 && args[2] == LOGGER_COMMAND_PARAMETER)
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

				IScrapeCommand scrapeCommand;

				if (args[0] == $"--{RefreshPlayersScrapeCommand.REFRESH_PLAYERS_COMMAND}")
				{
					scrapeCommand = new RefreshPlayersScrapeCommand(wtaWebsite, new PlayerRepository(args[1]), logger);

					scrapeCommand.RefreshData();

					return;
				}

				if (args[0] == $"--{RefreshTournamentsScrapeCommand.REFRESH_TOURNAMENTS_COMMAND}")
				{
					IRepositoryBuilder repositoryBuilder = new RepositoryBuilder();

					scrapeCommand =
						new RefreshTournamentsScrapeCommand(
							wtaWebsite, 
							repositoryBuilder.CreateTournamentRepository(
								secretStore.GetSecret(SecretStoreKeys.AWS_DB_ACCESS_KEY),
								secretStore.GetSecret(SecretStoreKeys.AWS_DB_SECRET_KEY),
								args[1]), 
							logger, DateTime.Now);
					scrapeCommand.RefreshData();

					return;
				}
			}
			catch (Exception ex)
			{
				if (logger == null)
					logger = new ConsoleLogger(System.Console.Out);

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
