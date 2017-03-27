using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bringpro.Web.Models.Requests.Tests
{
    public class MandrillRequestModelTest
    {
        public string apiKey { get; set; }

        public string toEmail { get; set; }

        public string fromEmail { get; set; }
    }
}