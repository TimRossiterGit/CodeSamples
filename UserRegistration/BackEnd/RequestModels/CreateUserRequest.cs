using bringpro.Web.Models.Requests.Bringg;
using bringpro.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bringpro.Web.Models.Requests.Users
{    //create user request model - inherit from bringg request model
    public class CreateUserRequest: BaseBringgRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ExternalUserId { get; set; }

        public string TokenHash { get; set; }

        public string Slug { get; set; }

    }

}