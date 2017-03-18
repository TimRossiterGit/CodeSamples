using Microsoft.Practices.Unity;
using Bringpro.Web.Domain;
using Bringpro.Web.Enums;
using Bringpro.Web.Models.ViewModels;
using Bringpro.Web.Services;
using Bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Bringpro.Web.Classes.Filter.Api;

namespace Bringpro.Web.Controllers
{
    [WebsiteActionFilter] //this is like the [Authorize] filter but custom made.
    public class WebsiteController : BaseController
    {
        //dependency from dashboard controller
        [Dependency]
        public IBrainTreeService _BrainTreeService { get; set; }

        [Dependency]
        public IWebsiteService _WebsiteService { get; set; }

        //// GET: Website by Slug
        [Route()]
        [Route("{Slug}/home")]
        public ActionResult Index(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);
          
            //access the same httpcontext
            vm.Website = (Website)Request.RequestContext.RouteData.Values["Website"];

            return View(vm);
        }

        //// GET: Website LOGIN by Slug
        [Route("login")]
        [Route("{Slug}/login")]
        public ActionResult LoginFrontEnd(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            return View(vm);
        }

        [Route("{Slug}/Register/{TokenHash:guid}")]
        [Route("{Slug}/Register")]
        public ActionResult RegisterFrontEnd(string Slug, string TokenHash = null)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            vm.TokenHash = TokenHash;

            return View(vm);
        }

        [Route("{Slug}/passwordreset")]
        public ActionResult PasswordResetFrontEnd(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            return View(vm);
        }

        //// GET: Website passwrdreset by Slug
        [Route("{Slug}/passwordauthentication/{emailGuid:guid}")]
        public ActionResult PasswordChangeConfirmFrontEnd(Guid emailGuid, string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            //For the Email Token
            Token userToken = null;
            userToken = TokenService.userGetByGuid(emailGuid);

            string tokenhash = userToken.TokenHash.ToString();
            TokenService.tokenUsedUpdate(userToken.UserId, tokenhash);

            return View(vm);
        }

        private WebsiteViewModel _GetViewModel(string Slug="bringpro")
        {
            if (Slug == null)
            {
                Slug = "bringpro";
            }
            WebsiteViewModel vm = new WebsiteViewModel();
            vm.Slug = Slug;
            vm.CategoryEnum = SettingsCategory.String;
            vm.SettingTypeEnum = SettingsType.Design;
            vm.SettingSectionEnum = SettingsSection.Layout;

            WebsiteSettingsServices websiteService = new WebsiteSettingsServices();
            List<WebsiteSettings> WebsiteBySlug = websiteService.GetSettingsBySlug(Slug);
            vm.Settings = WebsiteBySlug;
            if (vm.Settings.Count < 1 && Slug != "backoffice")
            {
                throw new HttpException(404, "Website Does Not Exist");
            }

            if (Slug != null && Slug != "")
            {
                vm.Website = WebsiteService.websiteGetBySlug(Slug);
            }

            return vm;
        }

        //DASHBOARD CONTROLLERS FOR CUSTOMERS
        // GET: Dashboard

        [InvoiceAuthorizationFilter]
        [Route("{Slug}/dashboard")]
        public ActionResult DashboardIndex(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            vm.ActivityTypes = ActivityTypeId.NewAccount;
            vm.Token = _BrainTreeService.CreateToken();
            return View("DashboardHome", vm);
        }

        //logout controller
        [Route("{Slug}/userlogout")]
        public ActionResult Logout(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            UserService.Logout();
            return RedirectToAction("Index", Slug + "/Home");
        }

        //JOBS CONTROLLERS
        [Route("{Slug}/job")]
        public ActionResult JobTypeFrontEnd(string Slug)
        {
          
            WebsiteViewModel vm = _GetViewModel(Slug);
            vm.BrainTreeToken = _BrainTreeService.CreateToken();
            vm.Website = WebsiteService.websiteGetBySlug(Slug);
            return View(vm);

        }

        //REGISTER SUCCESS
        [Route("{Slug}/RegisterSuccess")]
        public ActionResult RegisterSuccess(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            return View(vm);
        }

        //// CONTACT US
        [Route("{Slug}/contact")]
        public ActionResult ContactForm(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);
            vm.Item = ConfigService.RecaptchaKey;
            return View(vm);
        }

        //// CONTACT US SUCCESS
        [Route("{Slug}/contact_success")]
        public ActionResult ContactSuccess(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);
            
            return View(vm);
        }

        //ROUTE 
        [Route("{Slug}/authentication/{emailGuid:guid}")]
        public ActionResult ConfirmAuth(Guid emailGuid, string Slug)    //  this cannot be null
        {
            WebsiteViewModel vm = _GetViewModel(Slug);
            //ItemResponse<Token> userToken = new ItemResponse<Token>();
            vm.userToken = null;
            vm.userToken = TokenService.userGetByGuid(emailGuid);

            if (vm.userToken == null || vm.userToken.Used != null)
            {
                //ItemViewModel<Guid> vm = new ItemViewModel<Guid>();
                //vm.Item = emailGuid;
                return View(vm);
            }

            string tokenhash = vm.userToken.TokenHash.ToString();

            TokenService.tokenUsedUpdate(vm.userToken.UserId, tokenhash);
            return View("ConfirmSuccess", vm);

        }

        ////EMAIL CONFIRMATION ROUTE ON SUCCESS
        [Route("{Slug}/confirm_success")]
        public ActionResult ConfirmSuccess(string Slug)
        {
            WebsiteViewModel vm = _GetViewModel(Slug);

            return View(vm);
        }


    }
}