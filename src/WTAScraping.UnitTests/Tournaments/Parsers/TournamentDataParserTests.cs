using System;
using WTAScraping.Tournaments.Parsers;
using Xunit;

namespace WTAScraping.UnitTests.Tournaments.Parsers
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
		public void ParseName_ValidUrl_NameIsParsed()
		{
			Assert.Equal("Dalian", _parser.ParseName("http://www.wtatennis.com/tournament/2017-dalian#draws"));
		}

		[Fact]
		public void ParseName_TournamentWithTwoLetterAcronims_TheAcronimIsUppercase()
		{
			Assert.Equal("US Open", _parser.ParseName("http://www.wtatennis.com/tournament/2017-us-open#draws"));
		}
	}
}
