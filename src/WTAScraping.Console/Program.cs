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
using WTAScraping.UrlFormatters;
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
				driver = new ChromeDriver(AppContext.BaseDirectory);

				var wtaDriver = new WtaDriver(driver, new TournamentDataParser());
				var wtaWebsite = new WtaWebsite(wtaDriver, new UrlFormatter());
				var tournamentRepository = new TournamentRepository(args[0]);

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
			IEnumerable<TournamentDetails> newTournaments = wtaWebsite.GetTournamentsDetails().ToList();

			tournamentRepository.AddTournaments(newTournaments);

			//SendEmail("This is a test!");
		}

		private static void SendEmail(string subject)
		{
			IConfigurationRoot configuration = BuildConfiguration();

			var smtp = new SmtpClient("smtp.live.com", 587);

			smtp.Credentials = new NetworkCredential(configuration["OutlookUsername"], configuration["OutlookPassword"]);
			smtp.EnableSsl = true;

			smtp.Send("outlook_572ED13E695370D0@outlook.com", "coosmiin@yahoo.com", subject, string.Empty);
		}

		private static void PrintPlayers(IWebDriver driver)
		{
			var wta = new WtaDriver(driver, new TournamentDataParser());

			IEnumerable<string> tournamentNameUrls = wta.GetCurrentTournamentNameUrls();

			foreach (var tournamentNameUrl in tournamentNameUrls)
			{
				IEnumerable<Player> yieldPlayers = wta.GetTournamentPlayers(tournamentNameUrl);
				var players = new List<Player>();
				foreach (Player player in yieldPlayers)
				{
					players.Add(player);
					System.Console.Write("+");
				}

				System.Console.WriteLine();
				foreach (Player player in players.OrderBy(p => p.Rank))
				{
					System.Console.WriteLine(string.Format($"{player.Rank}\t\t{player.Name}"));
				}

				System.Console.WriteLine();
				System.Console.WriteLine("------------");
				System.Console.WriteLine();
			}
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
