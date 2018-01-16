namespace WTAScraper.Players
{
	public class Player
	{
		public int Rank { get; }
		public string Name { get; }

		public Player(string name, int rank)
		{
			Name = name;
			Rank = rank;
		}
	}
}