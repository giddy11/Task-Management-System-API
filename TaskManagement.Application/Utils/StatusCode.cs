using System.Net;

namespace TaskManagement.Application.Utils;

public enum StatusCode
{
    BadRequest = HttpStatusCode.BadRequest,
    Unauthorized = HttpStatusCode.Unauthorized,
    Forbidden = HttpStatusCode.Forbidden,
    NotFound = HttpStatusCode.NotFound,
    Conflict = HttpStatusCode.Conflict,
    InternalServerError = HttpStatusCode.InternalServerError,
    OK = HttpStatusCode.OK,
    Created = HttpStatusCode.Created,
    NoContent = HttpStatusCode.NoContent
}
