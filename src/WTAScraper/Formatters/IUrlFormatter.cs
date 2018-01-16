using System;

namespace WTAScraper.Formatters
{
	public interface IUrlFormatter
	{
		string GetTournamentUrl(int id, string name, DateTime date);
	}
}