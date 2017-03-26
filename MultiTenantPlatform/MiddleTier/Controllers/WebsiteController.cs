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

        //Dependency from website service - allows us to run specific services
        [Dependency]
        public IWebsiteService _WebsiteService { get; set; }

        //// GET: Website by Slug
        [Route()]
        [Route("{Slug}/home")]
        public ActionResult Index(string Slug)
        {
            //instantiate website viewmodel - majority of logic below in function as it repeats througout the various views
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
            //adding a tokenhash to the viewmodel for registration
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

        //get viewmodel function created for majority of logic needed to inject website view model with appropriate data
        private WebsiteViewModel _GetViewModel(string Slug)
        {
            //null check on slug, if null load bringpro website by default
            //added for the azure hosted version
            if (Slug == null)
            {
                Slug = "bringpro";
            }

            //instantiate new instance of website view model
            WebsiteViewModel vm = new WebsiteViewModel();
            vm.Slug = Slug; // add website slug to view model
            vm.CategoryEnum = SettingsCategory.String; //adding enums to the viewmodel
            vm.SettingTypeEnum = SettingsType.Design;
            vm.SettingSectionEnum = SettingsSection.Layout;

            WebsiteSettingsServices websiteService = new WebsiteSettingsServices(); // instantiate a new instance of website settings service
            //generate a new list of website settings - populated by service that loads settings by website slug
            List<WebsiteSettings> WebsiteBySlug = websiteService.GetSettingsBySlug(Slug); 
            vm.Settings = WebsiteBySlug;
            if (vm.Settings.Count < 1 && Slug != "backoffice")
            {
                throw new HttpException(404, "Website Does Not Exist");
            }

            //if website exists - load a model of website 
            if (Slug != null && Slug != "")
            {
                vm.Website = WebsiteService.websiteGetBySlug(Slug);
            }

            //returning the viewmodel after populating with website settings model and website model
            // both have different fields of data
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
      
            vm.userToken = null;
            vm.userToken = TokenService.userGetByGuid(emailGuid);

            if (vm.userToken == null || vm.userToken.Used != null)
            {
              
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