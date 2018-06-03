using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WTAData.Tournaments.DataAccess
{
	public class AWSTournamentDataAccess : ITournamentDataAccess
	{
		private readonly IAmazonDynamoDB _dynamoDb;

		public AWSTournamentDataAccess(string awsAccessKey, string awsSecretKey)
		{
			_dynamoDb = new AmazonDynamoDBClient(new BasicAWSCredentials(awsAccessKey, awsSecretKey), RegionEndpoint.EUCentral1);
		}

		public bool TryAddTournament(TournamentDetails tournament)
		{
			var request = new PutItemRequest
			{
				TableName = "WTAScraper_Tournaments",
				Item = new Dictionary<string, AttributeValue>()
					{
						{ "Id", new AttributeValue { N = tournament.Id.ToString() } },
						{ "Name", new AttributeValue { S = tournament.Name } },
						{ "StartDate", new AttributeValue { S = tournament.StartDate.ToString("yyyy-MM-dd") } },
						{ "EndDate", new AttributeValue { S = tournament.EndDate.ToString("yyyy-MM-dd") } },
						{ "Rounds", new AttributeValue { N = tournament.Rounds == int.MinValue ? "0" : tournament.Rounds.ToString() } },
						{ "Status", new AttributeValue { S = tournament.Status.ToString() } }
					},
				ConditionExpression = "attribute_not_exists(Id)"
			};

			if (tournament.SeededPlayerNames.Count() > 0)
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

		public bool TryUpdateTournamentStatus(TournamentDetails tournament)
		{
			var request = new UpdateItemRequest
			{
				TableName = "WTAScraper_Tournaments",
				Key = new Dictionary<string, AttributeValue>
				{
					{ "Id",  new AttributeValue { N = tournament.Id.ToString() } },
					{ "StartDate", new AttributeValue { S = tournament.StartDate.ToString("yyyy-MM-dd") } }
				},
				ExpressionAttributeNames = new Dictionary<string, string> { { "#Status", "Status" } },
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
				{
					{ ":status", new AttributeValue { S = tournament.Status.ToString() } }
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
	}
}
