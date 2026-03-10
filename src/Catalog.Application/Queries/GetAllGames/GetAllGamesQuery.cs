using MediatR;
using Catalog.Application.DTOs;

namespace Catalog.Application.Queries.GetAllGames;

public record GetAllGamesQuery(string Name = "", int Page = 0, int PageSize = 10)
    : IRequest<ResultViewModel<PageResultViewModel<GameViewModel>>>;
