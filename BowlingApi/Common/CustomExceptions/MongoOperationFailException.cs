using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BowlingApi.Common.CustomExceptions
{
    [Serializable]
    public class MongoOperationFailException : Exception
    {
        public MongoOperationFailException()
            : base()
        {
        }

        public MongoOperationFailException(string message)
            : base(message)
        {
        }

        public MongoOperationFailException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        protected MongoOperationFailException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
