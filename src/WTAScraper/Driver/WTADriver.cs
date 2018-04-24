using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAData.Players;
using WTAData.Tournaments;
using WTAScraper.Tournaments.Parsers;

namespace WTAScraper.Driver
{
	public class WtaDriver : IWtaDriver
	{
		private const string TOURNAMENTS_URL = "http://www.wtatennis.com/tournaments";

		private readonly IWebDriver _driver;
		private readonly ITournamentDataParser _tournamentDataParser;

		public WtaDriver(IWebDriver driver, ITournamentDataParser dataParser)
		{
			_driver = driver;
			_tournamentDataParser = dataParser;
		}

		public IEnumerable<Tournament> GetCurrentAndUpcomingTournaments()
		{
			var tournaments = new List<Tournament>();

			_driver.Navigate().GoToUrl(TOURNAMENTS_URL);

			IEnumerable<IWebElement> currentContainers =
				_driver.FindElements(By.CssSelector(".active-tournaments div[about]"));

			IEnumerable<IWebElement> upcomingContainers =
				_driver.FindElements(By.CssSelector(".view-display-id-page_1 div[about]"));

			tournaments.AddRange(currentContainers.Select(c => CreateTournament(c, TournamentStatus.Current)));
			tournaments.AddRange(upcomingContainers.Take(3).Select(c => CreateTournament(c, TournamentStatus.Upcomming)));

			return tournaments;
		}

		private Tournament CreateTournament(IWebElement container, TournamentStatus status)
		{
			IWebElement startDateElement = container.FindElement(By.CssSelector(".field--name-field-tournament-start-date span"));
			IWebElement endDateElement = container.FindElement(By.CssSelector(".field--name-field-tournament-end-date span"));

			string startDateAttribute = startDateElement.GetAttribute("content");
			string endDateAttribute = endDateElement.GetAttribute("content");

			DateTime startDate = _tournamentDataParser.ParseDate(startDateAttribute);
			DateTime endDate = _tournamentDataParser.ParseDate(endDateAttribute);

			string tournamentHrefAttribute =
				container.FindElement(By.CssSelector("h2.title-teaser.display-small a")).GetAttribute("href");

			string name = _tournamentDataParser.ParseName(tournamentHrefAttribute);

			int id = _tournamentDataParser.ParseId(tournamentHrefAttribute);

			return new Tournament(id, name, startDate, endDate, status);
		}

		public IEnumerable<SeededPlayer> GetTournamentPlayers(string tournamentNameUrl)
		{			
			_driver.Navigate().GoToUrl(string.Format($"http://www.wtatennis.com/tournament/{tournamentNameUrl}"));

			IWebElement drawsLink = 
				_driver.FindElements(By.CssSelector(".horizontal-tabs-list .horizontal-tab-button a[href='#draws']")).SingleOrDefault();

			if (drawsLink == null || !drawsLink.Displayed)
				return Enumerable.Empty<SeededPlayer>();

			drawsLink.Click();

			IEnumerable<IWebElement> elements = _driver.FindElements(By.CssSelector("#singles-draw-tab .srno"));

			var players = new List<SeededPlayer>();

			foreach (IWebElement element in elements)
			{
				IWebElement container = element.FindElement(By.XPath(".."));
				string seedText = container.FindElement(By.CssSelector(".additionalInfo")).Text.Replace("&nbsp;", string.Empty);

				IEnumerable<IWebElement> playerElements = container.FindElements(By.CssSelector(".player nobr"));

				string name = playerElements.Any() ? playerElements.First().Text.Replace("&nbsp;", string.Empty) : string.Empty;

				players.Add(new SeededPlayer(name, SeedRank(seedText)));
			}

			return players;
		}

		private int SeedRank(string seedText)
		{
			int index = seedText.IndexOf(' ');

			return index <= 0 ? SeededPlayer.MAX_SEED : int.Parse(seedText.Substring(0, index));
		}
	}
}