using XuongMay.Core.Constants;
using XuongMay.Core.Utils;

namespace XuongMay.Core.Base
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public StatusCodeHelper StatusCode { get; set; }
        public string? Code { get; set; }
        public BaseResponse(StatusCodeHelper statusCode, string code, T? data, string? message)
        {
            Data = data;
            Message = message;
            StatusCode = statusCode;
            Code = code;
        }

        public BaseResponse(StatusCodeHelper statusCode, string code, T? data)
        {
            Data = data;
            StatusCode = statusCode;
            Code = code;
        }

        public BaseResponse(StatusCodeHelper statusCode, string code, string? message)
        {
            Message = message;
            StatusCode = statusCode;
            Code = code;
        }

        public static BaseResponse<T> OkResponse(T? data)
        {
            return new BaseResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.Name(), data);
        }
        public static BaseResponse<T> OkResponse(string? mess)
        {
            return new BaseResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.Name(), mess);
        }

        public static BaseResponse<T> ErrorResponse(string? mess)
        {
            return new BaseResponse<T>(StatusCodeHelper.BadRequest, StatusCodeHelper.BadRequest.Name(), mess);
        }

        public static BaseResponse<T> NotFoundResponse(string? mess)
        {
            return new BaseResponse<T>(StatusCodeHelper.NotFound, StatusCodeHelper.NotFound.Name(), mess);
        }

        public static BaseResponse<T> UnauthorizeResponse(string? mess)
        {
            return new BaseResponse<T>(StatusCodeHelper.Unauthorized, StatusCodeHelper.Unauthorized.Name(), mess);
        }

        public static BaseResponse<T> ForbiddenResponse(string? mess)
        {
            return new BaseResponse<T>(StatusCodeHelper.Forbidden, StatusCodeHelper.Forbidden.Name(), mess);
        }
    }
}
