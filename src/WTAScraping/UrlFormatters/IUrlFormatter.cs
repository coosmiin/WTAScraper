namespace WTAScraping.UrlFormatters
{
	public interface IUrlFormatter
	{
		string GetTournamentUrl(string name, int year);
	}
}