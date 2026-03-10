namespace Catalog.Application.DTOs;

public record GameViewModel(Guid Id, string Name, string Description, decimal Price, bool IsActive, DateTime CreatedAt);

public record PageResultViewModel<T>(List<T> Items, int Total, int Page, int PageSize);
