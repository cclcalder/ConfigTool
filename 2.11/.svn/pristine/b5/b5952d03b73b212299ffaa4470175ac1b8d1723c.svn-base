using System.Collections.Generic;

namespace Exceedra.Common
{
    using System;
    using System.Linq;

    public static class AggregateExceptionExtensions
    {
        public static IEnumerable<string> GetMessages(this AggregateException exception)
        {
            if (exception == null) return Enumerable.Empty<string>();
            if (exception.InnerExceptions.Count > 0)
            {
                return exception.InnerExceptions.Select(ex => ex.Message);
            }
            if (exception.InnerException != null)
            {
                return Enumerable.Repeat(exception.InnerException.Message, 1);
            }
            return Enumerable.Repeat(exception.Message, 1);
        }
        public static string AggregateMessages(this AggregateException exception, string separator = "\r\n")
        {
            if (exception == null) return string.Empty;
            if (exception.InnerExceptions.Count == 0)
            {
                return exception.InnerException.Message;
            }
            if (exception.InnerExceptions.Count == 1)
            {
                return exception.InnerExceptions[0].Message;
            }
            return string.Join(separator, exception.InnerExceptions.Select(e => e.Message));
        }
    }
}