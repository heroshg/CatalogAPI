using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using Catalog.Domain.ValueObjects;

namespace Catalog.Tests.Domain;

public class GameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpaceName_Create_ThrowsDomainException(string? name)
    {
        var ex = Assert.Throws<DomainException>(() => new Game(name!, "A description.", 10m));
        Assert.Equal("Game name cannot be empty.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpaceDescription_Create_ThrowsDomainException(string? description)
    {
        var ex = Assert.Throws<DomainException>(() => new Game("Hades", description!, 10m));
        Assert.Equal("Game description cannot be empty.", ex.Message);
    }

    [Fact]
    public void NegativePrice_Create_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Game("Hades", "A roguelike.", -0.01m));
        Assert.Equal("Price cannot be negative.", ex.Message);
    }

    [Fact]
    public void ZeroPrice_Create_DoesNotThrow()
    {
        var ex = Record.Exception(() => new Game("Hades", "A roguelike.", 0m));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidArguments_Create_SetsPropertiesCorrectly()
    {
        var game = new Game("Hades", "A roguelike dungeon crawler.", 59.90m);

        Assert.Equal("Hades", game.Name.Value);
        Assert.Equal("A roguelike dungeon crawler.", game.Description.Value);
        Assert.Equal(59.90m, game.Price.Amount);
        Assert.True(game.IsActive);
        Assert.NotEqual(Guid.Empty, game.Id);
    }

    [Fact]
    public void Deactivate_ActiveGame_SetsIsActiveFalse()
    {
        var game = new Game("Hades", "A roguelike.", 10m);
        game.Deactivate();
        Assert.False(game.IsActive);
    }

    [Fact]
    public void Deactivate_AlreadyInactive_ThrowsDomainException()
    {
        var game = new Game("Hades", "A roguelike.", 10m);
        game.Deactivate();
        Assert.Throws<DomainException>(() => game.Deactivate());
    }

    [Fact]
    public void Reactivate_InactiveGame_SetsIsActiveTrue()
    {
        var game = new Game("Hades", "A roguelike.", 10m);
        game.Deactivate();
        game.Reactivate();
        Assert.True(game.IsActive);
    }

    [Fact]
    public void Reactivate_AlreadyActive_ThrowsDomainException()
    {
        var game = new Game("Hades", "A roguelike.", 10m);
        Assert.Throws<DomainException>(() => game.Reactivate());
    }

    [Fact]
    public void UpdateDetails_ChangesPropertiesAndUpdatedAt()
    {
        var game = new Game("Hades", "Original desc.", 10m);
        var before = game.UpdatedAt;

        game.UpdateDetails("Hades II", "New desc.", 20m);

        Assert.Equal("Hades II", game.Name.Value);
        Assert.Equal("New desc.", game.Description.Value);
        Assert.Equal(20m, game.Price.Amount);
        Assert.True(game.UpdatedAt >= before);
    }
}
