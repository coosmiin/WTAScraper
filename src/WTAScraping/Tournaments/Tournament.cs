using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using WTAScraping.JsonConverters;

namespace WTAScraping.Tournaments
{
	public class Tournament
	{
		public string Name { get; }

		[JsonConverter(typeof(OnlyDateConverter))]
		public DateTime Date { get; }

		[JsonConverter(typeof(StringEnumConverter))]
		public TournamentStatus Status { get; set; }

		public Tournament(string name, DateTime date, TournamentStatus status)
		{
			Name = name;
			Date = date;
			Status = status;
		}
	}
}
