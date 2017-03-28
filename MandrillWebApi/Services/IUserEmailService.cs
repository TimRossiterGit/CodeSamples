using bringpro.Web.Models;
using bringpro.Web.Models.Requests.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace bringpro.Web.Services.Interfaces
{
    public interface IUserEmailService
    {
        //SendGrid & Mandrill Signatures
        void SendProfileEmail(Guid? Token, string Email);
        void SendAdminProfileEmail(Guid? Token, string Email);
        void sendContactRequestEmail(EmailRequestModel crModel);
        void ResetPasswordEmail(Guid Token, string Email, string Slug);
        void ReferralEmail(Guid Token, string Email);
        void SendEmail(MandrillRequestModelTest model);


    }
}