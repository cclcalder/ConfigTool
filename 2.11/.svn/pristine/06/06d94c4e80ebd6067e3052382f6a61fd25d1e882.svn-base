using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Website.deployApp.log
{
    public partial class file : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string LocalPath
        {
            get
            {
                var p= "~/deployApp/log/" + DateTime.Now.ToString("yyyyMMdd") + "/";
                bool exists = System.IO.Directory.Exists(Server.MapPath(p));

                if (!exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(p));


                return p;
            }
        }

        protected void OnClick(object sender, EventArgs e)
        {
            var path = LocalPath;
            var subPath = TextBox1.Text;

            if (!string.IsNullOrWhiteSpace(subPath))
            {
                path = path +  subPath;
                bool exists = System.IO.Directory.Exists(Server.MapPath(path));

                if (!exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(path));

            }
            var fn = TextBox2.Text;


            using (StreamWriter _testData = new StreamWriter(Server.MapPath(path + "/" + fn + ".txt"), true))
            {
                _testData.WriteLine(TextBox3.Text); // Write the file.
            }    
        }

        protected void OnClick2(object sender, EventArgs e)
        {
            if (UploadTest.HasFile == false)
            {
                // No file uploaded!
                UploadDetails.Text = "Please first select a file to upload...";
            }
            else
            {
                var path = LocalPath;
                var subPath = TextBox1.Text;

                if (!string.IsNullOrWhiteSpace(subPath))
                {
                    path = path + subPath;
                    bool exists = System.IO.Directory.Exists(Server.MapPath(path));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(path));

                    path = path + "/";
                }

                if (UploadTest.FileBytes.Length < 4000000)
                {
                    // Display the uploaded file's details
                    UploadDetails.Text = string.Format(
                        @"Uploaded file: {0}<br />
                  File size (in bytes): {1:N0}<br />
                  Content-type: {2}",
                        UploadTest.FileName,
                        UploadTest.FileBytes.Length,
                        UploadTest.PostedFile.ContentType);
                    // Save the file
                    string filePath =
                        Server.MapPath(path + UploadTest.FileName);
                    UploadTest.SaveAs(filePath);
                }
                else
                {
                    UploadDetails.Text = string.Format(
                       @"Uploaded too big (max 6Mb): {0}<br />
                      File size (in bytes): {1:N0}<br />
                      Content-type: {2}",
                       UploadTest.FileName,
                       UploadTest.FileBytes.Length,
                       UploadTest.PostedFile.ContentType);
                }
            }
        }
    }
}