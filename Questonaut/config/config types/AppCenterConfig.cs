using System;
namespace Questonaut.config.configtypes
{
    public interface IAppCenterConfig
    {
        /// <summary>
        /// Return the app secret key formated for the app center start function.
        /// </summary>
        /// <returns>Returns a formatted string.</returns>
        string GetAppSecret();
    }

    public class AppCenterConfig : IAppCenterConfig
    {
        private readonly AppCenterSettings _appCenterSettings;

        public AppCenterConfig(AppCenterSettings appCenterSettings)
        {
            _appCenterSettings = appCenterSettings;
        }

        /// <summary>
        /// Format the appcenter app secret to use directly in the start method.
        /// </summary>
        /// <returns></returns>
        public string GetAppSecret()
        {
            return String.Format("android={0};" +
                  "uwp={0};" +
                  "ios={0}", _appCenterSettings.AppSecret);
        }
    }
}
