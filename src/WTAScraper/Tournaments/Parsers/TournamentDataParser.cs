using System;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

namespace WTAScraper.Tournaments.Parsers
{
	public class TournamentDataParser : ITournamentDataParser
	{
		private const int NAME_REGEX_GROUP_INDEX = 1;
		private const int ID_REGEX_GROUP_INDEX = 2;

		private readonly Regex _tournamentNameAndIdUrlRegex =
			new Regex(@"http:\/\/www\.wtatennis\.com\/tournament\/(.*)-(\d{1,})", RegexOptions.IgnoreCase);

		private readonly Regex _tournamentNameOnlyUrlRegex =
			new Regex(@"http:\/\/www\.wtatennis\.com\/tournament\/(.*)", RegexOptions.IgnoreCase);

		public int ParseId(string tournamentUrl)
		{
			if (TryGetRegexCaptureGroup(ID_REGEX_GROUP_INDEX, tournamentUrl, out string idText))
			{
				return int.Parse(idText);
			}

			return -(new Random()).Next(1000000, 1000000000);
		}

		public DateTime ParseDate(string date)
		{
			return DateTime.Parse(date.Substring(0, date.IndexOf("T")));
		}

		public string ParseName(string tournamentUrl)
		{
			if (!TryGetRegexCaptureGroup(NAME_REGEX_GROUP_INDEX, tournamentUrl, out string tournament))
				return string.Empty;

			string[] urlParts = tournament.Split('-');

			int skipCount = int.TryParse(urlParts[0], out int dummy) ? 1 : 0;

			string name =
				string.Join(
					" ",
					urlParts.Skip(skipCount).Select(
						u => u.Length <= 2 ? u.ToUpper() : string.Format($"{u.Substring(0, 1).ToUpper()}{u.Substring(1).ToLower()}")));

			return WebUtility.UrlDecode(name);
		}

		private bool TryGetRegexCaptureGroup(int groupIndex, string tournamentUrl, out string captureGroup)
		{
			captureGroup = string.Empty;
			GroupCollection groups = null;

			if (_tournamentNameAndIdUrlRegex.IsMatch(tournamentUrl))
			{
				groups = _tournamentNameAndIdUrlRegex.Match(tournamentUrl).Groups;
			}
			else if (_tournamentNameOnlyUrlRegex.IsMatch(tournamentUrl))
			{
				groups = _tournamentNameOnlyUrlRegex.Match(tournamentUrl).Groups;
			}

			if (groupIndex < groups?.Count)
			{
				captureGroup = groups[groupIndex].Value;
				return true;
			}

			return false;
		}
	}
}
