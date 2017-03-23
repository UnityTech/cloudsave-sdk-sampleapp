#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_ANDROID) || !UNITY_SOCIAL
using System;
using System.Collections.Generic;
using UnitySocial;


namespace UnitySocial
{
namespace Internal
{
    internal partial class UnitySocialBridge
    {
        public static void UnitySocialInitialize(string clientId, string eventListenerName, string bakedAchievements, string bakedLeaderboards) {}

        public static void UnitySocialStartSession() {}

        public static bool UnitySocialIsSupported() { return false; }

        public static bool UnitySocialIsReady() { return false; }

        public static void UnitySocialEndSession(string data) {}

        public static void UnitySocialPauseEngine(bool pause) {}

        public static void UnitySocialShowNotificationActorOnLeftTop(float top) {}

        public static void UnitySocialShowNotificationActorOnLeftBottom(float bottom) {}

        public static void UnitySocialShowNotificationActorOnRightTop(float top) {}

        public static void UnitySocialShowNotificationActorOnRightBottom(float bottom) {}

        public static void UnitySocialHideNotifications() {}

        public static void UnitySocialEntryPointClicked() {}

        public static string UnitySocialGetEntryPointState() { return null; }

        public static void UnitySocialEnableEntryPointUpdatesWithImageSize(int sizeInPhysicalPixels) {}

        public static void UnitySocialDisableEntryPointUpdates() {}

        public static void UnitySocialSetColorTheme(string theme) {}

        public static void UnitySocialSetManifestServer(string manifestServer) {}

        public static void UnitySocialAddTags(string data) {}

        public static void UnitySocialRemoveTags(string data) {}

        public static void UnitySocialUnityMessageReceived(string eventName, string data) {}

        public static void PlaySessionSendEvent(Dictionary<string, float> sessionEvent, params string[] tags) {}

        public static void PlaySessionSendEvent(string key, float value, params string[] tags) {}

        public static void PlaySessionActivateTag(string tag) {}

        public static void PlaySessionDeactivateTag(string tag) {}

        public static void PlaySessionBegin() {}

        public static void PlaySessionEnd() {}

        public static void PlaySessionCancel() {}

        public static void PlaySessionPause() {}

        public static void PlaySessionResume() {}

        public static bool PlaySessionIsActive() { return false; }

        public static void LeaderboardGetLeaderboardWithName(string name, LeaderboardCallback callback, int callbackId) {}

        public static void LeaderboardGetLeaderboardWithId(string id, LeaderboardCallback callback, int callbackId) {}

        public static void LeaderboardGetLeaderboards(LeaderboardsCallback callback, int callbackId) {}

        public static void LeaderboardGetScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId) {}

        public static void LeaderboardGetFriendsScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId) {}

        public static void LeaderboardGetPosition(IntPtr leaderboard, LeaderboardPositionCallback callback, int callbackId) {}

        public static void LeaderboardSetValueUpdatedCallback(LeaderboardValueUpdatedCallback callback) {}

        public static float LeaderboardGetValue(IntPtr leaderboard) { return 0; }

        public static string LeaderboardGetId(IntPtr leaderboard) { return ""; }

        public static string LeaderboardGetName(IntPtr leaderboard) { return ""; }

        public static string LeaderboardGetDescription(IntPtr leaderboard) { return ""; }
    }
}
}
#endif
