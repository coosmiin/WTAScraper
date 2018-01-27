using WTAData.Repositories;
using Xunit;

namespace WTAData.UnitTests.Repositories
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
