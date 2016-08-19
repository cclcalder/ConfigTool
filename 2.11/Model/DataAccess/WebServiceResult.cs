using System;
using System.Xml.Linq;

namespace Model.DataAccess
{
    public abstract class WebServiceResult
    {
        private readonly string _message;

        protected WebServiceResult(string message)
        {
            _message = message;
        }

        public string Message { get { return _message; } }

        public static WebServiceResult FromXml(XElement element)
        {
            XElement messageElement;
            if ((messageElement = element.Element("SuccessMessage")) != null)
            {
                return new WebServiceSuccess(messageElement.Value);
            }
            if ((messageElement = element.Element("Error")) != null)
            {
                return new WebServiceError(messageElement.Value);
            }

            throw new InvalidOperationException("Could not parse Web Service call result.");
        }
    }
}