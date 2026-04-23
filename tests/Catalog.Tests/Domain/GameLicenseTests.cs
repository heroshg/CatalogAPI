using Catalog.Domain.Entities;

namespace Catalog.Tests.Domain;

public class GameLicenseTests
{
    [Fact]
    public void Create_SetsGameIdUserIdAndAcquiredAt()
    {
        var gameId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var license = new GameLicense(gameId, userId);

        Assert.Equal(gameId, license.GameId);
        Assert.Equal(userId, license.UserId);
        Assert.NotEqual(Guid.Empty, license.Id);
        Assert.True(license.AcquiredAt >= before);
    }
}
