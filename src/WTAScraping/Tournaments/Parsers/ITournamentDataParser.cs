using System;

namespace WTAScraping.Tournaments.Parsers
{
	public interface ITournamentDataParser
	{
		DateTime ParseDate(string date);
		string ParseName(string tournamentUrl);
	}
}