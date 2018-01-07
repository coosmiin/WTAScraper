using System;

namespace WTAScraping.Formatters
{
	public class UrlFormatter : IUrlFormatter
	{
		public string GetTournamentUrl(int id, string name, DateTime date)
		{
			int year = (new DateTime(date.Year + 1, 1, 1) - date).TotalDays > 2 ? date.Year : date.Year + 1;

			name = name.Replace(" ", "-").ToLower();

			return string.Format($"{year}-{name}-{id}");
		}
	}
}
