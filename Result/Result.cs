using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ResultPattern.Result;

public record Result<T>
{
    public bool IsFailure { get; }
    public T Data { get; }
    public Error Error { get; }
    public string Title { get; }
    public List<FluentValidation.Results.ValidationFailure> ValidationFailures { get; }

    private Result(T data)
    {
        Data = data;
    }

    private Result(Error error)
    {
        Error = error;
        IsFailure = true;
    }

    private Result(Error error, string title)
    {
        Error = error;
        IsFailure = true;
        Title = title;
    }

    private Result(List<FluentValidation.Results.ValidationFailure> validationFailures)
    {
        IsFailure = true;
        ValidationFailures = validationFailures;
    }

    public static Result<T> OK()
            => new Result<T>(default(T));

    public static Result<T> OK(T Data)
        => new Result<T>(Data);

    public static Result<T> Failure(Error error)
        => new Result<T>(error);

    public static Result<T> Failure(Error error, string title)
        => new Result<T>(error, title);

    public static Result<T> Failure(List<FluentValidation.Results.ValidationFailure> validationFailures)
        => new Result<T>(validationFailures);

    public ObjectResult ApiResponse()
    {
        switch (IsFailure)
        {
            case true when Error != null:
                return new ObjectResult(new
                    {
                        Title = ((HttpStatusCode)Error.StatusCode).ToString(),
                        Status = Error.StatusCode,
                        Errors = new Dictionary<string, string> { { Title ?? "ErrorMessage", Error.PersianMessage } },
                    })
                    { StatusCode = Error.StatusCode };
            case true when ValidationFailures != null:
            {
                var groupedValidationFailures = ValidationFailures.GroupBy(x => x.PropertyName).ToList();
                var result = groupedValidationFailures.ToDictionary(item => "ErrorMessage", item => item.Select(x => x.ErrorMessage).FirstOrDefault());

                return new ObjectResult(new
                    {
                        Title = "Bad Request",
                        Status = 400,
                        Errors = result,
                    })
                    { StatusCode = 400 };
            }
            default:
                return new ObjectResult(new
                    {
                        Title = "Ok",
                        Status = 200,
                        Data
                    })
                    { StatusCode = 200 };
        }
    }
}
