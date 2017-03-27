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
    public class MandrillService : BaseService, IUserEmailService
    {
        public void ReferralEmail(Guid Token, string Email)
        {

            //Email = ConfigService.MandrillTestEmail;

            var SendReferral = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Timmy" } },
                FromEmail = ConfigService.MandrillFromEmail,
                //Html = "Your friend has invited you to try bringpro. Use their referral code to get 25% off your first order!" + ConfigService.SiteBaseUrl + "/Public/referralRegister/" + Token,
                Html = "Your friend has invited you to try bringpro. Use their referral code to get 25% off your first order! " + "http://bringpro.dev/bringpro/authentication/" + Token,
                Subject = "Invitation to try bringpro"
            });


            var api = new MandrillApi(ConfigService.MandrillApiKey);
            //api.SendMessage(SendReferral);

            Task.Run(async () => await api.SendMessage(SendReferral));
        }

        public void ResetPasswordEmail(Guid Token, string Email, string Slug)
        {
            //Email = ConfigService.MandrillTestEmail;

            var GetNewPassword = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Timmy" } },
                FromEmail = ConfigService.MandrillFromEmail,
                Html = "Click this link to reset your password: " + "http://bringpro.dev" + "/" + Slug + "/passwordauthentication/" + Token, //<<<<<<CHANGE BACK TO THIS when you are done testing>>>> GetNewPassword.Text = "Click this link to reset your password: " + ConfigService.SiteBaseUrl  + "/" + Slug + "/passwordauthentication/" + Token; 
                Subject = "Reset your bringpro password."
            });


            var api = new MandrillApi(ConfigService.MandrillApiKey);
            //api.SendMessage(GetNewPassword);

            Task.Run(async () => await api.SendMessage(GetNewPassword));
        }

        public void SendAdminProfileEmail(Guid? Token, string Email)
        {
            //Email = ConfigService.MandrillTestEmail;

            var ActivateUserEmail = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Timmy" } },
                FromEmail = ConfigService.MandrillFromEmail,
                //Html = "Activate Account Mandrill Test. Click this link to activate your account " + ConfigService.SiteBaseUrl + "/public/authentication/" + Token,
                Html = "Activate Account Mandrill Test. Click this link to activate your account " + "http://bringpro.dev/bringpro/authentication/" + Token,
                Subject = "Activate your bringpro account.",
                //Text = "Click this link to activate your account " + ConfigService.SiteBaseUrl + " /public/authentication/" + Token
            });


            var api = new MandrillApi(ConfigService.MandrillApiKey);
            // api.SendMessage(ActivateUserEmail);

            Task.Run(async () => await api.SendMessage(ActivateUserEmail));

         
        }

        public void sendContactRequestEmail(EmailRequestModel crModel)
        {
            string renderedHTML = EmailService.RenderRazorViewToString("~/Views/TestEmail/Index.cshtml", crModel);

            //crModel.Email = ConfigService.MandrillTestEmail;

            var ActivateCrEmail = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = crModel.Email, Name = "Timmy" } },
                FromEmail = ConfigService.MandrillFromEmail,
                Html = renderedHTML,
                Subject = "Your bringpro input has been successfully submitted!"
            });
            //ActivateCrEmail.Text = "We appreciate your feedback and will contact you within a few days for support.";

            var api = new MandrillApi(ConfigService.MandrillApiKey);
            //api.SendMessage(ActivateCrEmail);

            Task.Run(async () => await api.SendMessage(ActivateCrEmail));
        }

        //Test
        public void SendEmail(MandrillRequestModelTest model)
        {
            //Setup Test
            model.apiKey = ConfigService.MandrillApiKey;
            model.toEmail = ConfigService.MandrillTestEmail;
            model.fromEmail = ConfigService.MandrillFromEmail;

            //the Api
            var api = new MandrillApi(model.apiKey);


            var sendMessageRequest = new SendMessageRequest(new EmailMessage
            {
                //make it work??

                To =
                new List<EmailAddress> { new EmailAddress { Email = model.toEmail, Name = "Timmy" } },
                FromEmail = model.fromEmail,
                Subject = "Mandrill Integration Test",
                Html = "<strong>Scheduled Email. anna is the best.</strong>",
                Text = "Example text"
            });

            //List<EmailResult> result = await api.SendMessage(sendMessageRequest);

            Task.Run(async () => await api.SendMessage(sendMessageRequest));

        }

        public void SendProfileEmail(Guid? Token, string Email)
        {
            //Email = ConfigService.MandrillTestEmail;

            var ActivateUserEmail = new SendMessageRequest(new EmailMessage
            {
                To =
                new List<EmailAddress> { new EmailAddress { Email = Email, Name = "Timmy" } },
                FromEmail = ConfigService.MandrillFromEmail,
                //Html = "Activate Account Mandrill Test. Click this link to activate your account " + ConfigService.SiteBaseUrl + "/bringpro/authentication/" + Token,
                Html = "Activate Account Mandrill Test. Click this link to activate your account " + "http://bringpro.dev/bringpro/authentication/" + Token,
                Subject = "Activate your bringpro account.",

                //Text =  "Click this link to activate your account " + ConfigService.SiteBaseUrl + " /public/authentication/" + Token
            });


            var api = new MandrillApi(ConfigService.MandrillApiKey);
           // api.SendMessage(ActivateUserEmail);

            Task.Run(async () => await api.SendMessage(ActivateUserEmail));
        }
    }
}