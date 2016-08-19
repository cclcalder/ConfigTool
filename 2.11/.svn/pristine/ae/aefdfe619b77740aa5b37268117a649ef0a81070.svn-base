using System;
 
using System.IO;
 
using System.Net;
using System.Net.Mail;
using System.Text;
 

namespace Exceedra.Common.Logging
{
    public class FeedbackBase
    {
     
        public string Message { get; set; }
        public string User { get; set; }
        public byte[] ScreenShot { get; set; }
 

        public void SaveScreenShotToBLob()
        {
            //var credentials = new StorageCredentials("deldysoft",
            //"YOUR PRIMARY KEY FROM. GET IT FROM THE MANGEMENT PORTAL");
            //var client = new CloudBlobClient(new Uri("http://deldysoft.blob.core.windows.net/"), credentials);
            //// Retrieve a reference to a container. (You need to create one using the mangement portal, or call container.CreateIfNotExists())
            //var container = client.GetContainerReference("deldydk");
            //// Retrieve reference to a blob named "myfile.gif".
            //var blockBlob = container.GetBlockBlobReference("myfile.gif");
            //// Create or overwrite the "myblob" blob with contents from a local file.
            //using (var fileStream = System.IO.File.OpenRead(@"C:\\myfile.gif"))
            //{
            //blockBlob.UploadFromStream(fileStream);
            //} 
        }

        public void SendMail(string message, byte[] attachmentBytes, string logHost, string username )
        {
            try
            {
                var url = System.Windows.Interop.BrowserInteropHelper.Source;

                using (var client = new SmtpClient
                {
                    Host = "email-smtp.us-east-1.amazonaws.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("AKIAJHZ7XYRGH56V5RYA", "AkGHj3Rmvs5yFHLK9IZc4fEDt2nJpQtoBdJghca1ZxH7"),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                })
                {
                    var mail = new MailMessage();
                    mail.To.Add("craighogan+fr25gwnqbykgvul1qk54@boards.trello.com"); // Update your email address
                    mail.From = new MailAddress("ask@chillfire.com");
                    mail.Subject = String.Format("ESP Feedback: {0}", logHost);
                    StringBuilder sbBody = new StringBuilder();
                    sbBody.Append(logHost);
                    sbBody.Append("Date: " + DateTime.Now + Environment.NewLine);
                    sbBody.Append("User: " + username + Environment.NewLine);
                    sbBody.Append("Launch URL: " + url + Environment.NewLine);
                    sbBody.Append("Message: " + message + Environment.NewLine);

                    mail.Body = sbBody.ToString();

                    //Your log file path
                    Attachment attachment = new Attachment(new MemoryStream(attachmentBytes), "Screenshot.jpg");

                    Attachment attachment2 = new Attachment(AppDomain.CurrentDomain.GetPath() + "Log.txt");

                    mail.Attachments.Add(attachment); 
                    mail.Attachments.Add(attachment2);
                
                    //try
                    //{
                     client.Send(mail);                   
                    //}
                    //catch (Exception)
                    //{

                    //    throw;
                    //}
                }


            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
