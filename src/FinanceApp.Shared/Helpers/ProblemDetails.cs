namespace FinanceApp.Shared.Helpers;

/// <summary>
/// RFC 7807 Problem Details for HTTP APIs
/// </summary>
public class ProblemDetails
{
    public string Type { get; set; } = "about:blank";
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }

    public static ProblemDetails BadRequest(string detail)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = detail
        };
    }

    public static ProblemDetails Unauthorized(string detail = "Sie sind nicht angemeldet")
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = 401,
            Detail = detail
        };
    }

    public static ProblemDetails Forbidden(string detail = "Sie haben keine Berechtigung für diese Aktion")
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Title = "Forbidden",
            Status = 403,
            Detail = detail
        };
    }

    public static ProblemDetails NotFound(string detail)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Not Found",
            Status = 404,
            Detail = detail
        };
    }

    public static ProblemDetails InternalServerError(string detail = "Ein interner Fehler ist aufgetreten")
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = 500,
            Detail = detail
        };
    }

    public static ProblemDetails ValidationError(Dictionary<string, string[]> errors)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation Error",
            Status = 422,
            Detail = "Ein oder mehrere Validierungsfehler sind aufgetreten",
            Extensions = new Dictionary<string, object> { { "errors", errors } }
        };
    }
}
