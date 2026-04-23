namespace SaaS_BLL.Common;

/// <summary>
/// Represents the outcome of a service operation that returns no data.
/// Used for expected failures (validation, not found, etc).
/// Unexpected failures should throw exceptions.
/// </summary>
public class ServiceResult
{
    protected ServiceResult(bool success, string? error = null)
    {
        this.Success = success;
        this.Error = error;
    }

    public bool Success { get; }

    public string? Error { get; }

    public static ServiceResult Ok() => new (true);

    public static ServiceResult Fail(string error) => new (false, error);
}

/// <summary>
/// Represents the outcome of a service operation that returns data of type <typeparamref name="T"/>.
/// </summary>
public class ServiceResult<T> : ServiceResult
{
    private ServiceResult(bool success, T? data = default, string? error = null)
        : base(success, error)
    {
        this.Data = data;
    }

    public T? Data { get; }

    public static ServiceResult<T> Ok(T data) => new (true, data);

    public new static ServiceResult<T> Fail(string error) => new (false, error: error);
}