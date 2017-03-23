using System;
using System.Runtime.Serialization;

namespace UnitySocialCloudSave
{
    public class CredentialException : Exception
    {
        public CredentialException()
        {
        }

        public CredentialException(string message) : base(message)
        {
        }

        protected CredentialException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CredentialException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}