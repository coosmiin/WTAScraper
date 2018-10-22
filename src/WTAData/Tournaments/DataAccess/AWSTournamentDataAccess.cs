using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WTAData.Tournaments.DataAccess
{
	public class AWSTournamentDataAccess : ITournamentDataAccess
	{
		private const string TOURNAMENTS_TABLE_NAME = "WTAScraper_Tournaments";
		private const string DATE_TIME_FORMAT = "yyyy-MM-dd";

		private readonly IAmazonDynamoDB _dynamoDb;
		private readonly DateTime _currentDate;

		public AWSTournamentDataAccess(IAmazonDynamoDB dynamoDb, DateTime currentDate)
		{
			_dynamoDb = dynamoDb;
			_currentDate = currentDate;
		}

		public IEnumerable<TournamentDetails> GetFreshTournamentsWithoutPlayers()
		{
			var request = new ScanRequest
			{
				TableName = TOURNAMENTS_TABLE_NAME,
				ExpressionAttributeNames = new Dictionary<string, string> { { "#Status", "Status" } },
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>
				{
					{ ":statusCurrent", new AttributeValue { S = TournamentStatus.Current.ToString() } },
					{ ":statusUpcomming", new AttributeValue { S = TournamentStatus.Upcomming.ToString() } },
				},
				FilterExpression = "(#Status = :statusCurrent OR #Status = :statusUpcomming) AND attribute_not_exists(SeededPlayers)"
			};

			ScanResponse response = _dynamoDb.ScanAsync(request).Result;

			foreach (var item in response.Items)
			{
				yield return
					new TournamentDetails(
						int.Parse(item["Id"].N),
						item["Name"].S,
						DateTime.ParseExact(item["StartDate"].S, DATE_TIME_FORMAT, CultureInfo.CurrentCulture),
						DateTime.ParseExact(item["EndDate"].S, DATE_TIME_FORMAT, CultureInfo.CurrentCulture),
						(TournamentStatus)Enum.Parse(typeof(TournamentStatus), item["Status"].S),
						null,
						int.Parse(item["Rounds"].N));
			}
		}

		public IEnumerable<TournamentDetails> GetOldUnfinishedTournaments()
		{
			var request = new ScanRequest
			{
				TableName = TOURNAMENTS_TABLE_NAME,
				ExpressionAttributeNames = new Dictionary<string, string>
				{
					{ "#Status", "Status" },
					{ "#StartDate", "StartDate" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>
				{
					{ ":statusFinished", new AttributeValue { S = TournamentStatus.Finished.ToString() } },
					{ ":startDate", new AttributeValue { S = _currentDate.AddDays(-1).ToString(DATE_TIME_FORMAT) } }
				},
				FilterExpression = "#Status <> :statusFinished AND #StartDate < :startDate"
			};

			ScanResponse response = _dynamoDb.ScanAsync(request).Result;

			foreach (var item in response.Items)
			{
				yield return
					new TournamentDetails(
						int.Parse(item["Id"].N),
						item["Name"].S,
						DateTime.ParseExact(item["StartDate"].S, DATE_TIME_FORMAT, CultureInfo.CurrentCulture),
						DateTime.ParseExact(item["EndDate"].S, DATE_TIME_FORMAT, CultureInfo.CurrentCulture),
						(TournamentStatus)Enum.Parse(typeof(TournamentStatus), item["Status"].S),
						null, 0);
			}
		}

		public virtual bool TryFindTournamentId(string name, DateTime startDate, out int tournamentId)
		{
			tournamentId = 0;

			var request = new ScanRequest
			{
				TableName = TOURNAMENTS_TABLE_NAME,
				ExpressionAttributeNames = new Dictionary<string, string>
				{
					{ "#Name", "Name" },
					{ "#StartDate", "StartDate" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>
				{
					{ ":name", new AttributeValue { S = name } },
					{ ":startDate", new AttributeValue { S = startDate.ToString(DATE_TIME_FORMAT) } }
				},
				FilterExpression = "#Name = :name AND #StartDate = :startDate"
			};

			ScanResponse response = _dynamoDb.ScanAsync(request).Result;

			if (response.Count == 0)
				return false;

			tournamentId = int.Parse(response.Items[0]["Id"].N);

			return true;
		}

		public virtual bool TryAddTournament(TournamentDetails tournament)
		{
			var request = new PutItemRequest
			{
				TableName = TOURNAMENTS_TABLE_NAME,
				Item = new Dictionary<string, AttributeValue>()
					{
						{ "Id", new AttributeValue { N = tournament.Id.ToString() } },
						{ "Name", new AttributeValue { S = tournament.Name } },
						{ "StartDate", new AttributeValue { S = tournament.StartDate.ToString(DATE_TIME_FORMAT) } },
						{ "EndDate", new AttributeValue { S = tournament.EndDate.ToString(DATE_TIME_FORMAT) } },
						{ "Rounds", new AttributeValue { N = tournament.Rounds == int.MinValue ? "0" : tournament.Rounds.ToString() } },
						{ "Status", new AttributeValue { S = tournament.Status.ToString() } }
					},
				ConditionExpression = "attribute_not_exists(Id)"
			};

			if (tournament.SeededPlayerNames?.Count() > 0)
			{
				request.Item.Add("SeededPlayers", new AttributeValue { SS = tournament.SeededPlayerNames.ToList() });
			}

			try
			{
				PutItemResponse response = _dynamoDb.PutItemAsync(request).Result;
			}
			catch (AggregateException ex)
			{
				if (ex?.InnerException is ConditionalCheckFailedException)
					return false;

				throw ex;
			}

			return true;
		}

		public virtual bool TryUpdateTournamentStatus(int tournamentId, DateTime startDate, TournamentStatus status)
		{
			var request = new UpdateItemRequest
			{
				TableName = TOURNAMENTS_TABLE_NAME,
				Key = new Dictionary<string, AttributeValue>
				{
					{ "Id",  new AttributeValue { N = tournamentId.ToString() } },
					{ "StartDate", new AttributeValue { S = startDate.ToString("yyyy-MM-dd") } }
				},
				ExpressionAttributeNames = new Dictionary<string, string> { { "#Status", "Status" } },
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
				{
					{ ":status", new AttributeValue { S = status.ToString() } }
				},
				ConditionExpression = "attribute_exists(Id)",
				UpdateExpression = "SET #Status = :status"
			};

			try
			{
				UpdateItemResponse response = _dynamoDb.UpdateItemAsync(request).Result;
			}
			catch (AggregateException ex)
			{
				if (ex?.InnerException is ConditionalCheckFailedException)
					return false;

				throw ex;
			}

			return true;
		}

		public void UpdateTournament(TournamentDetails tournament)
		{
			var request = new UpdateItemRequest
			{
				TableName = TOURNAMENTS_TABLE_NAME,
				Key = new Dictionary<string, AttributeValue>
				{
					{ "Id",  new AttributeValue { N = tournament.Id.ToString() } },
					{ "StartDate", new AttributeValue { S = tournament.StartDate.ToString("yyyy-MM-dd") } }
				},
				ExpressionAttributeNames = new Dictionary<string, string> { { "#Status", "Status" } },
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
				{
					{ ":status", new AttributeValue { S = tournament.Status.ToString() } },
					{ ":rounds", new AttributeValue { N = tournament.Rounds.ToString() } }
				},
				UpdateExpression = "SET #Status = :status, Rounds = :rounds"
			};

			if (tournament.SeededPlayerNames != null && tournament.SeededPlayerNames.Any())
			{
				request.ExpressionAttributeValues.Add(":players", new AttributeValue { SS = tournament.SeededPlayerNames.ToList() });
				request.UpdateExpression = "SET SeededPlayers = :players, #Status = :status, Rounds = :rounds";
			}

			UpdateItemResponse response = _dynamoDb.UpdateItemAsync(request).Result;
		}
	}
}
