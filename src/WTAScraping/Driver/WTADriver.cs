using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraping.Tournaments;
using WTAScraping.Tournaments.Parsers;

namespace WTAScraping.Driver
{
	public class WtaDriver : IWtaDriver
	{
		private const string TOURNAMENTS_URL = "http://www.wtatennis.com/tournaments";

		private readonly IWebDriver _driver;
		private readonly ITournamentDataParser _dataParser;

		public WtaDriver(IWebDriver driver, ITournamentDataParser dataParser)
		{
			_driver = driver;
			_dataParser = dataParser;
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
			IWebElement dateElement = container.FindElement(By.CssSelector(".group-header .date-display-single"));
			string dateAttribute = dateElement.GetAttribute("content");

			DateTime date = _dataParser.ParseDate(dateAttribute);

			string tournamentHrefAttribute =
				container.FindElement(By.CssSelector(".field--name-draw-tournament-button a")).GetAttribute("href");

			string name = _dataParser.ParseName(tournamentHrefAttribute);

			return new Tournament(name, date, status);
		}

		public IEnumerable<string> GetCurrentTournamentNameUrls()
		{
			var urls = new List<string>();

			_driver.Navigate().GoToUrl(TOURNAMENTS_URL);

			IEnumerable<IWebElement> currentTournamentDrawAnchors = 
				_driver.FindElements(By.CssSelector(".active-tournaments .field--name-draw-tournament-button a"));

			if (!currentTournamentDrawAnchors.Any())
				return urls;

			foreach (var anchor in currentTournamentDrawAnchors)
			{
				
			}

			return urls;
		}

		public IEnumerable<Player> GetTournamentPlayers(string tournamentNameUrl)
		{			
			_driver.Navigate().GoToUrl(string.Format($"http://www.wtatennis.com/tournament/{tournamentNameUrl}#draws"));

			IEnumerable<IWebElement> elements = _driver.FindElements(By.CssSelector("#singles-draw-tab .srno"));

			foreach (IWebElement element in elements)
			{
				IWebElement container = element.FindElement(By.XPath(".."));
				string rankText = container.FindElement(By.CssSelector(".additionalInfo")).Text.Replace("&nbsp;", string.Empty);

				IEnumerable<IWebElement> playerElements = container.FindElements(By.CssSelector(".player nobr"));

				if (!playerElements.Any())
					continue;

				string name = playerElements.First().Text.Replace("&nbsp;", string.Empty);

				yield return new Player(name, GetRank(rankText));
			}
		}

		private static Tournament CreateTournament(DateTime date, string tournamentUrl, TournamentStatus status)
		{
			string[] urlParts = tournamentUrl.Split('-');

			string name =
				string.Join(
					" ",
					urlParts.Skip(1).Select(
						u => u.Length <= 2 ? u.ToUpper() : string.Format($"{u.Substring(0, 1).ToUpper()}{u.Substring(1)}")));

			return new Tournament(name, date, status);
		}

		private int GetRank(string rankText)
		{
			int index = rankText.IndexOf(' ');

			return index <= 0 ? 99999 : int.Parse(rankText.Substring(0, index));
		}
	}
}