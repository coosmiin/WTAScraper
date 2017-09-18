using WTAScraping.UrlFormatters;
using Xunit;

namespace WTAScraping.UnitTests.UrlFormatters
{
	public class UrlFormatterTests
	{
		[Fact]
		public void GetTournamentUrl_UrlIsFormattedCorrectly()
		{
			var urlFormatter = new UrlFormatter();

			var url = urlFormatter.GetTournamentUrl("Quebec City", 2017);

			Assert.Equal("2017-quebec-city", url);
		}
	}
}
