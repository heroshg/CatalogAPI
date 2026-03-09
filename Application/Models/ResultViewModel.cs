namespace CatalogAPI.Application.Models;

public class ResultViewModel<T>
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }

    private ResultViewModel() { }

    public static ResultViewModel<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static ResultViewModel<T> Error(string message) => new() { IsSuccess = false, Message = message };
}
