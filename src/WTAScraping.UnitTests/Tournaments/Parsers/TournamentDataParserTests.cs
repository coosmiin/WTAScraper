﻿using System;
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
		public void ParseId_ValidUrl_IdIsParsed()
		{
			Assert.Equal(123, _parser.ParseId("http://www.wtatennis.com/tournament/2017-dalian-123"));
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
	}
}
