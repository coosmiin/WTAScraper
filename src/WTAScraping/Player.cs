namespace WTAScraping
{
	public class Player
	{
		public Player(string name, int rank)
		{
			Name = name;
			Rank = rank;
		}

		public int Rank { get; }
		public string Name { get; }
	}
}