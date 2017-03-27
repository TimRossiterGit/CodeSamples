using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using bringpro.Web.Models.Responses;
using bringpro.Web.Services;
using bringpro.Web.Models.Requests.Users;
using Microsoft.AspNet.Identity.EntityFramework;
using bringpro.Web.Models;
using bringpro.Web.Domain;
using bringpro.Web.Exceptions;
using Microsoft.AspNet.Identity;
using bringpro.Web.Models.Requests;
using bringpro.Web.Enums;
using bringpro.Web.Classes.Tasks.Bringg;
using bringpro.Web.Models.Requests.Bringg;
using Microsoft.Practices.Unity;
using bringpro.Web.Services.Interfaces;
using bringpro.Web.Models.ViewModels;

namespace bringpro.Web.Controllers.Api
{
    [RoutePrefix("public/users")]
    public class PublicApiController : ApiController //inheritance from apicontroller
    {
        //dependency injections for access to email and user profile services
        [Dependency]
        public IUserEmailService _EmailService { get; set; }
        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }

        //register new user
        [Route("add"), HttpPost] // inserting the create user request model
        public HttpResponseMessage RegisterNewUser(CreateUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            // if email/phone number taken, return bad request.
            // checks for a null websiteId via slug
            bool checkUser = _UserProfileService.GetUserByEmailAndPhoneNumber(model);
            string thisSlug = model.Slug;
            var checkWebsite = WebsiteService.GetWebsiteIdBySlug(thisSlug);

            if (checkUser == true && checkWebsite == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);              
            }
            // if user has not already been registered run user service to create new user
            else
            {
                ApplicationUser newUser = null;
                try
                {   // submit users email, password, and phone number for registration
                    newUser = (ApplicationUser)UserService.CreateUser(model.Email, model.Password, model.Phone);
                }
                catch (IdentityResultException ex)
                {   // catches exceptions returns bad request         
                    string validatedData = UtilityService.ValidateData(ex);
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, validatedData, ex);
                }
                //send User to database
                //only run if there are no duplicate phone/email 
                //NOTHING SHOULD GET INTO THE DATABASE IF THERE IS A 400 ERROR
                if (newUser != null)
                {    // passing user model to service
                    _UserProfileService.CreateUserProfile(newUser.Id, model);
                }
                ItemResponse<UserProfile> response = new ItemResponse<UserProfile>();
                // return the newly created user profile
                response.Item = (UserProfile)_UserProfileService.GetUserById(newUser.Id);      

                return Request.CreateResponse(response);
            }
        }

        [Route("passwordReset"), HttpPost]
        public HttpResponseMessage GenerateTokenForPasswordReset(PasswordResetAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //UserProfile user = UserProfileService.GetUserByEmail(model.Email);
            UserProfile user = _UserProfileService.GetUserByPhoneNumber(model.Phone);
            
            if (user == null)
            {
                //(400 bad request)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This e-mail cannot be found");
                //also show the user a view that says e-mail cannot be found and try again
            }

            Guid userTokenGuid = TokenService.tokenInsert(user.UserId);

            //_EmailService.ResetPasswordEmail(userTokenGuid, model.Email);
            _EmailService.ResetPasswordEmail(userTokenGuid, user.Email, model.Slug);

            //ItemResponse<UserProfile> response = new ItemResponse<UserProfile>();

            return Request.CreateResponse(HttpStatusCode.OK, "Mission Success");

        }

        [Route("passwordReset/update"), HttpPost]
        public HttpResponseMessage GetUserIdByToken(PasswordResetGetUserId model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            Token token = TokenService.userGetByGuid(model.TokenHash);

            bool result = UserService.ChangePassWord(token.UserId, model.Password);

            if (!result)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter a valid password. Your password must be between five and 20 characters and contain: one special character, number, lower-case letter, and upper-case letter.");
            }


            return Request.CreateResponse(HttpStatusCode.OK, "Password Updated Successfully");
        }


        [Route("confirmSMS/{tokenSMS}"), HttpGet]
        public HttpResponseMessage GetUserByTokenSMS(string tokenSMS)
        {
            TokenSMS UserInfo = null;
            bool isSuccessful = false;

            //Get User Info By Token Entered
            UserInfo = TokenSMSService.UserGetByToken(tokenSMS);

            if (UserInfo.Used != null || UserInfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Verification Code Error.  Please Make Sure to enter carefully.  Re-send if necessary.");
            }

            isSuccessful = TokenSMSService.TokenSMSUpdateUsed(UserInfo.UserId, tokenSMS);

            return Request.CreateResponse(isSuccessful);

        }

        [Route("confirmSMS/{UserId}"), HttpPut]
        public HttpResponseMessage GetNewToken(string UserId)
        {
            // - Validate incoming payload model
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<TokenSMS> response = new ItemResponse<TokenSMS>();

            response.Item = TokenSMSService.UserGetByNewToken(UserId);

            //Service to grab user Phone#
            UserProfile UserInfo = _UserProfileService.GetUserById(UserId);
            String UserPhone = UserInfo.Phone;

            //Service to Send New Text with existing Model
            NotifySMSRequest RequestText = new NotifySMSRequest();
            RequestText.Phone = UserPhone;
            RequestText.TokenSMS = response.Item.TokenHash;
            object NewTextResult = NotifySMSService.SendConfirmText(RequestText);

            return Request.CreateResponse(NewTextResult);

        }
    }
}
