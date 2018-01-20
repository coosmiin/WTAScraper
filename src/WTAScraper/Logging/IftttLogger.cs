using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Text;

namespace WTAScraper.Logging
{
	public class IftttLogger : ILogger
	{
		public const string LOGGER_NAME = "ifttt";

		private const string IFTTT_WEBHOOKS_NOTIFY_URL_FORMAT = "https://maker.ifttt.com/trigger/notify/with/key/{0}";
		private readonly string _applicationName;
		private readonly string _iftttKey;

		public IftttLogger(string applicationName, string iftttKey)
		{
			_applicationName = applicationName;
			_iftttKey = iftttKey;
		}

		public void Log(string eventName, string message)
		{
			var httpClient = new HttpClient();

			var content = new IftttContent
			{
				Value1 = message,
				Value2 = _applicationName,
				Value3 = eventName
			};

			httpClient.PostAsync(
				string.Format(IFTTT_WEBHOOKS_NOTIFY_URL_FORMAT, _iftttKey),
				new StringContent(
					JsonConvert.SerializeObject(
						content, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), 
					Encoding.UTF8, 
					"application/json"))
				.GetAwaiter().GetResult();
		}

		private struct IftttContent
		{
			public string Value1;
			public string Value2;
			public string Value3;
		}
	}
}
