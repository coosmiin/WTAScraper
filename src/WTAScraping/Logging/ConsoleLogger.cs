using System.IO;

namespace WTAScraping.Logging
{
	public class ConsoleLogger : ILogger
	{
		private readonly TextWriter _textWriter;

		public ConsoleLogger(TextWriter textWriter)
		{
			_textWriter = textWriter;
		}

		public void Log(string eventName, string message)
		{
			_textWriter.WriteLine($"{eventName}: {message}");
		}
	}
}
