namespace WTAScraper.Logging
{
	public class EmailSettings
	{
		public string SmtpHost { get; }
		public int SmtpPort { get; }
		public string SmtpUsername { get; }
		public string SmtpPassword { get; }
		public string EmailSenderAddress { get; }
		public string EmailToAddress { get; }

		public EmailSettings(
			string smtpHost, int smtpPort, string smtpUsername, string smtpPassword, string emailSenderAddress, string emailToAddress)
		{
			SmtpHost = smtpHost;
			SmtpPort = smtpPort;
			SmtpUsername = smtpUsername;
			SmtpPassword = smtpPassword;
			EmailSenderAddress = emailSenderAddress;
			EmailToAddress = emailToAddress;
		}
	}
}
