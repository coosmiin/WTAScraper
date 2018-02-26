using System;
using System.Linq;
using WTAData.Tournaments;
using WTAScraper.Driver;
using WTAScraper.Formatters;
using WTAScraper.Website;
using Xunit;

namespace WTAScraper.IntegrationTests.Website
{
	public class WtaWebsiteTests : IDisposable
	{
		IWtaWebsite _wtaWebsite;
		IWebDriverWrapper _webDriverWrapper;

		public WtaWebsiteTests()
		{
			_wtaWebsite = 
				new WtaWebsite(new WtaDriverFactory(AppContext.BaseDirectory), new UrlFormatter(), new PlayerNameFormatter());
			_webDriverWrapper = _wtaWebsite as IWebDriverWrapper;
		}

		[Fact]
		public void RefreshSeededPlayers()
		{
			var tournaments =
				_wtaWebsite.RefreshSeededPlayers(
					new TournamentDetails[]
					{
						new TournamentDetails(
							718, "Dubai United Arab Emirates", new DateTime(2018, 2, 12), new DateTime(0),
							TournamentStatus.Upcomming, new string[] { }, 0)
					}).ToList();
		}

		public void Dispose()
		{
			if (_webDriverWrapper != null)
				_webDriverWrapper.CloseDriver();
		}
	}
}
