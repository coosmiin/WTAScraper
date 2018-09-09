using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using WTAData.Tournaments.DataAccess;

namespace WTAData.Repositories
{
	public class RepositoryBuilder : IRepositoryBuilder
	{
		public ITournamentRepository CreateTournamentRepository(string dbAccessKey, string dbSecretKey, string filePath)
		{
			DateTime currentDateTime = DateTime.Now;

			return
				new TournamentRepository(
					new AWSTournamentDataAccess(
						new AmazonDynamoDBClient(
							new BasicAWSCredentials(dbAccessKey, dbSecretKey), RegionEndpoint.EUCentral1),
						currentDateTime),
					filePath, currentDateTime);
		}
	}
}
