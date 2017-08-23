using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;

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

				var wta = new WTA(driver);

				string currentTournamentNameUrl = wta.GetCurrentTournamentNameUrl();

				IEnumerable<Player> players = wta.GetTournamentPlayers(currentTournamentNameUrl);
				foreach (Player player in players)
				{
					System.Console.Write("+");
				}

				System.Console.WriteLine();
				foreach (Player player in players.OrderBy(p => p.Rank))
				{
					System.Console.WriteLine(string.Format($"{player.Rank}\t\t{player.Name}"));
				}

				driver.Close();
			}
			catch (Exception ex)
			{
				if (driver != null)
					driver.Close();

				throw ex;
			}
		}
	}
}
