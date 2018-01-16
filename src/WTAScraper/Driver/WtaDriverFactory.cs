using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WTAScraper.Tournaments.Parsers;
using WTAScraper.Website;

namespace WTAScraper.Driver
{
	public class WtaDriverFactory : IWtaDriverFactory, IWebDriverWrapper
	{
		private readonly string _driverDirectory;

		private IWebDriver _webDriver;

		public WtaDriverFactory(string driverDirectory)
		{
			_driverDirectory = driverDirectory;
		}

		public IWtaDriver CreateDriver()
		{
			_webDriver = new ChromeDriver(_driverDirectory);

			return new WtaDriver(_webDriver, new TournamentDataParser());
		}

		public void CloseDriver()
		{
			if (_webDriver != null)
				_webDriver.Close();
		}
	}
}
