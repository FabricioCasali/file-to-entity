using System;
using System.Runtime.Serialization;

namespace FileToEntityLib
{
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParserException(long line, IRule rule)
        {
            Line = line;
            Rule = rule;
        }

        public ParserException(string message, long line, IRule rule) : base(message)
        {
            Line = line;
            Rule = rule;
        }

        public ParserException(string message, Exception innerException, long line, IRule rule) : base(message, innerException)
        {
            Line = line;
            Rule = rule;
        }

        protected ParserException(SerializationInfo info, StreamingContext context, long line, IRule rule) : base(info, context)
        {
            Line = line;
            Rule = rule;
        }

        protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public long Line { get; set; }

        public IRule Rule { get; set; }
    }
}