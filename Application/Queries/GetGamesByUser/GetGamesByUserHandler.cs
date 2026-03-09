using MediatR;
using CatalogAPI.Application.Models;
using CatalogAPI.Domain.Interfaces;

namespace CatalogAPI.Application.Queries.GetGamesByUser;

public class GetGamesByUserHandler(IGameLicenseRepository repository)
    : IRequestHandler<GetGamesByUserQuery, ResultViewModel<List<GameViewModel>>>
{
    public async Task<ResultViewModel<List<GameViewModel>>> Handle(GetGamesByUserQuery request, CancellationToken ct)
    {
        var games = await repository.GetGamesByUserIdAsync(request.UserId, ct);
        var vms = games.Select(g => new GameViewModel(g.Id, g.Name, g.Description, g.Price, g.IsActive, g.CreatedAt)).ToList();
        return ResultViewModel<List<GameViewModel>>.Success(vms);
    }
}
