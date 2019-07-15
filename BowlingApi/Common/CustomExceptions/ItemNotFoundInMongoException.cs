using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BowlingApi.Common.CustomExceptions
{
    [Serializable]
    public class ItemNotFoundInMongoException : Exception
    {
        public ItemNotFoundInMongoException()
            : base()
        {          
        }

        public ItemNotFoundInMongoException(string message)
            : base(message)
        {               
        }

        public ItemNotFoundInMongoException(string message, Exception innerException)
            : base (message, innerException)
        {

        }
        
        protected ItemNotFoundInMongoException(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {

        }
    }
}
