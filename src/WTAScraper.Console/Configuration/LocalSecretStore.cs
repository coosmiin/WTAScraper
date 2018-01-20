using Microsoft.Extensions.Configuration;

namespace WTAScraper.Console.Configuration
{
	public class LocalSecretStore<T> : ISecretStore where T : class
	{
		IConfigurationRoot _configurationRoot;

		public LocalSecretStore()
		{
			_configurationRoot = BuildConfiguration();
		}

		public string GetIftttKey()
		{
			return _configurationRoot["IftttKey"];
		}

		public string GetSmtpUserName()
		{
			return _configurationRoot["OutlookUsername"];
		}

		public string GetSmtpPassword()
		{
			return _configurationRoot["OutlookPassword"];
		}

		public string GetEmailSenderAddress()
		{
			return _configurationRoot["OutlookSenderAddress"];
		}

		public string GetEmailToAddress()
		{
			return _configurationRoot["OutlookToAddress"];
		}

		private static IConfigurationRoot BuildConfiguration()
		{
			IConfigurationBuilder builder = new ConfigurationBuilder();

			builder.AddUserSecrets<T>();

			IConfigurationRoot configuration = builder.Build();

			return configuration;
		}
	}
}
