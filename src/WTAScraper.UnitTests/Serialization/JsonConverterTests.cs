using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WTAData.Tournaments;
using Xunit;

namespace WTAScraper.UnitTests.Serialization
{
	public class JsonConverterTests
	{
		[Fact]
		public void SerializeObject_ObjectOverridesEquals_SerializaionIsSuccessful()
		{
			JsonConvert.SerializeObject(
				new List<TournamentDetails>
				{
					new TournamentDetails(1, "Some Name", DateTime.Now, new DateTime(0), TournamentStatus.Current, new [] { "FS", "SS" })
				});
		}
	}
}
