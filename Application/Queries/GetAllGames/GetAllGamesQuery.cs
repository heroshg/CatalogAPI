using MediatR;
using CatalogAPI.Application.Models;

namespace CatalogAPI.Application.Queries.GetAllGames;

public record GetAllGamesQuery(string Name = "", int Page = 0, int PageSize = 10)
    : IRequest<ResultViewModel<PageResultViewModel<GameViewModel>>>;
