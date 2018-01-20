namespace WTAScraper.Console.Configuration
{
	public interface ISecretStore
	{
		string GetIftttKey();
		string GetSmtpUserName();
		string GetSmtpPassword();
		string GetEmailSenderAddress();
		string GetEmailToAddress();
	}
}
