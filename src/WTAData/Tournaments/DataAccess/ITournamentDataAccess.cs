namespace WTAData.Tournaments.DataAccess
{
	public interface ITournamentDataAccess
	{
		/// <summary>
		/// Tries to add a new tournament. It does not update and returns 'false' if the tournament exists.
		/// </summary>
		bool TryAddTournament(TournamentDetails tournament);

		/// <summary>
		/// Tries to update tournament status. It does not add new and returns 'false' if the tournament does not exist.
		/// </summary>
		bool TryUpdateTournamentStatus(TournamentDetails tournament);
	}
}
