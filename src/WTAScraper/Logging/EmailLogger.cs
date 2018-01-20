using System.Net;
using System.Net.Mail;

namespace WTAScraper.Logging
{
	public class EmailLogger<T> : ILogger where T : class
	{
		public const string LOGGER_NAME = "email";

		private readonly string _applicationName;
		private readonly EmailSettings _settings;

		public EmailLogger(string applicationName, EmailSettings settings)
		{
			_applicationName = applicationName;
			_settings = settings;
		}

		public void Log(string eventName, string message)
		{
			var smtp = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort);

			smtp.Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword);
			smtp.EnableSsl = true;

			smtp.Send(_settings.EmailSenderAddress, _settings.EmailToAddress, eventName, message);
		}
	}
}
