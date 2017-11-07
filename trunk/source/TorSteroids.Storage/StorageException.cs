using System;
using System.Runtime.Serialization;
using TorSteroids.Storage.Resources;

namespace TorSteroids.Storage
{
    [Serializable]
    public class StorageException: ApplicationException
    {
        public StorageException()
            : base(SR.GeneralStorageException)
        {
        }

        public StorageException(string s)
            : base(s)
        {
        }

        protected StorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public StorageException(string s, Exception innerException)
            : base(s, innerException)
        {
        }
    }
}
