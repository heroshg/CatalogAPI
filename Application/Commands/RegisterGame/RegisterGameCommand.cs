using MediatR;
using CatalogAPI.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.Application.Commands.RegisterGame;

public record RegisterGameCommand(
    [Required][MaxLength(80)] string Name,
    [Required][MaxLength(300)] string Description,
    [Required][Range(0, double.MaxValue)] decimal Price
) : IRequest<ResultViewModel<Guid>>;
