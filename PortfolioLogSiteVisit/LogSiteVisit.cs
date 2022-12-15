using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace PortfolioLogSiteVisit
{
    public static class LogSiteVisit
    {
        [FunctionName("LogSiteVisit")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string pw = Environment.GetEnvironmentVariable("App_Pw");
            string message = "Someone just visited your profile.";
            string emailFrom = Environment.GetEnvironmentVariable("Email_From");
            string emailTo = Environment.GetEnvironmentVariable("Email_To");

            // Send email
            MailMessage emailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            emailMessage.From = new MailAddress(emailFrom);
            emailMessage.To.Add(new MailAddress(emailTo));
            //emailMessage.IsBodyHtml = true;
            emailMessage.Body = message.ToString();

            emailMessage.Subject = "Site Visit Notification - Dibendu Saha";
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(emailTo, pw);

            smtpClient.Send(emailMessage);

            return new OkResult();
        }
    }
}
