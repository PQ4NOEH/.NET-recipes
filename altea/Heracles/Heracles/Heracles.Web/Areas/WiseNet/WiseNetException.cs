namespace Heracles.Web.Areas.WiseNet
{
    using System;

    using System.Runtime.Serialization;

    public class WiseNetException : Exception, ISerializable
    {
        public WiseNetException()
        {
        }

        public WiseNetException(string uri)
            : base(uri)
        {
        }

        public WiseNetException(string uri, Exception inner)
            : base(uri, inner)
        {
        }

        protected WiseNetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
    }
}