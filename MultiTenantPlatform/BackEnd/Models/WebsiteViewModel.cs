using Bringpro.Web.Domain;
using Bringpro.Web.Enums;
using Bringpro.Web.Services;
using System.Collections.Generic;

namespace Bringpro.Web.Models.ViewModels
{
    //creating a website view model to render pages on load without an ajax call
    public class WebsiteViewModel : BaseViewModel
    {
        public string Item { get; set; }
        public string Slug { get; set; } //allows the slug to be easily accessible from the view w/ razor
        //Enums
        public SettingsCategory CategoryEnum { get; set; }
        public SettingsType SettingTypeEnum { get; set; }
        public SettingsSection SettingSectionEnum { get; set; }
        public List<WebsiteSettings> Settings { get; set; } //encapsulates all the settings for a page in a list
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
        public string GetImageValueBySettingSlug(string settingSlug) // pass in the setting slug
        {
            WebsiteSettings websiteSettings = GetSettingBySettingSlug(settingSlug); //calls the website object from below 

            if (websiteSettings == null || websiteSettings.Media == null) // performs null check
            {
                return "";
            }
            return websiteSettings.Media.fullUrl; //returns the full media url
        }

        public string GetStringValueBySettingSlug(string settingSlug)
        {
            WebsiteSettings websiteSettings = GetSettingBySettingSlug(settingSlug); //calls the object from below

            if (websiteSettings == null || websiteSettings.SettingsValue == null) // performs null check
            {
                return "";
            }
            return websiteSettings.SettingsValue; //returns the value associated with the setting
        }

        public WebsiteSettings GetSettingBySettingSlug(string settingSlug) //passes in string setting slug
        {
            foreach (WebsiteSettings set in Settings)
            {
                if (set.Setting != null && set.Setting.SettingSlug == settingSlug)
                {
                    return set; //returns full website settings object
                }
            }
            return null;
        }

    }
}