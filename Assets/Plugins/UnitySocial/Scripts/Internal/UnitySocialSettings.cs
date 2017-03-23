using UnityEngine;

namespace UnitySocial
{
namespace Internal
{
    /// <summary>
    /// Unity Social settings
    /// </summary>
    public class UnitySocialSettings : ScriptableObject
    {
        /// <summary>
        /// Client ID
        /// </summary>
        public string clientId;

        /// <summary>
        /// Is iOS support enabled
        /// </summary>
        public bool iosSupportEnabled;

        /// <summary>
        /// Is Android support enabled
        /// </summary>
        public bool androidSupportEnabled;

        /// <summary>
        /// Android push notification backend
        /// </summary>
        public string androidPushNotificationBackend;

        /// <summary>
        /// Android push notification sender ID
        /// </summary>
        public string androidPushNotificationSenderId;

        /// <summary>
        /// Baked leaderboards
        /// </summary>
        public string bakedLeaderboards;

        /// <summary>
        /// Checks if Unity Social is enabled
        /// </summary>
        public bool enabled
        {
            get
            {
                #if UNITY_IOS
                return iosSupportEnabled;
                #elif UNITY_ANDROID
                return androidSupportEnabled;
                #else
                return false;
                #endif
            }
        }

        /// <summary>
        /// Checks if settings are valid
        /// </summary>
        public bool isValid
        {
            get
            {
                // string.IsNullOrWhiteSpace
                if (!(string.IsNullOrEmpty(clientId) || clientId.Trim().Length == 0))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
}
