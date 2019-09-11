using System;

namespace GeoCoding.Errors
{
    [Serializable]
    public class GCException : SystemException
    {
        public StatusCode StatusCode { get; }

        public GCException(StatusCode status) : base($"GCException: {status.ToString()}")
        {
            StatusCode = status;
        }

        public GCException(string message, StatusCode status) : base(message)
        {
            StatusCode = status;
        }
    }
}
