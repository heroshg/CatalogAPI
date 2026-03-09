using MediatR;
using CatalogAPI.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.Application.Commands.PlaceOrder;

public record PlaceOrderCommand(
    [Required] Guid GameId
) : IRequest<ResultViewModel<Guid>>;
