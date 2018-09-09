using System;
using WTAScraper.Tournaments.Parsers;
using Xunit;

namespace WTAScraper.UnitTests.Tournaments.Parsers
{
	public class TournamentDataParserTests
	{
		TournamentDataParser _parser;

		public TournamentDataParserTests()
		{
			_parser = new TournamentDataParser();
		}

		[Fact]
		public void ParseDate_DateWithTimeZone_TimeZoneIsIgnored()
		{
			var expectedDate = new DateTime(2017, 8, 28);

			Assert.Equal(expectedDate, _parser.ParseDate("2017-08-28T00:00:01+00:00"));
		}

		[Fact]
		public void ParseId_ValidUrl_IdIsParsed()
		{
			Assert.Equal(123, _parser.ParseId("http://www.wtatennis.com/tournament/2017-dalian-123"));
		}

		[Fact]
		public void ParseName_ValidUrlWithoutId_NameIsParsed()
		{
			Assert.Equal("Wta Quebec City", _parser.ParseName("http://www.wtatennis.com/tournament/wta-quebec-city"));
		}

		[Fact]
		public void ParseId_ValidUrlWithoutId_LargeNegativeRandonIdIsReturned()
		{
			int id = _parser.ParseId("http://www.wtatennis.com/tournament/wta-quebec-city");
			Assert.True(id <= -1000000 && id >= -1000000000);
		}

		[Fact]
		public void ParseName_ValidUrl_NameIsParsed()
		{
			Assert.Equal("Dalian", _parser.ParseName("http://www.wtatennis.com/tournament/2017-dalian-123"));
		}

		[Fact]
		public void ParseName_TournamentWithTwoLetterAcronims_TheAcronimIsUppercase()
		{
			Assert.Equal("US Open", _parser.ParseName("http://www.wtatennis.com/tournament/2017-us-open-123"));
		}

		[Fact]
		public void ParseName_TournamentWithOnlyUpperLetters_NameIsParsedAsPascalCase()
		{
			Assert.Equal("Wuhan", _parser.ParseName("http://www.wtatennis.com/tournament/2017-WUHAN-123"));
		}

		[Fact]
		public void ParseName_UpperCaseUrlPart_NameIsParsedCorrectly()
		{
			Assert.Equal("Australian Open Australia", _parser.ParseName("http://www.wtatennis.com/TOURNAMENT/2018-AUSTRALIAN-OPEN-AUSTRALIA-901"));
		}

		[Fact]
		public void ParseName_UrlEncodedUrlPart_NameIsUrlDecoded()
		{
			Assert.Equal("Nürnberg Germany", _parser.ParseName("http://www.wtatennis.com/tournament/2018-n%C3%BCrnberg-germany-1068"));
		}
	}
}
