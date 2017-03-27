using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Microsoft.Practices.Unity;
using bringpro.Web.Models;
using bringpro.Web.Models.Requests.Tests;
using bringpro.Web.Models.Responses;
using bringpro.Web.Services;
using bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace bringpro.Web.Controllers.Api
{
    [RoutePrefix("api/mandrill")]
    public class MandrillApiController : ApiController
    {
        //USE FOR TESTING MANDRILL

        //might need dependency
        [Dependency]
        public IUserEmailService _MandrillService { get; set; }

        [Route("test"), HttpPost]
        public HttpResponseMessage SendEmail(MandrillRequestModelTest model)
        {
           

             //_MandrillService.ResetPasswordEmail(Token, Email, "bringpro");

            _MandrillService.SendEmail(model);

            //return Request.CreateResponse(HttpStatusCode.OK, response);

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            return Request.CreateResponse(HttpStatusCode.OK, "bringpro");


        }
        

   
    }
}

