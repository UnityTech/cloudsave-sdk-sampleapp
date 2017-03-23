#if UNITY_IOS && !UNITY_EDITOR && UNITY_SOCIAL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnitySocial
{
namespace Internal
{
    internal partial class UnitySocialBridge
    {
        private const string kDllName = "__Internal";

        [DllImport(kDllName)]
        public static extern void UnitySocialInitialize(string clientId, string eventListenerName, string bakedAchievements, string bakedLeaderboards);

        [DllImport(kDllName)]
        public static extern void UnitySocialStartSession();

        [DllImport(kDllName)]
        public static extern bool UnitySocialIsSupported();

        [DllImport(kDllName)]
        public static extern bool UnitySocialIsReady();

        [DllImport(kDllName)]
        public static extern void UnitySocialEndSession(string data);

        [DllImport(kDllName)]
        public static extern void UnitySocialPauseEngine(bool pause);

        [DllImport(kDllName)]
        public static extern void UnitySocialShowNotificationActorOnLeftTop(float top);

        [DllImport(kDllName)]
        public static extern void UnitySocialShowNotificationActorOnLeftBottom(float bottom);

        [DllImport(kDllName)]
        public static extern void UnitySocialShowNotificationActorOnRightTop(float top);

        [DllImport(kDllName)]
        public static extern void UnitySocialShowNotificationActorOnRightBottom(float bottom);

        [DllImport(kDllName)]
        public static extern void UnitySocialHideNotifications();

        [DllImport(kDllName)]
        public static extern void UnitySocialEntryPointClicked();

        [DllImport(kDllName)]
        public static extern string UnitySocialGetEntryPointState();

        [DllImport(kDllName)]
        public static extern void UnitySocialEnableEntryPointUpdatesWithImageSize(int sizeInPhysicalPixels);

        [DllImport(kDllName)]
        public static extern void UnitySocialDisableEntryPointUpdates();

        [DllImport(kDllName)]
        public static extern void UnitySocialSetColorTheme(string theme);

        [DllImport(kDllName)]
        public static extern void UnitySocialSetManifestServer(string manifestServer);

        [DllImport(kDllName)]
        public static extern void UnitySocialAddTags(string data);

        [DllImport(kDllName)]
        public static extern void UnitySocialRemoveTags(string data);

        [DllImport(kDllName)]
        public static extern void UnitySocialUnityMessageReceived(string eventName, string data);

        [DllImport(kDllName)]
        private static extern void PlaySessionSendEvent(string[] sessionEvent_keys, float[] sessionEvent_values, int sessionEvent_length, string[] tags_p, int tags_length);

        [DllImport(kDllName)]
        private static extern void PlaySessionSendEvent1(string key, float value, string[] tags_p, int tags_length);

        [DllImport(kDllName)]
        public static extern void PlaySessionActivateTag(string tag);

        [DllImport(kDllName)]
        public static extern void PlaySessionDeactivateTag(string tag);

        [DllImport(kDllName)]
        public static extern void PlaySessionBegin();

        [DllImport(kDllName)]
        public static extern void PlaySessionEnd();

        [DllImport(kDllName)]
        public static extern void PlaySessionCancel();

        [DllImport(kDllName)]
        public static extern void PlaySessionPause();

        [DllImport(kDllName)]
        public static extern void PlaySessionResume();

        [DllImport(kDllName)]
        public static extern bool PlaySessionIsActive();

        [DllImport(kDllName)]
        public static extern void LeaderboardGetLeaderboardWithName(string name, LeaderboardCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetLeaderboardWithId(string id, LeaderboardCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetLeaderboards(LeaderboardsCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetFriendsScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetPosition(IntPtr leaderboard, LeaderboardPositionCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardSetValueUpdatedCallback(LeaderboardValueUpdatedCallback callback);

        [DllImport(kDllName)]
        public static extern float LeaderboardGetValue(IntPtr leaderboard);

        [DllImport(kDllName)]
        public static extern string LeaderboardGetId(IntPtr leaderboard);

        [DllImport(kDllName)]
        public static extern string LeaderboardGetName(IntPtr leaderboard);

        [DllImport(kDllName)]
        public static extern string LeaderboardGetDescription(IntPtr leaderboard);

        public static void PlaySessionSendEvent(Dictionary<string, float> sessionEvent, params string[] tags)
        {
            PlaySessionSendEvent(sessionEvent.Keys.ToArray(), sessionEvent.Values.ToArray(), sessionEvent.Count, tags, tags.Length);
        }

        public static void PlaySessionSendEvent(string key, float value, params string[] tags)
        {
            PlaySessionSendEvent1(key, value, tags, tags.Length);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardInternal
        {
            public string id;
            public IntPtr entries;
            public int numEntries;
        }
    }
}
}
#endif
