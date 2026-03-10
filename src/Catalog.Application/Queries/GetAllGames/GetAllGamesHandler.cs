using MediatR;
using Catalog.Application.DTOs;
using Catalog.Domain.Interfaces;

namespace Catalog.Application.Queries.GetAllGames;

public class GetAllGamesHandler(IGameRepository repository)
    : IRequestHandler<GetAllGamesQuery, ResultViewModel<PageResultViewModel<GameViewModel>>>
{
    public async Task<ResultViewModel<PageResultViewModel<GameViewModel>>> Handle(GetAllGamesQuery request, CancellationToken ct)
    {
        var games = await repository.GetAllAsync(request.Name, request.Page, request.PageSize, ct);
        var vms = games.Select(g => new GameViewModel(g.Id, g.Name, g.Description, g.Price, g.IsActive, g.CreatedAt)).ToList();
        return ResultViewModel<PageResultViewModel<GameViewModel>>.Success(
            new PageResultViewModel<GameViewModel>(vms, vms.Count, request.Page, request.PageSize));
    }
}
