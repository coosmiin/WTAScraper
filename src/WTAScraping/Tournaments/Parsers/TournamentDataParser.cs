using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WTAScraping.Tournaments.Parsers
{
	public class TournamentDataParser : ITournamentDataParser
	{
		private readonly Regex _tournamentNameUrlRegex = new Regex(@"http:\/\/www\.wtatennis\.com\/tournament\/(.*)-(\d{1,})");

		public int ParseId(string tournamentUrl)
		{
			int id = int.Parse(_tournamentNameUrlRegex.Match(tournamentUrl).Groups[2].Value);

			return id;
		}

		public DateTime ParseDate(string date)
		{
			return DateTime.Parse(date.Substring(0, date.IndexOf("T")));
		}

		public string ParseName(string tournamentUrl)
		{
			string tournament = _tournamentNameUrlRegex.Match(tournamentUrl).Groups[1].Value;
			string[] urlParts = tournament.Split('-');

			string name =
				string.Join(
					" ",
					urlParts.Skip(1).Select(
						u => u.Length <= 2 ? u.ToUpper() : string.Format($"{u.Substring(0, 1).ToUpper()}{u.Substring(1).ToLower()}")));

			return name;
		}
	}
}
