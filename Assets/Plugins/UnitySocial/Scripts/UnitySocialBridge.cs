using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySocial;
using UnitySocialTools;

namespace UnitySocialInternal
{
    internal partial class UnitySocialBridge : MonoBehaviour
    {
        private static UnitySocialBridge s_BridgeInstance;

        public delegate void LeaderboardCallback(IntPtr leaderboardPointer, int callbackId);
        public delegate void LeaderboardsCallback(IntPtr leaderboardPointer, int count, int callbackId);
        public delegate void LeaderboardEntryCallback(IntPtr leaderboardPointer, IntPtr entries, int count, int callbackId);
        public delegate void LeaderboardPositionCallback(IntPtr leaderboard, int totalEntries, int position, int callbackId);
        public delegate void LeaderboardValueUpdatedCallback(IntPtr leaderboard, float value);

        public static UnitySocialBridge GetBridge()
        {
            if (s_BridgeInstance == null && !Application.isEditor)
            {
                UnitySocialSettings settings = (UnitySocialSettings) Resources.Load("UnitySocialSettings");

                if (settings != null)
                {
                    if (settings.enabled)
                    {
                        GameObject gameObject = new GameObject("UnitySocial");

                        if (gameObject != null)
                        {
                            gameObject.name = gameObject.name + gameObject.GetInstanceID();
                            s_BridgeInstance = gameObject.AddComponent<UnitySocialBridge>();

                            if (s_BridgeInstance != null)
                            {
                                #if UNITY_ANDROID && !UNITY_EDITOR && UNITY_SOCIAL
                                var options = GetPushNotificationOptions(settings);
                                s_AndroidPushNotificationOptions = (options != null) ? UnitySocialTools.Json.Serialize(options) : null;
                                #endif
                                UnitySocialInitialize(settings.clientId, s_BridgeInstance.name, "", settings.bakedLeaderboards);
                            }

                            DontDestroyOnLoad(gameObject);
                        }
                    }
                }
            }
            return s_BridgeInstance;
        }

        private static Dictionary<string, object> GetPushNotificationOptions(UnitySocialSettings settings)
        {
            if (settings == null)
            {
                return null;
            }

            Dictionary<string, object> pnOptions = new Dictionary<string, object>();

            string backend = settings.androidPushNotificationBackend;
            if (backend != null && backend.Length > 0)
            {
                pnOptions.Add("backend", backend);
            }
            else
            {
                return null;
            }

            string senderId = settings.androidPushNotificationSenderId;
            if (senderId != null && senderId.Length > 0)
            {
                pnOptions.Add("senderId", senderId);
            }
            else
            {
                Debug.LogWarning("Push notifications backend is set but no sender ID is set. Disabling push notifications.");
                return null;
            }

            return pnOptions;
        }

        public void OnIdentityChanged(string data)
        {
            var dict = DictionaryExtensions.JsonToDictionary(data);
            UnitySocialCredential credential = null;
            object projectId, userId, accessToken, userName, isAnonymous, callbackId;
            string error = null;
            if (dict.TryGetValue("projectId", out projectId) &&
                dict.TryGetValue("userId", out userId) &&
                dict.TryGetValue("isAnonymous", out isAnonymous) &&
                dict.TryGetValue("accessToken", out accessToken) &&
                dict.TryGetValue("userName", out userName))
            {
                credential = new UnitySocialCredential();
                credential.projectId = (string) projectId;
                credential.userId = (string) userId;
                credential.token = (string) accessToken;
                credential.username = (string) userName;
                credential.isAnonymous = (bool) isAnonymous;
            }
            else
            {
                error = "Message from js was:\"" + data + "\"";
            }

            if (dict.TryGetValue("callbackId", out callbackId))
            {
                Identity.DefaultProvider().CallGetOrRefreshCredentialAsyncCallback(Convert.ToUInt32(callbackId), error, credential);
            }
            else
            {
                Identity.DefaultProvider().SetCredential(error, credential);
            }
        }

        // From native
        private void UnitySocialGameShouldPause()
        {
            SocialCore.onGameShouldPause.Invoke();

            if (SocialCore.pauseEngineAutomatically)
            {
                UnitySocialPauseEngine(true);
            }
        }

        private void UnitySocialGameShouldResume()
        {
            SocialCore.onGameShouldResume.Invoke();
        }

        private void UnitySocialUpdateEntryPointState(string data)
        {
            if (data != null && data.Length > 0)
            {
                Dictionary<string, object> dict = UnitySocialTools.DictionaryExtensions.JsonToDictionary(data);
                SocialOverlay.EntryPointState newState = new SocialOverlay.EntryPointState();

                if (UnitySocialTools.DictionaryExtensions.TryGetValue(dict, "imageURL", out newState.imageURL) &&
                    UnitySocialTools.DictionaryExtensions.TryGetValue(dict, "notificationCount", out newState.notificationCount))
                {
                    SocialOverlay.onEntryPointStateUpdated.Invoke(newState);
                }
            }
        }

        private void UnitySocialInitialized(string result)
        {
            if (result != null && result.Length > 0)
            {
                Dictionary<string, object> dict = UnitySocialTools.DictionaryExtensions.JsonToDictionary(result);
                bool isSupported;

                if (UnitySocialTools.DictionaryExtensions.TryGetValue(dict, "isSupported", out isSupported))
                {
                    SocialCore.onInitialized.Invoke(isSupported);
                }
            }
        }

        private void UnitySocialRewardClaimed(string metadata)
        {
            if (metadata != null && metadata.Length > 0)
            {
                Dictionary<string, object> metadataDictionary = UnitySocialTools.DictionaryExtensions.JsonToDictionary(metadata);
                SocialCore.onRewardClaimed.Invoke(metadataDictionary);
            }
        }
    }
}
