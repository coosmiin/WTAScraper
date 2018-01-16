using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraper.Data;
using WTAScraper.Tournaments;
using Xunit;

namespace WTAScraper.UnitTests.Data
{
	public class PlayerRepositoryTests
	{
		[Fact]
		public void GetPlayers_FileDoesNotExist_ReturnsEmptyCollection()
		{
			var repository = new PlayerRepository("does_not_exist.json");

			Assert.Empty(repository.GetPlayers());
		}
	}
}
