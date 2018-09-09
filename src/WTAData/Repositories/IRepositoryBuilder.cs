namespace WTAData.Repositories
{
	public interface IRepositoryBuilder
	{
		ITournamentRepository CreateTournamentRepository(string dbAccessKey, string dbSecretKey, string filePath);
	}
}
