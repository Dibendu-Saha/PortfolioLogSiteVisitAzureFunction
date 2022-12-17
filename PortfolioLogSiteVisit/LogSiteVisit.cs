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
using PortfolioLogSiteVisit.Models;
using System.Text;

namespace PortfolioLogSiteVisit
{
    public static class LogSiteVisit
    {
        [FunctionName("LogSiteVisit")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string page = req.Query["page"].ToString();
            string loc_data_raw = req.Query["loc_data"].ToString();
            LocationData locData = JsonConvert.DeserializeObject<LocationData>(loc_data_raw);

            string pw = Environment.GetEnvironmentVariable("App_Pw");

            string tblLocationDetails;
            if (locData != null)
                tblLocationDetails
                    = "<hr><div>"
                    + "<p>Location details:</p>"
                    + "<table border=\"1\">"
                    + $"<tr> <td><strong>Country Code</strong></td> <td>{locData.Country_Code}</td> </tr>"
                    + $"<tr> <td><strong>Country</strong></td> <td>{locData.Country_Name}</td> </tr>"
                    + $"<tr> <td><strong>State</strong></td> <td>{locData.State}</td> </tr>"
                    + $"<tr> <td><strong>City</strong></td> <td>{locData.City}</td> </tr>"
                    + $"<tr> <td><strong>Postal</strong></td> <td>{locData.Postal}</td> </tr>"
                    + $"<tr> <td><strong>Latitude</strong></td> <td>{locData.Latitude}</td> </tr>"
                    + $"<tr> <td><strong>Longitude</strong></td> <td>{locData.Longitude}</td> </tr>"
                    + $"<tr> <td><strong>IPv4</strong></td> <td>{locData.IPv4}</td> </tr>"
                    + "</table></div><hr>";
            else
                tblLocationDetails = "<hr><div><p>Location data not available.</p></div>";

            StringBuilder message = new StringBuilder();
            message.Append(
                page.Contains("download")
                    ? "<p>Visitor just downloaded your CV.</p>"
                    : page.Contains("click")
                        ? $"<p>Visitor just clicked on your social icon ({page.Split("_")[2]}).</p>"
                        : "<p>Someone just visited your website.</p>"
                             + $"<p>Website section: <strong>{page}</strong></p>"
                );


            message.Append(tblLocationDetails);

            string emailFrom = Environment.GetEnvironmentVariable("Email_From");
            string emailTo = Environment.GetEnvironmentVariable("Email_To");

            // Send email
            MailMessage emailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            emailMessage.From = new MailAddress(emailFrom);
            emailMessage.To.Add(new MailAddress(emailTo));
            emailMessage.IsBodyHtml = true;
            emailMessage.Body = message.ToString();

            emailMessage.Subject =
                page.Contains("download")
                    ? "CV Download Notification - Dibendu Saha"
                    : page.Contains("click")
                        ? $"Social Logo Click Notification - Dibendu Saha ({page.Split("_")[2]})"
                        : $"Site Visit Notification - Dibendu Saha ({page})";

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
