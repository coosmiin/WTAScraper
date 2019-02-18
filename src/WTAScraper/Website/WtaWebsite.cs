using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WTAScraper.Driver;
using WTAData.Players;
using WTAData.Tournaments;
using WTAScraper.Formatters;

namespace WTAScraper.Website
{
	public class WtaWebsite : IWtaWebsite, IWebDriverWrapper
	{
		private const string RANKINGS_URL = "http://www.wtatennis.com/rankings";

		private static object _lock = new object();

		private readonly Regex _rankingsInstantRegex = new Regex(@"https:\\\/\\\/www\.wtatennis\.com\\\/node\\\/(.*)\\\/singles\\\/ranking\.json", RegexOptions.Compiled);
		private readonly Regex _playerNameRegex = new Regex("<div class=\"player-hidden\">(.*)<\\/div>", RegexOptions.Compiled);

		private readonly string _rankingsJsonUrlFormat = "http://www.wtatennis.com/node/{0}/singles/ranking.json";

		private readonly IWtaDriverFactory _driverFactory;
		private readonly IUrlFormatter _urlFormatter;
		private readonly IPlayerNameFormatter _playerNameFormatter;

		private IWtaDriver _wtaDriver;

		private IWtaDriver WtaDriver
		{
			get
			{
				if (_wtaDriver == null)
				{
					lock (_lock)
					{
						if (_wtaDriver == null)
						{
							_wtaDriver = _driverFactory.CreateDriver();
						}
					}
				}
				return _wtaDriver;
			}
		}

		public WtaWebsite(IWtaDriverFactory driverFactory, IUrlFormatter urlFormatter, IPlayerNameFormatter playerNameFormatter)
		{
			_driverFactory = driverFactory;
			_urlFormatter = urlFormatter;
			_playerNameFormatter = playerNameFormatter;
		}

		public IEnumerable<Tournament> GetCurrentAndUpcomingTournaments()
		{
			IEnumerable<Tournament> tournaments = WtaDriver.GetCurrentAndUpcomingTournaments();

			return tournaments;
		}

		public IEnumerable<TournamentDetails> RefreshSeededPlayers(IEnumerable<TournamentDetails> tournamentsDetails)
		{
			foreach (TournamentDetails tournament in tournamentsDetails)
			{
				IEnumerable<SeededPlayer> players =
					WtaDriver.GetTournamentPlayers(
						_urlFormatter.GetTournamentUrl(tournament.Id, tournament.Name, tournament.StartDate));

				if (!players.Any() && tournament.Status == TournamentStatus.Current)
				{
					tournament.Status = TournamentStatus.Invalid;
				}

				IEnumerable<string> playerNames = 
					players
						.Where(p => p.Seed != SeededPlayer.MAX_SEED)
						.OrderBy(p => p.Seed).Select(p => _playerNameFormatter.GetPlayerName(p.Name))
						.ToList();

				int tournamentRounds =
					players.Any() 
					? (int)Math.Log(players.Count(), 2) - (playerNames.Count() > 1 && playerNames.ElementAt(1) == string.Empty ? 1 : 0)
					: 0;

				yield return tournament.AsTournamentDetails(playerNames, tournamentRounds);
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

		public void CloseDriver()
		{
			IWebDriverWrapper webDriverWrapper = _driverFactory as IWebDriverWrapper;

			if (webDriverWrapper != null)
				webDriverWrapper.CloseDriver();
		}

		private class PlayerResultItem
		{
			public string FullName { get; set; }
			public int Rank { get; set; }
		}
	}
}
