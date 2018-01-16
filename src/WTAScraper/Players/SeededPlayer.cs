namespace WTAScraper.Players
{
	public class SeededPlayer
	{
		public static int MAX_SEED = 99999;

		public string Name { get; }
		public int Seed { get; }

		public SeededPlayer(string name, int seed)
		{
			Name = name;
			Seed = seed;
		}
	}
}
