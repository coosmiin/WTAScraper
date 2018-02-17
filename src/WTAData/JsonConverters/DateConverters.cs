using Newtonsoft.Json.Converters;

namespace WTAData.JsonConverters
{
	public class OnlyDateConverter : IsoDateTimeConverter
	{
		public OnlyDateConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}

	public class OnlyDateAndTimeConverter : IsoDateTimeConverter
	{
		public OnlyDateAndTimeConverter()
		{
			DateTimeFormat = "yyyy-MM-dd HH:mm";
		}
	}
}
