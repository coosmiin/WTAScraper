using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraping.Data;
using WTAScraping.Tournaments;
using Xunit;

namespace WTAScraping.UnitTests.Data
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
