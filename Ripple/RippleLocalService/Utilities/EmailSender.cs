using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RippleLocalService.Utilities
{
    class EmailSender
    {
        MailMessage message;
        String SMTPServer = String.Empty;

        public EmailSender(String smptServer, String emailTo, String emailFrom, String emailSubject)
        {
            message = new MailMessage();
            message.From = new MailAddress(emailFrom);
            message.Subject = emailSubject;
            message.To.Add(emailTo);
            message.IsBodyHtml = false;
            SMTPServer = smptServer;
        }

        public void sendmail(String emailBody)
        {
            //Send only if there are enough attachments
            if (message.Attachments.Count > 0)
            {
                // Body Field        
                message.Body = emailBody;
                SmtpClient smtp = new SmtpClient(SMTPServer);
                smtp.Send(message);
            }
        }

        public void addAttachments(String FilePath)
        {
            message.Attachments.Add(new Attachment(FilePath));
        }

        private void addto(string username)
        {
            // To Field       
            message.To.Add(username);

        }
        public virtual void Dispose()
        {
            message.Attachments.Dispose();
            message.Dispose();
            message = null;
        }
    }
}
