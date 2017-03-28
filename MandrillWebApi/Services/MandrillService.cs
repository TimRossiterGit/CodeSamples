using bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bringpro.Web.Models;
using System.Configuration;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using System.Threading.Tasks;
using bringpro.Web.Models.Requests.Tests;
using System.Net.Mail;

namespace bringpro.Web.Services
{
    //Mandrill Service - Implements Interface for Email sErvice
    public class MandrillService : BaseService, IUserEmailService
    {
        //Sending an email based on customer referral
        public void ReferralEmail(Guid Token, string Email)
        {
            //the message payload
            var SendReferral = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Bringpro" } },
                FromEmail = ConfigService.MandrillFromEmail,               
                Html = "Your friend has invited you to try bringpro. Use their referral code to get 25% off your first order! " + "http://bringpro.dev/bringpro/authentication/" + Token,
                Subject = "Invitation to try bringpro"
            });
            //sets api key to api key stored in config file - makes easily changeable
            var api = new MandrillApi(ConfigService.MandrillApiKey);

            //Mandrill Requires functions to be set as async.
            //This line of code lets us run void functions asynchronously
            //Purpose - to match interface signatures with sendgrid interface
            //Will be in ALL Mandrill Functions
            Task.Run(async () => await api.SendMessage(SendReferral));
        }
        //Sending an Email to reset a customers password - based on website(slug)
        public void ResetPasswordEmail(Guid Token, string Email, string Slug)
        {
            //email payload
            var GetNewPassword = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Bringpro" } },
                FromEmail = ConfigService.MandrillFromEmail,
                Html = "Click this link to reset your password: " + "http://bringpro.dev" + "/" + Slug + "/passwordauthentication/" + Token, 
                Subject = "Reset your bringpro password."
            });
            //sets api key to api key stored in config file - makes easily changeable
            var api = new MandrillApi(ConfigService.MandrillApiKey);

            //Mandrill Requires functions to be set as async.
            //This line of code lets us run void functions asynchronously
            //Purpose - to match interface signatures with sendgrid interface
            //Will be in ALL Mandrill Functions
            Task.Run(async () => await api.SendMessage(GetNewPassword));
        }

        public void SendAdminProfileEmail(Guid? Token, string Email)
        {
            //email payload
            var ActivateUserEmail = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Bringpro" } },
                FromEmail = ConfigService.MandrillFromEmail,
  
                Html = "Activate Account Mandrill Test. Click this link to activate your account " + "http://bringpro.dev/bringpro/authentication/" + Token,
                Subject = "Activate your bringpro account.",
  
            });
            //sets api key to api key stored in config file - makes easily changeable
            var api = new MandrillApi(ConfigService.MandrillApiKey);

            //Mandrill Requires functions to be set as async.
            //This line of code lets us run void functions asynchronously
            //Purpose - to match interface signatures with sendgrid interface
            //Will be in ALL Mandrill Functions
            Task.Run(async () => await api.SendMessage(ActivateUserEmail));

         
        }

        public void sendContactRequestEmail(EmailRequestModel crModel)
        {
            string renderedHTML = EmailService.RenderRazorViewToString("~/Views/TestEmail/Index.cshtml", crModel);
            //email payload
            var ActivateCrEmail = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = crModel.Email, Name = "Bringpro" } },
                FromEmail = ConfigService.MandrillFromEmail,
                Html = renderedHTML,
                Subject = "Your bringpro input has been successfully submitted!"
            });
            //sets api key to api key stored in config file - makes easily changeable
            var api = new MandrillApi(ConfigService.MandrillApiKey);

            //Mandrill Requires functions to be set as async.
            //This line of code lets us run void functions asynchronously
            //Purpose - to match interface signatures with sendgrid interface
            //Will be in ALL Mandrill Functions
            Task.Run(async () => await api.SendMessage(ActivateCrEmail));
        }

      

        public void SendProfileEmail(Guid? Token, string Email)
        {
            //email payload
            var ActivateUserEmail = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Bringpro" } },
                FromEmail = ConfigService.MandrillFromEmail,
          
                Html = "Activate Account Mandrill Test. Click this link to activate your account " + "http://bringpro.dev/bringpro/authentication/" + Token,
                Subject = "Activate your bringpro account.",
                
            });
            //sets api key to api key stored in config file - makes easily changeable
            var api = new MandrillApi(ConfigService.MandrillApiKey);

            //Mandrill Requires functions to be set as async.
            //This line of code lets us run void functions asynchronously
            //Purpose - to match interface signatures with sendgrid interface
            //Will be in ALL Mandrill Functions
            Task.Run(async () => await api.SendMessage(ActivateUserEmail));
        }

        //Test
        public void SendEmail(MandrillRequestModelTest model)
        {
            //Setup Test
            model.apiKey = ConfigService.MandrillApiKey;
            model.toEmail = ConfigService.MandrillTestEmail;
            model.fromEmail = ConfigService.MandrillFromEmail;

            //sets api key to api key stored in config file - makes easily changeable
            var api = new MandrillApi(model.apiKey);

            //mandrill payload
            var sendMessageRequest = new SendMessageRequest(new EmailMessage
            {           
                To =
                new List<EmailAddress> { new EmailAddress { Email = model.toEmail, Name = "Bringpro" } },
                FromEmail = model.fromEmail,
                Subject = "Mandrill Integration Test",
                Html = "<strong>Scheduled Email.</strong>",
                Text = "Example text"
            });

            Task.Run(async () => await api.SendMessage(sendMessageRequest));

        }
    }
}