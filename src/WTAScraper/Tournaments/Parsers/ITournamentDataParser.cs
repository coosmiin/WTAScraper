using System;

namespace WTAScraper.Tournaments.Parsers
{
	public interface ITournamentDataParser
	{
		int ParseId(string tournamentUrl);
		DateTime ParseDate(string date);
		string ParseName(string tournamentUrl);
	}
}