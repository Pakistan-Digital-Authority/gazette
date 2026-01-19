namespace gop.Enums;

public enum StatusCodeEnum
{ 
    Ok = 200, 
    Created = 201,
    Accepted = 202,
    NoContent = 204,

    // 4xx Client Errors
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    Conflict = 409,
    UnsupportedMediaType = 415,
    UnprocessableEntity = 422,
    Locked = 423,

    // 5xx Server Errors
    InternalServerError = 500,
    NotImplemented = 501,
    BadGateway = 502,
    ServiceUnavailable = 503,
    GatewayTimeout = 504
}