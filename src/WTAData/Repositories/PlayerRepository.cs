using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WTAData.Players;

namespace WTAData.Repositories
{
	public class PlayerRepository : IPlayerRepository
	{
		private readonly string _filePath;

		public PlayerRepository(string filePath)
		{
			_filePath = filePath;
		}

		public IEnumerable<Player> GetPlayers()
		{
			if (!File.Exists(_filePath))
				return Enumerable.Empty<Player>();

			return JsonConvert.DeserializeObject<IEnumerable<Player>>(File.ReadAllText(_filePath));
		}

		public void SavePlayers(IEnumerable<Player> players)
		{
			File.WriteAllText(_filePath, JsonConvert.SerializeObject(players));
		}
	}
}
