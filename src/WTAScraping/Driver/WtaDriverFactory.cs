using OpenQA.Selenium.Chrome;
using WTAScraping.Tournaments.Parsers;

namespace WTAScraping.Driver
{
	public class WtaDriverFactory : IWtaDriverFactory
	{
		private readonly string _driverDirectory;

		public WtaDriverFactory(string driverDirectory)
		{
			_driverDirectory = driverDirectory;
		}

		public IWtaDriver CreateDriver()
		{
			return new WtaDriver(new ChromeDriver(_driverDirectory), new TournamentDataParser());
		}
	}
}
