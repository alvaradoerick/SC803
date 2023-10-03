using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using AseIsthmusAPI.Data.DTOs;
using MailKit.Net.Smtp;
using AseIsthmusAPI.Templates;
using System.Text.RegularExpressions;

namespace AseIsthmusAPI.Services
{
    public class EmailService
    {
       private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public void SendEmail(string emailTemplate, string subject, string emailTo, string attachmentLink = null, string fileName = "documento")
        {
            var email = new MimeMessage();


            email.From.Add(MailboxAddress.Parse("configuration6Aso@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailTemplate };


            if (!string.IsNullOrEmpty(attachmentLink))
            {
                var fileId = GetGoogleDriveFileId(attachmentLink);

                if (!string.IsNullOrEmpty(fileId))
                {
                    var downloadLink = $"https://drive.google.com/uc?id={fileId}&export=download";

                    var fileContent = DownloadFileFromUrl(downloadLink);
                    if (fileContent != null)
                    {
                        var attachment = new MimePart("application", "pdf")
                        {
                            Content = new MimeContent(new MemoryStream(fileContent)),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = fileName+".pdf"
                        };


                        var multipart = new Multipart("mixed")
                        {
                            email.Body,
                            attachment
                        };

                        email.Body = multipart;
                    }
                }
            }
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        private string GetGoogleDriveFileId(string googleDriveLink)
        {
            var uri = new Uri(googleDriveLink);
            var fileIdMatch = Regex.Match(uri.AbsoluteUri, @"\/file\/d\/([^\/]+)\/?");
            if (fileIdMatch.Success && fileIdMatch.Groups.Count > 1)
            {
                return fileIdMatch.Groups[1].Value;
            }
            return null;
        }


        private byte[] DownloadFileFromUrl(string url)
        {
            using var httpClient = new HttpClient();
            var response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsByteArrayAsync().Result;
            }
            return null;
        }

    }
}
