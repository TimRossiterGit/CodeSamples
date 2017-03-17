using Sabio.Web.Enums;
using Sabio.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sabio.Web.Controllers
{
    [RoutePrefix("settings")]
    public class SettingsController : BaseController
    {
  
        // GET: Settings
        public ActionResult Index()
        {
            SettingsViewModel vm = new SettingsViewModel();
            vm.CategoryEnum = SettingsCategory.String;
            vm.SettingSectionEnum = SettingsSection.Layout;

            return View("IndexNg", vm);
        }

        [Route("add")]
        [Route("edit/{Id:int}")]
        public ActionResult SettingsForm(int? Id = null)
        {
            //ItemViewModel<int?> vm = new ItemViewModel<int?>();
            //vm.Item = Id;
            SettingsViewModel vm = new SettingsViewModel();
            vm.CategoryEnum = SettingsCategory.String;
            vm.SettingSectionEnum = SettingsSection.Layout;

            vm.SettingId = Id;
            return View("SettingsFormNg", vm);
        }

      
    }
}