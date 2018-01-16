using Newtonsoft.Json.Converters;

namespace WTAScraper.JsonConverters
{
	public class OnlyDateConverter : IsoDateTimeConverter
	{
		public OnlyDateConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
