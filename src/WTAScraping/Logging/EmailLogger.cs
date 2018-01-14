using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace WTAScraping.Logging
{
	public class EmailLogger<T> : ILogger where T : class
	{
		public const string LOGGER_NAME = "email";

		private readonly string _applicationName;

		public EmailLogger(string applicationName)
		{
			_applicationName = applicationName;
		}

		public void Log(string eventName, string message)
		{
			IConfigurationRoot configuration = BuildConfiguration();

			var smtp = new SmtpClient("smtp.live.com", 587);

			smtp.Credentials = new NetworkCredential(configuration["OutlookUsername"], configuration["OutlookPassword"]);
			smtp.EnableSsl = true;

			smtp.Send(configuration["OutlookSenderAddress"], configuration["OutlookToAddress"], eventName, message);
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
