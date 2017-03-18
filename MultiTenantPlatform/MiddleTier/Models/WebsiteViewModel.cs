using Bringpro.Web.Domain;
using Bringpro.Web.Enums;
using Bringpro.Web.Services;
using System.Collections.Generic;

namespace Bringpro.Web.Models.ViewModels
{
    public class WebsiteViewModel : BaseViewModel
    {
        public string Item { get; set; }
        public string Slug { get; set; }
        //Enums
        public SettingsCategory CategoryEnum { get; set; }
        public SettingsType SettingTypeEnum { get; set; }
        public SettingsSection SettingSectionEnum { get; set; }
        public List<WebsiteSettings> Settings { get; set; }
        //get token hash for register
        public string TokenHash { get; set; }
        //Website Object
        public Website Website { get; set; }
        //from dashboard view model
        public ActivityTypeId ActivityTypes { get; set; }
        //Login Tokens
        public string Token { get; set; }
        public Token userToken { get; set; }
        public string BrainTreeToken { get; set; }      
        //for Job Item Invoice information
        public int jobInvoiceId { get; set; }
        public string GoogleStaticMapKey = ConfigService.GoogleStaticMapApiKey;

        //Return the Media Url
        public string GetImageValueBySettingSlug(string settingSlug)
        {
            WebsiteSettings websiteSettings = GetSettingBySettingSlug(settingSlug);

            if (websiteSettings == null || websiteSettings.Media == null)
            {
                return "";
            }
            return websiteSettings.Media.fullUrl;
        }

        public string GetStringValueBySettingSlug(string settingSlug)
        {
            WebsiteSettings websiteSettings = GetSettingBySettingSlug(settingSlug);

            if (websiteSettings == null || websiteSettings.SettingsValue == null)
            {
                return "";
            }
            return websiteSettings.SettingsValue;
        }

        public WebsiteSettings GetSettingBySettingSlug(string settingSlug)
        {
            foreach (WebsiteSettings set in Settings)
            {
                if (set.Setting != null && set.Setting.SettingSlug == settingSlug)
                {
                    return set;
                }
            }
            return null;
        }

    }
}