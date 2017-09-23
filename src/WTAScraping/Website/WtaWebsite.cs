using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WTAScraping.Driver;
using WTAScraping.Players;
using WTAScraping.Tournaments;
using WTAScraping.Formatters;

namespace WTAScraping.Website
{
	public class WtaWebsite : IWtaWebsite
	{
		private const string RANKINGS_URL = "http://www.wtatennis.com/rankings";

		private readonly Regex _rankingsInstantRegex = new Regex(@"http:\\\/\\\/www\.wtatennis\.com\\\/node\\\/(.*)\\\/singles\\\/ranking\.json", RegexOptions.Compiled);
		private readonly Regex _playerNameRegex = new Regex("<div class=\"player-hidden\">(.*)<\\/div>", RegexOptions.Compiled);

		private readonly string _rankingsJsonUrlFormat = "http://www.wtatennis.com/node/{0}/singles/ranking.json";

		private readonly IWtaDriver _driver;
		private readonly IUrlFormatter _urlFormatter;
		private readonly IPlayerNameFormatter _playerNameFormatter;

		public WtaWebsite(IWtaDriver driver, IUrlFormatter urlFormatter, IPlayerNameFormatter playerNameFormatter)
		{
			_driver = driver;
			_urlFormatter = urlFormatter;
			_playerNameFormatter = playerNameFormatter;
		}

		public IEnumerable<Tournament> GetCurrentAndUpcomingTournaments()
		{
			IEnumerable<Tournament> tournaments = _driver.GetCurrentAndUpcomingTournaments();

			return tournaments;
		}

		public IEnumerable<TournamentDetails> RefreshSeededPlayers(IEnumerable<TournamentDetails> tournamentsDetails)
		{
			foreach (TournamentDetails tournament in tournamentsDetails)
			{
				IEnumerable<SeededPlayer> players =
					_driver.GetTournamentPlayers(_urlFormatter.GetTournamentUrl(tournament.Name, tournament.Date.Year));

				if (!players.Any() && tournament.Status == TournamentStatus.Current)
				{
					tournament.Status = TournamentStatus.Invalid;
				}

				IEnumerable<string> playerNames = 
					players
						.Where(p => p.Seed != SeededPlayer.MAX_SEED)
						.OrderBy(p => p.Seed).Select(p => _playerNameFormatter.GetPlayerName(p.Name))
						.ToList();

				yield return tournament.AsTournamentDetails(playerNames);
			}
		}

		public IEnumerable<Player> GetPlayers()
		{
			var httpClient = new HttpClient();

			HttpResponseMessage response = Task.Run(async () => await httpClient.GetAsync(RANKINGS_URL)).Result;

			string pageConent = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;

			string rankingsJsonUrl =
				string.Format(_rankingsJsonUrlFormat, _rankingsInstantRegex.Match(pageConent).Groups[1].Value);

			response = Task.Run(async () => await httpClient.GetAsync(rankingsJsonUrl)).Result;

			var playerResultItems =
				JsonConvert.DeserializeObject<IEnumerable<PlayerResultItem>>(Task.Run(async () => await response.Content.ReadAsStringAsync()).Result);

			return playerResultItems.Select(p => new Player(_playerNameRegex.Match(p.FullName).Groups[1].Value, p.Rank));
		}

		private class PlayerResultItem
		{
			public string FullName { get; set; }
			public int Rank { get; set; }
		}
	}
}
