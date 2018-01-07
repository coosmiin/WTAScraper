using System;

namespace WTAScraping.Tournaments.Parsers
{
	public interface ITournamentDataParser
	{
		int ParseId(string tournamentUrl);
		DateTime ParseDate(string date);
		string ParseName(string tournamentUrl);
	}
}