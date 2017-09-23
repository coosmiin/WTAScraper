using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WTAScraping.Tournaments;
using Xunit;

namespace WTAScraping.UnitTests.Serialization
{
	public class JsonConverterTests
	{
		[Fact]
		public void SerializeObject_ObjectOverridesEquals_SerializaionIsSuccessful()
		{
			JsonConvert.SerializeObject(
				new List<TournamentDetails>
				{
					new TournamentDetails("Some Name", DateTime.Now, TournamentStatus.Current, new [] { "FS", "SS" })
				});
		}
	}
}
