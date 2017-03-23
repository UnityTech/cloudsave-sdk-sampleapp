#if UNITY_ANDROID && !UNITY_EDITOR && UNITY_SOCIAL
using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySocial;
using UnitySocial.Entities;

namespace UnitySocial
{
namespace Internal
{
    internal partial class UnitySocialBridge
    {
        private static AndroidJavaObject s_UnitySocialClass = null;
        private static string s_AndroidPushNotificationOptions = null;

        public static void UnitySocialInitialize(string clientId, string eventListenerName, string bakedAchievements, string bakedLeaderboards)
        {
            if (s_UnitySocialClass == null)
            {
                s_UnitySocialClass = new AndroidJavaObject("com.unity.unitysocial.UnityWrapper");
                s_UnitySocialClass.Call("initialize", clientId, eventListenerName, s_AndroidPushNotificationOptions);
            }
        }

        public static void UnitySocialHideNotifications()
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("hideNotifications");
            }
        }

        public static void UnitySocialShowNotificationActorOnLeftTop(float top)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("showNotificationActorOnLeftTop", top);
            }
        }

        public static void UnitySocialShowNotificationActorOnLeftBottom(float bottom)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("showNotificationActorOnLeftBottom", bottom);
            }
        }

        public static void UnitySocialShowNotificationActorOnRightTop(float top)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("showNotificationActorOnRightTop", top);
            }
        }

        public static void UnitySocialShowNotificationActorOnRightBottom(float bottom)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("showNotificationActorOnRightBottom", bottom);
            }
        }

        public static void UnitySocialDisableEntryPointUpdates()
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("disableEntryPointUpdates");
            }
        }

        public static void UnitySocialEnableEntryPointUpdatesWithImageSize(int sizeInPhysicalPixels)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("enableEntryPointUpdatesWithImageSize", sizeInPhysicalPixels);
            }
        }

        public static string UnitySocialGetEntryPointState()
        {
            if (s_UnitySocialClass != null)
            {
                return s_UnitySocialClass.Call<string>("getEntryPointState");
            }
            return null;
        }

        public static void UnitySocialEntryPointClicked()
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("entryPointClicked");
            }
        }

        public static void UnitySocialStartSession()
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("startSession");
            }
        }

        public static bool UnitySocialIsSupported()
        {
            if (s_UnitySocialClass != null)
            {
                return s_UnitySocialClass.Call<bool>("isSupported");
            }
            return false;
        }

        public static bool UnitySocialIsReady()
        {
            if (s_UnitySocialClass != null)
            {
                return s_UnitySocialClass.Call<bool>("isReady");
            }
            return false;
        }

        public static void UnitySocialEndSession(string data)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("endSession", data);
            }
        }

        public static void UnitySocialSetColorTheme(string theme)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("setColorTheme", theme);
            }
        }

        public static void UnitySocialSetManifestServer(string manifestServer)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("setManifestServer", manifestServer);
            }
        }

        public static void UnitySocialPauseEngine(bool pause)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("pauseEngine", pause);
            }
        }

        public static void UnitySocialAddTags(string data)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("addTags", data);
            }
        }

        public static void UnitySocialRemoveTags(string data)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("removeTags", data);
            }
        }

        public static void UnitySocialUnityMessageReceived(string methodName, string data)
        {
            if (s_UnitySocialClass != null)
            {
                s_UnitySocialClass.Call("unityMessageReceived", methodName, data);
            }
        }

        public static void PlaySessionSendEvent(Dictionary<string, float> sessionEvent, params string[] tags)
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity.unitysocial.PlaySession"))
            {
                AndroidJavaObject hashMap = new AndroidJavaObject("java.util.HashMap");
                IntPtr put = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
                object[] args = new object[2];
                foreach (KeyValuePair<string, float> kvp in sessionEvent)
                {
                    AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
                    AndroidJavaObject v = new AndroidJavaObject("java.lang.Float", kvp.Value);
                    args[0] = k;
                    args[1] = v;
                    AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), put, AndroidJNIHelper.CreateJNIArgArray(args));
                    k.Dispose();
                    v.Dispose();
                }
                javaClass.CallStatic("sendEvent", hashMap);
                hashMap.Dispose();
            }
        }

        public static void PlaySessionSendEvent(string key, float value, params string[] tags)
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity.unitysocial.PlaySession"))
            {
                AndroidJavaObject hashMap = new AndroidJavaObject("java.util.HashMap");
                IntPtr put = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
                object[] args = new object[2];
                AndroidJavaObject k = new AndroidJavaObject("java.lang.String", key);
                AndroidJavaObject v = new AndroidJavaObject("java.lang.Float", value);
                args[0] = k;
                args[1] = v;
                AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), put, AndroidJNIHelper.CreateJNIArgArray(args));
                k.Dispose();
                v.Dispose();
                javaClass.CallStatic("sendEvent", hashMap);
                hashMap.Dispose();
            }
        }

        public static void PlaySessionActivateTag(string tag)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void PlaySessionDeactivateTag(string tag)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void PlaySessionBegin()
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity.unitysocial.PlaySession"))
            {
                javaClass.CallStatic("begin");
            }
        }

        public static void PlaySessionEnd()
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity.unitysocial.PlaySession"))
            {
                javaClass.CallStatic("end");
            }
        }

        public static void PlaySessionCancel()
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void PlaySessionPause()
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void PlaySessionResume()
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static bool PlaySessionIsActive()
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
            return false;
        }

        public static void LeaderboardsGetLeaderboards(Leaderboard.LeaderboardsCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardsGetLeaderboard(string name, Leaderboard.LeaderboardCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardsGetLeaderboardWithId(string id, Leaderboard.LeaderboardCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardsSetValueUpdatedCallback(Leaderboard.LeaderboardValueUpdatedCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardGetLeaderboardWithName(string name, LeaderboardCallback callback, int callbackId)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardGetLeaderboardWithId(string id, LeaderboardCallback callback, int callbackId)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardGetLeaderboards(LeaderboardsCallback callback, int callbackId)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardGetScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardGetFriendsScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardGetPosition(IntPtr leaderboard, LeaderboardPositionCallback callback, int callbackId)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static void LeaderboardSetValueUpdatedCallback(LeaderboardValueUpdatedCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public static float LeaderboardGetValue(IntPtr leaderboard)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
            return 0;
        }

        public static string LeaderboardGetId(IntPtr leaderboard)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
            return "";
        }

        public static string LeaderboardGetName(IntPtr leaderboard)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
            return "";
        }

        public static string LeaderboardGetDescription(IntPtr leaderboard)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
            return "";
        }

        public void LeaderboardsGetScores(int index, int count, Leaderboard.LeaderboardEntryCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public void LeaderboardsGetFriendsScores(int index, int count, Leaderboard.LeaderboardEntryCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public void LeaderboardsGetPosition(Leaderboard.LeaderboardPositionCallback callback)
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
        }

        public float LeaderboardsGetValue()
        {
            Debug.LogWarning("This method is not yet implemented on Android.");
            return 0;
        }
    }
}
}
#endif
