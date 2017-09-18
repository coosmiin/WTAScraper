using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace WTAScraping.Tournaments
{
	public class TournamentDetails : Tournament
	{
		public string FirstSeed { get; }

		public TournamentDetails(string name, DateTime date, TournamentStatus status, string firstSeed)
			: base(name, date, status)
		{
			FirstSeed = firstSeed;
		}

		public override int GetHashCode()
		{
			// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
			unchecked
			{
				int hash = 17;

				hash = hash * 397 + Name?.GetHashCode() ?? 0;
				hash = hash * 397 + Date.GetHashCode();
				hash = hash * 397 + Status.GetHashCode();
				hash = hash * 397 + FirstSeed?.GetHashCode() ?? 0;

				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			var o = (TournamentDetails)obj;

			return Name == o.Name && Date == o.Date && Status == o.Status && FirstSeed == o.FirstSeed;
		}
	}
}
