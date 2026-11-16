namespace Core.Domain.Common;

public sealed record PaginationSettings(int DefaultPageSize = 10, int MaxPageSize = 100)
{
    public const int DefaultPage = 1;

    public static PaginationSettings Default { get; } = new();
}
