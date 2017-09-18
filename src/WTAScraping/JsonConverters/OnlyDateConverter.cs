using Newtonsoft.Json.Converters;

namespace WTAScraping.JsonConverters
{
	public class OnlyDateConverter : IsoDateTimeConverter
	{
		public OnlyDateConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
