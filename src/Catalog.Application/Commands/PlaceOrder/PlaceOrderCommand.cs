using MediatR;
using Catalog.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Commands.PlaceOrder;

public record PlaceOrderCommand([Required] Guid GameId) : IRequest<ResultViewModel<Guid>>;
