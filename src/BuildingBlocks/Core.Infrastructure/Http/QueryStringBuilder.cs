using Microsoft.AspNetCore.WebUtilities;

namespace Core.Infrastructure.Http;

public static class QueryStringBuilder
{
    /// <summary>
    /// Appends the non-null parameters as an encoded query string: <c>Build("/orders", new() { ["page"] = "2" })</c>.
    /// Replaces <c>Helpers.BuildUrlWithQueryStringUsingParseQueryStringMethod</c> (System.Web dependency, kept nulls).
    /// </summary>
    public static string Build(string basePath, IReadOnlyDictionary<string, string?> queryParameters) =>
        QueryHelpers.AddQueryString(basePath, queryParameters.Where(parameter => parameter.Value is not null));
}
