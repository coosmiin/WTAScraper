using System;

namespace WTAScraping.Formatters
{
	public interface IUrlFormatter
	{
		string GetTournamentUrl(int id, string name, DateTime date);
	}
}