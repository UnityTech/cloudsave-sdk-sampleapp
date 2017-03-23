using UnityEngine;

namespace UnitySocial
{
    public class UnitySocialSettings : ScriptableObject
    {
        public string clientId;

        public bool iosSupportEnabled;
        public bool androidSupportEnabled;

        public string androidPushNotificationBackend;
        public string androidPushNotificationSenderId;
        public string bakedLeaderboards;

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
