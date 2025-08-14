using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Planorama.User.Core.Exceptions;
using System.Diagnostics;
using System.Text.Json;

namespace Planorama.User.API
{
    public static class StartupExtensions
    {
        public static void AddExceptionHandling(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    var problemDetails = new ProblemDetails
                    {
                        Type = $"https://example.com/problem-types/{exception.GetType().Name}",
                        Title = "An unexpected error occurred!",
                        Detail = "Something went wrong",
                        Instance = errorFeature switch
                        {
                            ExceptionHandlerFeature e => e.Path,
                            _ => "unknown"
                        },
                        Status = StatusCodes.Status500InternalServerError,
                        Extensions =
                        {
                            ["trace"] = Activity.Current?.Id ?? context?.TraceIdentifier
                        }
                    };

                    switch (exception)
                    {
                        case ValidationException validationException:
                            problemDetails.Status = StatusCodes.Status400BadRequest;
                            problemDetails.Title = "One or more validation errors occurred";
                            problemDetails.Detail = "The request contains invalid parameters. More information can be found in the errors.";
                            problemDetails.Extensions["errors"] = validationException.Errors;
                            break;
                        case AuthorizationException:
                            problemDetails.Status = StatusCodes.Status401Unauthorized;
                            problemDetails.Title = "Unauthorized Access";
                            problemDetails.Detail = "You are not authorized to access this resource.";
                            break;
                        case LoginFailedException:
                            problemDetails.Status = StatusCodes.Status401Unauthorized;
                            problemDetails.Title = "Login Failed";
                            problemDetails.Detail = "Your email and/or password is invalid.";
                            break;
                        case RefreshTokenException refreshTokenException:
                            problemDetails.Status = StatusCodes.Status401Unauthorized;
                            problemDetails.Title = "One or more refresh token errors occurred";
                            problemDetails.Detail = "The refresh token is invalid or expired. More information can be found in the errors.";
                            problemDetails.Extensions["errors"] = refreshTokenException.Errors;
                            break;
                        case ForbiddenException:
                            problemDetails.Status = StatusCodes.Status403Forbidden;
                            problemDetails.Title = "Authenticated user is not authorized";
                            problemDetails.Detail = "You do not have the correct role to perform this request.";
                            break;
                        case NotFoundException notFoundException:
                            problemDetails.Status = StatusCodes.Status404NotFound;
                            problemDetails.Title = "Resource Not Found";
                            problemDetails.Detail = "The resource you are looking for was not found or has been deleted.";
                            problemDetails.Extensions["errors"] = notFoundException.Errors;
                            break;
                    }

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = problemDetails.Status.Value;
                    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                    {
                        NoCache = true,
                    };

                    await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails);

                });
            });
        }
    }
}
