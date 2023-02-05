using FreshFarmMarket.Settings;
using System.Diagnostics;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;


namespace FreshFarmMarket.Services
{

    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public bool SendEmail(string ToEmail, string? Subject, string? HTMLContent, string? TextContent, List<IFormFile>? Attachments)
        {
            if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                Configuration.Default.ApiKey.Add("api-key", _emailConfig.API);
            }
            var apiInstance = new TransactionalEmailsApi();
            string SenderName = "Spoonful";
            string SenderEmail = "noreply@Spoonful.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(ToEmail);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>
            {
                smtpEmailTo
            };

            List<SendSmtpEmailAttachment> Attachment = null;
            if (Attachments != null)
            {
                byte[] fileBytes;
                Attachment = new List<SendSmtpEmailAttachment>();
                foreach (var file in Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        string AttachmentUrl = null;
                        string AttachmentName = file.FileName;
                        SendSmtpEmailAttachment AttachmentContent = new SendSmtpEmailAttachment(AttachmentUrl, fileBytes, AttachmentName);
                        Attachment.Add(AttachmentContent);
                    }
                }
            }
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, HTMLContent, TextContent, Subject, null, Attachment, null, null, null, null, null);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(e.Message);
                return false;
            }
            
        }
    }
}