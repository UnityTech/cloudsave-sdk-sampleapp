using System;
using System.Runtime.Serialization;

namespace UnitySocialCloudSave
{
    public class DatasetSyncException : Exception
    {
        public DatasetSyncException()
        {
        }

        public DatasetSyncException(string message) : base(message)
        {
        }

        protected DatasetSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DatasetSyncException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SyncCanceledException : DatasetSyncException
    {
        public SyncCanceledException()
        {
        }

        public SyncCanceledException(string message) : base(message)
        {
        }

        protected SyncCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SyncCanceledException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SyncNetworkException : DatasetSyncException
    {
        public SyncNetworkException()
        {
        }

        public SyncNetworkException(string message) : base(message)
        {
        }

        protected SyncNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SyncNetworkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SyncConflictingException : SyncCanceledException
    {
        public SyncConflictingException()
        {
        }

        public SyncConflictingException(string message) : base(message)
        {
        }

        protected SyncConflictingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SyncConflictingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}