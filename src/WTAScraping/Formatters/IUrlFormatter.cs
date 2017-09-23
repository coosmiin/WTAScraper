namespace WTAScraping.Formatters
{
	public interface IUrlFormatter
	{
		string GetTournamentUrl(string name, int year);
	}
}