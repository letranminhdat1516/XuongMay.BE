﻿using XuongMay.Core.Utils;

namespace XuongMay.Core.Constants
{
    public enum StatusCodeHelper
    {
        [CustomName("Success")]
        OK = 200,

        [CustomName("Bad Request")]
        BadRequest = 400,

        [CustomName("Unauthorized")]
        Unauthorized = 401,

        [CustomName("Forbidden")]
        Forbidden = 403,

        [CustomName("Internal Server Error")]
        ServerError = 500,

        [CustomName("Not Found")]
        NotFound = 404
    }
}
