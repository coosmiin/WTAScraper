using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WTAScraping.Data;
using WTAScraping.Driver;
using WTAScraping.Tournaments;
using WTAScraping.Formatters;
using WTAScraping.Website;
using WTAScraping.Scraping;
using WTAScraping.Logging;

namespace WTAScraping.Console
{
	class Program
	{
		public const string APPLICATION_NAME = "WTA";

		static IWtaWebsite _wtaWebsite;

		static void Main(string[] args)
		{
			IWebDriver driver = null;
			// TODO: do we still need the try/catch?
			try
			{
				// var tournamentRepository = new TournamentRepository(args[1]);
				// driver = new ChromeDriver(AppContext.BaseDirectory);

				// _wtaWebsite = CreateWtaWebsite();

				if (args[0] == $"--{Scraper.REFRESH_PLAYERS_COMMAND}")
				{
					IScraper scraper =
						new Scraper(CreateWtaWebsite(), new PlayerRepository(args[1]), new IftttLogger(APPLICATION_NAME));
					scraper.RefreshPlayers();

					return;
				}

				// RefreshTournamentsData(_wtaWebsite, tournamentRepository);

				// var players = playerRepository.GetPlayers();
				// PrintPlayers(driver);
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
			finally
			{
				if (driver != null)
					driver.Close();
			}
		}

		private static void RefreshTournamentsData(IWtaWebsite wtaWebsite, ITournamentRepository tournamentRepository)
		{
			IEnumerable<Tournament> newTournaments = wtaWebsite.GetCurrentAndUpcomingTournaments();

			tournamentRepository.AddTournaments(newTournaments.AsTournamentDetails());

			IEnumerable<TournamentDetails> tournamentsDetails =
				tournamentRepository
					.GetTournaments(
						t => (t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming)  
							&& (t.SeededPlayerNames == null || !t.SeededPlayerNames.Any()));

			tournamentsDetails = wtaWebsite.RefreshSeededPlayers(tournamentsDetails);

			tournamentRepository.UpdateTournaments(tournamentsDetails);

			//SendEmail("This is a test!");
		}

		private static void SendEmail(string subject)
		{
			IConfigurationRoot configuration = BuildConfiguration();

			var smtp = new SmtpClient("smtp.live.com", 587);

			smtp.Credentials = new NetworkCredential(configuration["OutlookUsername"], configuration["OutlookPassword"]);
			smtp.EnableSsl = true;

			smtp.Send(configuration["OutlookSenderAddress"], configuration["OutlookToAddress"], subject, string.Empty);
		}

		private static IConfigurationRoot BuildConfiguration()
		{
			IConfigurationBuilder builder = new ConfigurationBuilder();

			builder.AddUserSecrets<Program>();

			IConfigurationRoot configuration = builder.Build();

			return configuration;
		}

		private static IWtaWebsite CreateWtaWebsite()
		{
			return 
				new WtaWebsite(
					new WtaDriverFactory(AppContext.BaseDirectory), new UrlFormatter(), new PlayerNameFormatter());
		}
	}
}
