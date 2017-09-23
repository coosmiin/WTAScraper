using WTAScraping.Formatters;
using Xunit;

namespace WTAScraping.UnitTests.Formatters
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
