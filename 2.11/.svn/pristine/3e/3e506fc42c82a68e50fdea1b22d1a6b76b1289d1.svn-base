using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Xml.Linq;

namespace Exceedra.Common.Entity
{
    public class AppEmailer
    {
        public AppEmailer() { }

        //public void SendEmail(ResetEmailModel resetEmailModel, ServerDetailsModel serverDetailsModel, string hostConfigDetails)
        //{
        //    string to = resetEmailModel.UserEmail;
        //    string from = serverDetailsModel.FromEmail;
        //    string sender = serverDetailsModel.FromEmail;
        //    string subject = resetEmailModel.Subject;
        //    string body = resetEmailModel.Body + "\n \n \n";
        //    string footer = hostConfigDetails;

        //    MailMessage message = new MailMessage(from, to, subject, (body + footer));
        //    message.IsBodyHtml = false;
        //    message.Sender = new MailAddress(sender);

        //    SmtpClient client = new SmtpClient();

        //    client.UseDefaultCredentials = false;

        //    client.Port = serverDetailsModel.Port;
        //    client.Host = serverDetailsModel.ServerAddress;

        //    client.Credentials = new NetworkCredential(serverDetailsModel.FromEmail, serverDetailsModel.Password);

        //    //client.DeliveryMethod = SmtpDeliveryMethod.Network;

        //    client.EnableSsl = true;

        //    // Credentials are necessary if the server requires the client 
        //    // to authenticate before it will send e-mail on the client's behalf.

        //    try
        //    {
        //        client.Send(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
        //            ex.ToString());
        //    }


        //}

        public void SendFeedbackEmail(ServerDetailsModel serverDetailsModel, string hostConfigDetails, string version, string body, byte[] attachmentBytes, Attachment attachment2)
        {

            string to ="";
            string from = serverDetailsModel.FromEmail;
            string sender = serverDetailsModel.FromEmail;
            string subject = "ESP Feedback";
            
            string footer = hostConfigDetails;

            MailMessage message = new MailMessage(from, to, subject, (body + footer));
            message.IsBodyHtml = false;
            message.Sender = new MailAddress(sender);

            Attachment attachment = new Attachment(new MemoryStream(attachmentBytes), "Screenshot.jpg");
            message.Attachments.Add(attachment);
            message.Attachments.Add(attachment2);

            SmtpClient client = new SmtpClient();

            client.UseDefaultCredentials = false;

            client.Port = serverDetailsModel.Port;
            client.Host = serverDetailsModel.ServerAddress;

            client.Credentials = new NetworkCredential(serverDetailsModel.FromEmail, serverDetailsModel.Password);

            client.EnableSsl = true;
             
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
                    ex.ToString());
            }


        }

    }

    public class ResetEmailModel
    {
        public ResetEmailModel() {}

        public ResetEmailModel(XElement resElement)
        {
            //ResetCode = resElement.Element("User_ResetCode").Value;
            //UserEmail = resElement.Element("User_Email").Value;
            Message = resElement.Element("Message").Value;
            //Subject = resElement.Element("Email_Subject").Value;
            //Body = resElement.Element("Email_Body").Value;
        }

        //public string ResetCode { get; set; }
        //public string UserEmail { get; set; }
        public string Message { get; set; }
        //public string Subject { get; set; }
        //public string Body { get; set; }
    }

    public class ServerDetailsModel
    {
        public ServerDetailsModel() { }

        public ServerDetailsModel(XElement resElement)
        {
            Port = Int32.Parse(resElement.Element("PortNumber").Value);
            Password = resElement.Element("Password").Value;
            ServerAddress = resElement.Element("ServerAddress").Value;
            FromEmail = resElement.Element("From_Email").Value;
        }

        public int Port { get; set; }
        public string Password { get; set; }
        public string ServerAddress { get; set; }
        public string FromEmail { get; set; }
    }
}
