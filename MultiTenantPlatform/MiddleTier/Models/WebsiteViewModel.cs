using Sabio.Web.Domain;
using Sabio.Web.Enums;
using Sabio.Web.Services;
using System.Collections.Generic;

namespace Sabio.Web.Models.ViewModels
{
    public class WebsiteViewModel : BaseViewModel
    {
        public string Item { get; set; }
        public string Slug { get; set; }
        public SettingsCategory CategoryEnum { get; set; }
        public SettingsType SettingTypeEnum { get; set; }
        public SettingsSection SettingSectionEnum { get; set; }
        public List<WebsiteSettings> Settings { get; set; }
        //trying to get token hash for register
        public string TokenHash { get; set; }
        public Website Website { get; set; }
        //from dashboard view model
        public ActivityTypeId ActivityTypes { get; set; }
        public string Token { get; set; }
        public Token userToken { get; set; }
        //
        public string BrainTreeToken { get; set; }
      
        //for Job Item Invoice information
        public int jobInvoiceId { get; set; }
        public string GoogleStaticMapKey = ConfigService.GoogleStaticMapApiKey;

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