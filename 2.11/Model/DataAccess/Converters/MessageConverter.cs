using System.Windows;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Mvvm;

namespace Model.DataAccess.Converters
{
    public static class MessageConverter
    {
        public static bool DisplayMessage(XElement result)
        {
            bool success = CheckForSuccess(result);
            string mess = "";
            
            var messageElement = result.Element("Message");
            if(messageElement != null)
            {
                //edge case for RobCreator
                var outcomeMessageElement = messageElement.Element("OutcomeMsg");
                if (outcomeMessageElement != null)
                    mess = outcomeMessageElement.MaybeValue();
                else
                    mess = messageElement.MaybeValue();
            }
            mess = string.IsNullOrWhiteSpace(mess) ? result.Value : mess;

            if (success)
            {
                Messages.Instance.PutInfo(mess);
            }
            else
            {
                Messages.Instance.PutError(mess);
            }
      
            return success;
        }

        public static bool CheckForSuccess(XElement result)
        {
            return result != null && result.ToString().ToLower().Contains("success");
        }

        public static bool CheckForError(XElement result)
        {
            return result == null || result.ToString().ToLower().Contains("error");
        }
    }
}