using Catalog.Application.Commands.RegisterGame;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using Catalog.Domain.Interfaces;
using Moq;

namespace Catalog.Tests.Application;

public class RegisterGameHandlerTests
{
    private static RegisterGameCommand ValidCmd => new("Hades", "A roguelike dungeon crawler.", 59.90m);

    [Fact]
    public async Task DuplicateName_Handle_ReturnsError()
    {
        var repo = new Mock<IGameRepository>();
        repo.Setup(r => r.ExistsByNameAsync("Hades", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var sut = new RegisterGameHandler(repo.Object);
        var result = await sut.Handle(ValidCmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("A game with this name already exists.", result.Message);
        repo.Verify(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Valid_Handle_CreatesGameReturnsId()
    {
        var expectedId = Guid.NewGuid();

        var repo = new Mock<IGameRepository>();
        repo.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var sut = new RegisterGameHandler(repo.Object);
        var result = await sut.Handle(ValidCmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedId, result.Data);
        repo.Verify(r => r.AddAsync(
            It.Is<Game>(g => g.Name.Value == "Hades" && g.Price.Amount == 59.90m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NegativePrice_Handle_ThrowsDomainException()
    {
        var repo = new Mock<IGameRepository>();
        repo.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new RegisterGameHandler(repo.Object);
        var cmd = ValidCmd with { Price = -5m };
        var act = async () => await sut.Handle(cmd, CancellationToken.None);

        await Assert.ThrowsAsync<DomainException>(act);
        repo.Verify(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
