using MediatR;
using Catalog.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Commands.RegisterGame;

public record RegisterGameCommand(
    [Required][MaxLength(80)] string Name,
    [Required][MaxLength(300)] string Description,
    [Required][Range(0, double.MaxValue)] decimal Price
) : IRequest<ResultViewModel<Guid>>;
