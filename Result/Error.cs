using System.Net;

namespace ResultPattern.Result;

public record Error
{
    public string Code { get; set; }
    public int StatusCode { get; set; }
    public string PersianMessage { get; set; }
    public string EnglishMessage { get; set; }

    public Error()
    {
        Code = "";
        StatusCode = 0;
        PersianMessage = "";
    }

    public Error(string code, int statusCode, string persianMessage)
    {
        Code = code;
        StatusCode = statusCode;
        PersianMessage = persianMessage;
    }

    public static Error InternalServerError
        => new ("Error 01", (int)HttpStatusCode.InternalServerError, "خطای داخلی سیستم لطفا دوباره تلاش کنید");

    public static Error BadRequest
        => new ("Error 02", (int)HttpStatusCode.BadRequest, "درخواست اشتباه ارسال کردید");

    public static Error Unauthorized
        => new ("Error 03", (int)HttpStatusCode.Unauthorized, "وارد سیستم نشده اید");

    public static Error Forbidden
        => new ("Error 04", (int)HttpStatusCode.Forbidden, "به این قسمت دسترسی ندارید");
}