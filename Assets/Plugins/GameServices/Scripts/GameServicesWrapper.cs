#if UNITY_SOCIAL

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
#define UNITY_SOCIAL_SUPPORTED
#endif

using System;
using System.Runtime.InteropServices;

namespace UnitySocial
{
    internal static class GameServicesWrapper
    {
        const string kDllName = "__Internal";

        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardInternal
        {
            public string id;
            public IntPtr entries;
            public int numEntries;
        }

        [DllImport(kDllName)]
        public static extern void PlaySessionSendEvent(string[] sessionEvent_keys, float[] sessionEvent_values, int sessionEvent_length, string[] tags_p, int tags_length);

        [DllImport(kDllName)]
        public static extern void PlaySessionSendEvent1(string key, float value, string[] tags_p, int tags_length);

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
        public static extern int AchievementsGetAchievementDefinitionsCount();

        [DllImport(kDllName)]
        public static extern IntPtr AchievementsGetAchievementDefinition(int index);

        public delegate void AchievementUnlockedCallback(string id);

        [DllImport(kDllName)]
        public static extern void AchievementsSetAchievementUnlockedCallback(AchievementUnlockedCallback unlockedCallback);

        [DllImport(kDllName)]
        public static extern IntPtr AchievementsGetStatus(string id);

        [DllImport(kDllName)]
        public static extern void AchievementsClaimAchievement(string id);

        public delegate void LeaderboardCallback(IntPtr leaderboardPointer, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetLeaderboardWithName(string name, LeaderboardCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetLeaderboardWithId(string id, LeaderboardCallback callback, int callbackId);

        public delegate void LeaderboardsCallback(IntPtr leaderboardPointer, int count, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetLeaderboards(LeaderboardsCallback callback, int callbackId);

        public delegate void LeaderboardEntryCallback(IntPtr leaderboardPointer, IntPtr entries, int count, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetFriendsScores(IntPtr leaderboard, int index, int count, LeaderboardEntryCallback callback, int callbackId);

        public delegate void LeaderboardPositionCallback(IntPtr leaderboard, int totalEntries, int position, int callbackId);

        [DllImport(kDllName)]
        public static extern void LeaderboardGetPosition(IntPtr leaderboard, LeaderboardPositionCallback callback, int callbackId);

        public delegate void LeaderboardValueUpdatedCallback(IntPtr leaderboard, float value);

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
    }
}
#endif
