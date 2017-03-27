using Microsoft.Practices.Unity;
using bringpro.Data;
using bringpro.Web.Classes.Tasks.Bringg.Interfaces;
using bringpro.Web.Domain;
using bringpro.Web.Enums;
using bringpro.Web.Models.Requests;
using bringpro.Web.Models.Requests.Bringg;
using bringpro.Web.Models.Requests.Users;
using bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace bringpro.Web.Services
{
    public class UserProfileService : BaseService, IUserProfileService //inherit from base service and DI userprofile service
    {
        [Dependency]
        public IAdminUserService _AdminService { get; set; }

        [Dependency]
        public IUserCreditsService _CreditsService { get; set; }
        [Dependency]
        public IUserEmailService _EmailService { get; set; }
        [Dependency("CreateCustomerTask")]
        public IBringgTask<RegisterBringgRequest> _CreateCustomerTask { get; set; }
        [Dependency]
        public IActivityLogService _ActivityLogService { get; set; }
        [Dependency]
        public IWebsiteService _WebsiteService { get; set; }

        //Service ran for creating a new user - pass in users id and request model
        public void CreateUserProfile(string userId, CreateUserRequest model)
        {
            //Updated the create user so that it can take in the tokenHash param if there is one provided
            DataProvider.ExecuteNonQuery(GetConnection, "UserProfiles_Insert" // stored procedure
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {    //input fields
                    paramCollection.AddWithValue("@UserId", userId);
                    paramCollection.AddWithValue("@FirstName", model.FirstName);
                    paramCollection.AddWithValue("@LastName", model.LastName);
                    paramCollection.AddWithValue("@TokenHash", model.TokenHash); //this is provided if they were referred by a current customer 

                }
                );
            //referral coupon
            //if TokenHash referral is null, they're making a new account.
            //if TokenHash referral is NOT null, we want to give them (new user) a Coupon value and add it to their Credit.
            // we also want to give the original user (one who referred) the same Coupon value and add it to their Credit.
            if (model.TokenHash != null)
            {
                //retrieve coupon information + token information based on the token hash.
                CouponsDomain userCoupon = TokenService.GetReferralTokenByGuid(model.TokenHash);
                if (userCoupon.Token.Used == null)
                {
                    //send token to Credit service 
                    UserCreditsRequest insertRefferalCredits = new UserCreditsRequest();
                    UserCreditsRequest insertFriendCredits = new UserCreditsRequest();

                    //send credits to new User who was referred
                    insertRefferalCredits.Amount = userCoupon.CouponValue;
                    insertRefferalCredits.TransactionType = "Add";
                    insertRefferalCredits.UserId = userId;
                    _CreditsService.InsertUserCredits(insertRefferalCredits);

                }

                else
                {
                    //send error message that token has already been used.
                }
            }

            //Activity Services
            ActivityLogRequest Activity = new ActivityLogRequest();
            Activity.ActivityType = ActivityTypeId.NewAccount;
            _ActivityLogService.InsertActivityToLog(userId, Activity);

            //----------//create a new Website Id ---------updated the AddUserToWebsite using the updated domain with int[]   
            //getting the websiteId via the website Slug
            Website w = null;
            string Slug = model.Slug;
            w = WebsiteService.GetWebsiteIdBySlug(Slug);

            int[] WebsiteIds = new int[1];
            WebsiteIds[0] = w.Id;
            //populating userwebsite object
            UserWebsite userWebsite = new UserWebsite();
            userWebsite.UserId = userId;
            userWebsite.WebsiteIds = WebsiteIds;
            WebsiteService.AddUserToWebsite(userWebsite);

            //creae a new Customer role
            //set role as customer by default - change role by admin panel
            UserProfile aspUser = new UserProfile();
            aspUser.FirstName = model.FirstName;
            aspUser.LastName = model.LastName;
            aspUser.RoleId = ConfigService.CustomerRole;
            _AdminService.CreateUserRole(userId, aspUser);

            //create a new Braintree account using UserID
            //braintree used for handling credit card transactions
            CustomerPaymentRequest Payment = new CustomerPaymentRequest();
            Payment.FirstName = model.FirstName;
            Payment.LastName = model.LastName;
            Payment.UserId = userId;
            Payment.Phone = model.Phone;

            //Send a confirmation Text Msg
            string UserSMSToken = TokenSMSService.TokenSMSInsert(userId);
            //send a text msg
            NotifySMSRequest NotifyCustomer = new NotifySMSRequest();
            NotifyCustomer.Phone = model.Phone;
            NotifyCustomer.TokenSMS = UserSMSToken;

            try
            {
                NotifySMSService.SendConfirmText(NotifyCustomer);

            }
            catch (ArgumentException ex)
            {     
                //if phone number is already registered will not send a registration check
                //should never get this far
                throw new System.ArgumentException(ex.Message);
            }       
     
            //bringg create account
            RegisterBringgRequest bringgRequest = new RegisterBringgRequest();
            bringgRequest.Name = model.FirstName + " " + model.LastName;
            bringgRequest.Phone = model.Phone;
            bringgRequest.Email = model.Email;
            bringgRequest.UserId = userId;
            this._CreateCustomerTask.Execute(bringgRequest);

            _CreateCustomerTask.Execute(bringgRequest);

            BrainTreeService brainTree = new BrainTreeService();

            brainTree.newCustomerInsert(Payment);

            ////generate a new token
            Guid userTokenGuid = TokenService.tokenInsert(userId);

            //send a confirmation email
            _EmailService.SendProfileEmail(userTokenGuid, model.Email);
        }

        public UserProfile GetUserById(string userId)
        {
            UserProfile c = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.AdminUserBy_Id_v4"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@userId", userId);
              }, map: delegate (IDataReader reader, short set)
              {
                  c = new UserProfile();
                  int startingIndex = 0;
                  c.UserProfileId = reader.GetSafeInt32(startingIndex++);
                  c.UserId = reader.GetSafeString(startingIndex++);
                  c.FirstName = reader.GetSafeString(startingIndex++);
                  c.LastName = reader.GetSafeString(startingIndex++);
                  c.ExternalUserId = reader.GetSafeString(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.DateModified = reader.GetSafeDateTime(startingIndex++);
                  c.MediaId = reader.GetSafeInt32(startingIndex++);
                  c.Email = reader.GetSafeString(startingIndex++);
                  c.Phone = reader.GetSafeString(startingIndex++);


                  c.Role.RoleId = reader.GetSafeString(startingIndex++);
                  c.Role.Name = reader.GetSafeString(startingIndex++);

                  //u.RoleId = reader.GetString(startingIndex++);
                  //u.Name = reader.GetString(startingIndex++);


                  Media m = new Media();

                  m.Id = reader.GetSafeInt32(startingIndex++);
                  m.DateModified = reader.GetSafeDateTime(startingIndex++);
                  m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  m.Url = reader.GetSafeString(startingIndex++);
                  m.MediaType = reader.GetSafeInt32(startingIndex++);
                  m.UserId = reader.GetSafeString(startingIndex++);
                  m.Title = reader.GetSafeString(startingIndex++);
                  m.Description = reader.GetSafeString(startingIndex++);
                  m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                  m.FileType = reader.GetSafeString(startingIndex++);


                  if (m.Id != 0)
                  {
                      c.Media = m;
                  }

              }

           );
            return c;
        }

        public UserProfile GetUserByEmail(string Email)
        {
            UserProfile c = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfiles_SelectByEmail"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Email", Email);

              }, map: delegate (IDataReader reader, short set)
              {
                  c = new UserProfile();
                  int startingIndex = 0;
                  c.UserProfileId = reader.GetSafeInt32(startingIndex++);
                  c.UserId = reader.GetSafeString(startingIndex++);
                  c.FirstName = reader.GetSafeString(startingIndex++);
                  c.LastName = reader.GetSafeString(startingIndex++);
                  c.ExternalUserId = reader.GetSafeString(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.Email = reader.GetSafeString(startingIndex++);
                  c.Phone = reader.GetSafeString(startingIndex++);
              }
           );
            return c;
        }

        //DF
        public UserProfile GetUserByPhoneNumber(string PhoneNumber)
        {
            UserProfile c = null;
            List<Website> Websites = new List<Website>();
            
         

            DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfiles_SelectByPhoneNumber_V2"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@PhoneNumber", PhoneNumber);
                 


              }, map: delegate (IDataReader reader, short set)
              {
                  if (set == 0)
                  {
                      c = new UserProfile();
                      int startingIndex = 0;
                      c.UserProfileId = reader.GetSafeInt32(startingIndex++);
                      c.UserId = reader.GetSafeString(startingIndex++);
                      c.FirstName = reader.GetSafeString(startingIndex++);
                      c.LastName = reader.GetSafeString(startingIndex++);
                      c.ExternalUserId = reader.GetSafeString(startingIndex++);
                      c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      c.Email = reader.GetSafeString(startingIndex++);
                      c.Phone = reader.GetSafeString(startingIndex++);
                      c.RoleId = reader.GetSafeString(startingIndex++);
                      c.Role.Name = reader.GetSafeString(startingIndex++);
                  }
                  else if (set == 1)
                  {
                      Website websiteData = new Website();
                      int startingIndex = 0;
                      websiteData.Id = reader.GetSafeInt32(startingIndex++);
                      websiteData.Name = reader.GetSafeString(startingIndex++);
                      websiteData.Slug = reader.GetSafeString(startingIndex++);
                      websiteData.Description = reader.GetSafeString(startingIndex++);
                      websiteData.Url = reader.GetSafeString(startingIndex++);
                      Websites.Add(websiteData);

                  }
              
              }
           );
            c.Websites = Websites;
            return c;
        }

        public void UpdateProfileTable(string userId, CreateUserRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "UserProfiles_UpdateProfileTable"
                 , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                 {
                     paramCollection.AddWithValue("@UserId", userId);
                     paramCollection.AddWithValue("@FirstName", model.FirstName);
                     paramCollection.AddWithValue("@LastName", model.LastName);

                 }
                 );


            string ExternalId = GetExternalId(userId);
            RegisterBringgRequest bringgRequest = new RegisterBringgRequest();
            bringgRequest.Name = model.FirstName + " " + model.LastName;
            bringgRequest.Phone = model.Phone;
            bringgRequest.Email = model.Email;
            bringgRequest.UserId = userId;
            bringgRequest.Id = ExternalId;
            //UpdateCustomerTask<RegisterBringgRequest> Task = new UpdateCustomerTask<RegisterBringgRequest>();
            //IBringgTask<RegisterBringgRequest> _UpdateCustomerTask = UnityConfig.GetContainer().Resolve<IBringgTask<RegisterBringgRequest>>();
            _CreateCustomerTask.Execute(bringgRequest);
        }
        public void UpdateAspNetUserTable(string userId, CreateUserRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "UserProfiles_UpdateAspUserTable"
                 , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                 {
                     paramCollection.AddWithValue("@UserId", userId);
                     paramCollection.AddWithValue("@Email", model.Email);
                     paramCollection.AddWithValue("@PhoneNumber", model.Phone);

                 }
                 );
        }

        //update by UserProfile Media Id
        public void UpdateUserMediaId(string userId, int mediaId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserProfiles_UpdateMediaId",
                inputParamMapper: delegate (SqlParameterCollection paramColection)
                {
                    paramColection.AddWithValue("@UserId", userId);
                    paramColection.AddWithValue("@MediaId", mediaId);
                });

        }


        public string GetExternalId(string UserId)
        {
            UserProfile c = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfiles_GetExternalId"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", UserId);

              }, map: delegate (IDataReader reader, short set)
              {
                  c = new UserProfile();
                  int startingIndex = 0;
                  c.ExternalUserId = reader.GetSafeString(startingIndex++);
              }
           );
            return c.ExternalUserId;
        }

        public List<UserWebsite> GetWebsiteId()
        {
            List<UserWebsite> userWebsite = new List<UserWebsite>();
            DataProvider.ExecuteCmd(GetConnection, "dbo.UserWebsite_SelectAll"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {

                }, map: delegate (IDataReader reader, short set)
                {
                    UserWebsite uw = new UserWebsite();

                    int startingIndex = 0;

                    uw.UserId = reader.GetSafeString(startingIndex++);
                    uw.WebsiteId = reader.GetSafeInt32(startingIndex++);

                    userWebsite.Add(uw);
                }

                );

            return userWebsite;

        }

        public bool GetUserByEmailAndPhoneNumber(CreateUserRequest model)
        {
            bool existingUser = false;
            AspNetUsersDomain p = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.AspNetUsers_SelectByEmailAndPhone"
                   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                   {
                       paramCollection.AddWithValue("@PhoneNumber", model.Phone);
                       paramCollection.AddWithValue("@Email", model.Email);

                   }, map: delegate (IDataReader reader, short set)
                   {
                       int startingIndex = 0;
                       p = new AspNetUsersDomain();
                       p.Id = reader.GetSafeString(startingIndex++);
                       p.Email = reader.GetSafeString(startingIndex++);
                       p.EmailConfirmed = reader.GetSafeBool(startingIndex++);
                       p.PasswordHash = reader.GetSafeString(startingIndex++);
                       p.SecurityStamp = reader.GetSafeString(startingIndex++);
                       p.PhoneNumber = reader.GetSafeString(startingIndex++);
                       p.PhoneNumberConfirmed = reader.GetSafeBool(startingIndex++);
                       p.TwoFactorEnabled = reader.GetSafeBool(startingIndex++);
                       p.LockoutEndDateUtc = reader.GetSafeDateTime(startingIndex++);
                       p.LockoutEnabled = reader.GetSafeBool(startingIndex++);
                       p.AccessFailedCount = reader.GetSafeInt32(startingIndex++);
                       p.UserName = reader.GetSafeString(startingIndex++);

                       if (p == null)
                       {
                           existingUser = false;

                       }
                       else if (p != null)
                       {
                           existingUser = true;
                       }

                   });

            return existingUser;
        }

        public void DeleteUserProfileByUserId(string Id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserProfiles_DeleteByUserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", Id);
                });
        }

        public void DeleteUserWebsiteByUserId(string Id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserWebsite_DeleteByUserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", Id);
                });
        }

        public void DeleteAspNetUserByUserId(string Id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_DeleteByUserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", Id);
                });
        }

        public string GetTokenHashByUserId(string UserId)
        {

            string TokenHash = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.UserProfiles_GetTokenHashByUserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", UserId);

                }, map: delegate (IDataReader reader, short set)
                {

                    int startingIndex = 0;
                    TokenHash = reader.GetSafeString(startingIndex++);

                });

            return TokenHash;

        }
    }


}
