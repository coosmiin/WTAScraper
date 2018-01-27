using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using WTAData.JsonConverters;

namespace WTAData.Tournaments
{
	public class Tournament
	{
		public int Id { get; }

		public string Name { get; }

		[JsonConverter(typeof(OnlyDateConverter))]
		public DateTime Date { get; }

		[JsonConverter(typeof(StringEnumConverter))]
		public TournamentStatus Status { get; set; }

		public Tournament(int id, string name, DateTime date, TournamentStatus status)
		{
			Id = id;
			Name = name;
			Date = date;
			Status = status;
		}
	}
}
