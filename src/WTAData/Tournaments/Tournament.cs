using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using WTAData.JsonConverters;

namespace WTAData.Tournaments
{
	public class Tournament
	{
		public int Id { get; set; }

		public string Name { get; }

		[JsonConverter(typeof(OnlyDateConverter))]
		public DateTime StartDate { get; }

		[JsonConverter(typeof(OnlyDateConverter))]
		public DateTime EndDate { get; }

		[JsonConverter(typeof(StringEnumConverter))]
		public TournamentStatus Status { get; set; }

		public Tournament(int id, string name, DateTime startDate, DateTime endDate, TournamentStatus status)
		{
			Id = id;
			Name = name;
			StartDate = startDate;
			EndDate = endDate;
			Status = status;
		}
	}
}
