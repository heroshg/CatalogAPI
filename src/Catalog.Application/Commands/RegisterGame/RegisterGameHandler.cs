using MediatR;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.Application.Commands.RegisterGame;

public class RegisterGameHandler(IGameRepository repository)
    : IRequestHandler<RegisterGameCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RegisterGameCommand request, CancellationToken ct)
    {
        if (await repository.ExistsByNameAsync(request.Name, ct))
            return ResultViewModel<Guid>.Error("A game with this name already exists.");

        var game = new Game(request.Name, request.Description, request.Price);
        var id = await repository.AddAsync(game, ct);
        return ResultViewModel<Guid>.Success(id);
    }
}
