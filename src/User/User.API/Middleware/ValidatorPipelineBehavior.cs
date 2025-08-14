using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.API.Middleware
{
    public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            // The below is inefficient
            var validationErrors = validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .ToList();

            if (validationErrors.Any())
            {
                var errors = validationErrors
                    .GroupBy(x => x.PropertyName.ToCamelCase())
                    .ToDictionary(k => k.Key, v => v.Select(x => x.ErrorMessage).ToArray());

                throw new Core.Exceptions.ValidationException(errors);
            }

            return next();
        }
    }

    public static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            return char.ToLowerInvariant(value[0]) + value[1..];
        }
    }
}
