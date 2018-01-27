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
}
