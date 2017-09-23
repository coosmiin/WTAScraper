using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WTAScraping.Data;
using WTAScraping.Driver;
using WTAScraping.Tournaments;
using WTAScraping.Tournaments.Parsers;
using WTAScraping.Formatters;
using WTAScraping.Website;

namespace WTAScraping.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			IWebDriver driver = null;
			try
			{
				var tournamentRepository = new TournamentRepository(args[0]);
				var playerRepository = new PlayerRepository(args[1]);

				driver = new ChromeDriver(AppContext.BaseDirectory);

				var wtaDriver = new WtaDriver(driver, new TournamentDataParser());
				var wtaWebsite = new WtaWebsite(wtaDriver, new UrlFormatter(), new PlayerNameFormatter());

				playerRepository.SavePlayers(wtaWebsite.GetPlayers());
				var players = playerRepository.GetPlayers();

				RefreshTournamentsData(wtaWebsite, tournamentRepository);

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
	}
}
