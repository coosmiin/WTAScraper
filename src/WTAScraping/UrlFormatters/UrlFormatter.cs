namespace WTAScraping.UrlFormatters
{
	public class UrlFormatter : IUrlFormatter
	{
		public string GetTournamentUrl(string name, int year)
		{
			name = name.Replace(" ", "-").ToLower();

			return string.Format($"{year}-{name}");
		}
	}
}
