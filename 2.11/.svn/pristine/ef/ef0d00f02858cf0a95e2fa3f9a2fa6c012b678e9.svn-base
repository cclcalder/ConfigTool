using System.Xml.Linq;

namespace SalesPlannerWeb.Models
{
    public class Message
    {
        public static Message New(XElement xml)
        {
            Message message = new Message();

            var xSuccessfulMessage = xml.Element("SuccessMessage");
            var xWarning = xml.Element("Warning");

            if (xSuccessfulMessage != null)
            {
                message.Type = MessageType.Success;
                message.Text = xSuccessfulMessage.Value;
            }
            else if (xWarning != null)
            {
                message.Type = MessageType.Warning;
                message.Text = xWarning.Value;
            }

            return message;
        }

        public MessageType Type { get; set; }
        public string Text { get; set; }
    }
}