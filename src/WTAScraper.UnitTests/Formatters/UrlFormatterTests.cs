using System;
using WTAScraper.Formatters;
using Xunit;

namespace WTAScraper.UnitTests.Formatters
{
	public class UrlFormatterTests
	{
		[Fact]
		public void GetTournamentUrl_UrlIsFormattedCorrectly()
		{
			var urlFormatter = new UrlFormatter();

			var url = urlFormatter.GetTournamentUrl(123, "Quebec City", new DateTime(2017, 10, 13));

			Assert.Equal("2017-quebec-city-123", url);
		}

		[Fact]
		public void GetTournamentUrl_TwoDaysBeforeEndOfYear_UrlContainsNextYear()
		{
			var urlFormatter = new UrlFormatter();

			var url = urlFormatter.GetTournamentUrl(123, "Quebec City", new DateTime(2017, 12, 30));

			Assert.Equal("2018-quebec-city-123", url);
		}
	}
}
