using WTAScraper.Formatters;
using Xunit;

namespace WTAScraper.UnitTests.Formatters
{
	public class PlayerNameFormatterTests
	{
		[Fact]
		public void GetPlayerName_NameIsFormattedCorrectly()
		{
			var formatter = new PlayerNameFormatter();

			string name = formatter.GetPlayerName("LN, FN");

			Assert.Equal("FN LN", name);
		}

		[Fact]
		public void GetPlayerName_TwoWordsLastname_NameIsFormattedCorrectly()
		{
			var formatter = new PlayerNameFormatter();

			string name = formatter.GetPlayerName("Van Uytvanck, Alison");

			Assert.Equal("Alison Van Uytvanck", name);
		}
	}
}
