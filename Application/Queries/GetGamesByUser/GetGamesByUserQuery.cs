using MediatR;
using CatalogAPI.Application.Models;

namespace CatalogAPI.Application.Queries.GetGamesByUser;

public record GetGamesByUserQuery(Guid UserId) : IRequest<ResultViewModel<List<GameViewModel>>>;
