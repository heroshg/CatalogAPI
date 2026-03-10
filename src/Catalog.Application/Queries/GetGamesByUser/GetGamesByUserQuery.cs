using MediatR;
using Catalog.Application.DTOs;

namespace Catalog.Application.Queries.GetGamesByUser;

public record GetGamesByUserQuery(Guid UserId) : IRequest<ResultViewModel<List<GameViewModel>>>;
