using System.Xml.Linq;

namespace Exceedra.Common
{
    public enum ValidationStatus
    {
        Success = 1,
        Warning = 2,
        Error = 0
    }

    public static class ValidationStatusGetter
    {
        public static ValidationStatus Get(XElement xml)
        {
            if (xml == null || xml.Name != "ValidationStatus") return 0;
            var validationStatus = xml.MaybeValue();

            switch (validationStatus.ToLower())
            {
                case "2": return ValidationStatus.Warning;
                case "1": case "true": return ValidationStatus.Success;
                default: return ValidationStatus.Error;
            }
        }
    }

    public struct ValidationResult
    {
        private readonly ValidationStatus _status;
        private readonly string _message;

        public ValidationResult(ValidationStatus status, string message)
            : this()
        {
            _status = status;
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }

        public ValidationStatus Status
        {
            get { return _status; }
        }
    }
}