using System;
using System.Collections.Generic;
using System.Linq;

namespace WTAScraping.Tournaments
{
	public class TournamentDetails : Tournament
	{
		public IEnumerable<string> SeededPlayerNames { get; }

		public TournamentDetails(int id, string name, DateTime date, TournamentStatus status, IEnumerable<string> seededPlayerNames)
			: base(id, name, date, status)
		{
			SeededPlayerNames = seededPlayerNames;
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
				hash = hash * 397 + SeededPlayerNames?.GetHashCode() ?? 0;

				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			var o = obj as TournamentDetails;

			if (o == null)
				return false;

			return Name == o.Name && Date == o.Date && Status == o.Status
				&& (SeededPlayerNames ?? Enumerable.Empty<string>()).SequenceEqual(o.SeededPlayerNames ?? Enumerable.Empty<string>());
		}
	}
}
