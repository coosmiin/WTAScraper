using System;
using System.Collections.Generic;
using System.Linq;

namespace WTAData.Tournaments
{
	public class TournamentDetails : Tournament
	{
		public IEnumerable<string> SeededPlayerNames { get; }
		public int Rounds { get; }

		public TournamentDetails(
			int id, string name, DateTime startDate, DateTime endDate, TournamentStatus status, 
			IEnumerable<string> seededPlayerNames, int rounds)
			: base(id, name, startDate, endDate, status)
		{
			SeededPlayerNames = seededPlayerNames;
			Rounds = rounds;
		}

		public override int GetHashCode()
		{
			// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
			unchecked
			{
				int hash = 17;

				hash = hash * 397 + Name?.GetHashCode() ?? 0;
				hash = hash * 397 + StartDate.GetHashCode();
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

			return Name == o.Name && StartDate == o.StartDate && EndDate == o.EndDate && Status == o.Status
				&& (SeededPlayerNames ?? Enumerable.Empty<string>()).SequenceEqual(o.SeededPlayerNames ?? Enumerable.Empty<string>());
		}
	}
}
