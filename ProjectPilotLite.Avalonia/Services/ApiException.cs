using System;
using System.Net;

namespace ProjectPilotLite.Avalonia.Services;

public class ApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public ApiException(string message, HttpStatusCode? statusCode = null, Exception? inner = null)
        : base(message, inner) => StatusCode = statusCode;
}