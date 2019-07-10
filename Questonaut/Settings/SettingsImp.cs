using System;
using Firebase.Rest.Auth.Payloads;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Plugin.Settings.Abstractions.Extensions;
using Xamarin.Forms;

namespace Questonaut.Settings
{
    public static class SettingsImp
    {
        #region instance
        private static Lazy<ISettings> _settings;

        public static ISettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Lazy<ISettings>(() => CrossSettings.Current, System.Threading.LazyThreadSafetyMode.PublicationOnly);
                }

                return _settings.Value;
            }
            set => _settings = new Lazy<ISettings>(() => CrossSettings.Current, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }
        #endregion

        private const string UserKey = "user";
        private static readonly string UserDefault = string.Empty;

        public static string UserValue
        {
            get => Settings.GetValueOrDefault(UserKey, UserDefault);
            set => Settings.AddOrUpdateValue(UserKey, value);
        }

        private const string ShowIntroKey = "showIntro";
        private static readonly string ShowIntroDefault = "true";

        public static string ShowIntro
        {
            get => Settings.GetValueOrDefault(ShowIntroKey, ShowIntroDefault);
            set => Settings.AddOrUpdateValue(ShowIntroKey, value);
        }
    }
}
