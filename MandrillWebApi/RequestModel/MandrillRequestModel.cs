using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bringpro.Web.Models.Requests
{
    //Mandrill Request Model
    //Used for testing Mandrill
    public class MandrillRequestModel
    {
        public string apiKey { get; set; }

        public string toEmail { get; set; }

        public string fromEmail { get; set; }
    }
}