using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace UnitySocialCloudSave.Client
{
    [Serializable]
    public class ErrorResponseException : Exception
    {
        private readonly string _code;
        private readonly HttpStatusCode _httpStatusCode;

        public static bool IsDatasetConflict(object err)
        {
            var serverError = err as ErrorResponseException;
            return serverError != null && serverError.Code == "DatasetConflict";
        }

        public string Code
        {
            get { return _code; }
        }

        public HttpStatusCode HttpStatusCode
        {
            get { return _httpStatusCode; }
        }

        public ErrorResponseException(string code, HttpStatusCode httpStatusCode)
        {
            _code = code;
            _httpStatusCode = httpStatusCode;
        }

        protected ErrorResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _code = info.GetString("Code");
            _httpStatusCode = (HttpStatusCode) info.GetValue("HttpStatusCode", typeof(HttpStatusCode));
        }

        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Code", Code);
            info.AddValue("HttpStatusCode", HttpStatusCode, typeof(HttpStatusCode));
        }
    }
}