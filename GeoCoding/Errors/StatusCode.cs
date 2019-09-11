using System;

namespace GeoCoding.Errors
{
    public enum StatusCode
    {
        Success = 0,

        BadHttpResponse,
        ApiError,
        MissingJsonParams,
        
        GenericError = 999
    }
}
