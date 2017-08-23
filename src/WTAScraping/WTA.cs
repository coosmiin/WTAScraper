using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WTAScraping
{
	public class WTA
	{
		private readonly IWebDriver _driver;
		private readonly Regex _tournamentNameUrlRegex = new Regex(@"http:\/\/www\.wtatennis\.com\/tournament\/(.*)#draws");

		public WTA(IWebDriver driver)
		{
			_driver = driver;
		}

		public string GetCurrentTournamentNameUrl()
		{
			_driver.Navigate().GoToUrl("http://www.wtatennis.com/tournaments");

			IEnumerable<IWebElement> currentTournamentDrawAnchors = 
				_driver.FindElements(By.CssSelector(".field--name-draw-tournament-button a"));

			if (!currentTournamentDrawAnchors.Any())
				return string.Empty;

			return _tournamentNameUrlRegex.Match(currentTournamentDrawAnchors.First().GetAttribute("href")).Groups[1].Value;
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

		private int GetRank(string rankText)
		{
			int index = rankText.IndexOf(' ');

			return index <= 0 ? 99999 : int.Parse(rankText.Substring(0, index));
		}
	}
}