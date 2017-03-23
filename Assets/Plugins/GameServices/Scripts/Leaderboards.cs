#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnitySocial.Internal;

namespace UnitySocial
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct LeaderboardEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string username;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string region;
        public float score;
        public int rank;
    }

    public class Leaderboard
    {
        private static Dictionary<IntPtr, Leaderboard> s_LoadedLeaderboards = new Dictionary<IntPtr, Leaderboard>();
        private static Dictionary<int, Delegate> s_CallbackStore = new Dictionary<int, Delegate>();
        private static int s_CallbackId = 0;
        private static LeaderboardValueUpdatedCallback s_ValueUpdatedCallback = null;

        private IntPtr m_Ptr;

        public delegate void LeaderboardsCallback(Leaderboard[] leaderboards);
        public delegate void LeaderboardCallback(Leaderboard leaderboards);
        public delegate void LeaderboardValueUpdatedCallback(Leaderboard leaderboard, float value);
        public delegate void LeaderboardEntryCallback(Leaderboard leaderboard, LeaderboardEntry[] entries);
        public delegate void LeaderboardPositionCallback(Leaderboard leaderboard, int totalEntries, int position);

        private delegate void LeaderboardsLoadedPInvoke(IntPtr ptr, int count, int callbackId);
        private delegate void LeaderboardLoadedPInvoke(IntPtr ptr, int callbackId);
        private delegate void LeaderboardValueUpdatedPInvoke(IntPtr ptr, float value);
        private delegate void LeaderboardScoresLoadedPInvoke(IntPtr ptr, IntPtr entriesPtr, int count, int callbackId);
        private delegate void LeaderboardFriendsScoresLoadedPInvoke(IntPtr ptr, IntPtr entriesPtr, int count, int callbackId);
        private delegate void LeaderboardPositionLoadedPInvoke(IntPtr ptr, int totalEntries, int position, int callbackId);

        public string name { get; private set; }
        public string id { get; private set; }
        public string description { get; private set; }

        public static void GetLeaderboards(LeaderboardsCallback callback)
        {
            UnitySocialBridge.LeaderboardGetLeaderboards(OnLeaderboardsLoaded, GetCallbackId(callback));
        }

        public static void GetLeaderboard(string name, LeaderboardCallback callback)
        {
            UnitySocialBridge.LeaderboardGetLeaderboardWithName(name, OnLeaderboardLoaded, GetCallbackId(callback));
        }

        public static void GetLeaderboardWithId(string id, LeaderboardCallback callback)
        {
            UnitySocialBridge.LeaderboardGetLeaderboardWithId(id, OnLeaderboardLoadedWithId, GetCallbackId(callback));
        }

        public static void SetValueUpdatedCallback(LeaderboardValueUpdatedCallback callback)
        {
            if (s_ValueUpdatedCallback == null)
            {
                UnitySocialBridge.LeaderboardSetValueUpdatedCallback(OnLeaderboardValueUpdated);
            }
            s_ValueUpdatedCallback = callback;
        }

        public void GetScores(int index, int count, LeaderboardEntryCallback callback)
        {
            UnitySocialBridge.LeaderboardGetScores(m_Ptr, index, count, OnLeaderboardScoresLoaded, GetCallbackId(callback));
        }

        public void GetFriendsScores(int index, int count, LeaderboardEntryCallback callback)
        {
            UnitySocialBridge.LeaderboardGetFriendsScores(m_Ptr, index, count, OnLeaderboardFriendsScoresLoaded, GetCallbackId(callback));
        }

        public void GetPosition(LeaderboardPositionCallback callback)
        {
            UnitySocialBridge.LeaderboardGetPosition(m_Ptr, OnLeaderboardPositionLoaded, GetCallbackId(callback));
        }

        public float GetValue()
        {
            return UnitySocialBridge.LeaderboardGetValue(m_Ptr);
        }

        private static int GetCallbackId(Delegate del)
        {
            int id = s_CallbackId++;
            s_CallbackStore.Add(id, del);
            return id;
        }

        private static T GetCallback<T>(int id)
        {
            T callback = (T) Convert.ChangeType(s_CallbackStore[id], typeof(T));
            s_CallbackStore.Remove(id);
            return callback;
        }

        private static Leaderboard LeaderboardNativeToManaged(IntPtr pointer)
        {
            if (s_LoadedLeaderboards.ContainsKey(pointer))
            {
                return s_LoadedLeaderboards[pointer];
            }

            Leaderboard leaderboard = new Leaderboard();
            leaderboard.m_Ptr = pointer;
            leaderboard.name = UnitySocialBridge.LeaderboardGetName(pointer);
            leaderboard.id = UnitySocialBridge.LeaderboardGetId(pointer);
            leaderboard.description = UnitySocialBridge.LeaderboardGetDescription(pointer);
            s_LoadedLeaderboards.Add(pointer, leaderboard);
            return leaderboard;
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardsLoadedPInvoke))]
        private static void OnLeaderboardsLoaded(IntPtr leaderboardsPtr, int count, int callbackId)
        {
            Leaderboard[] leaderboards = new Leaderboard[count];
            for (int i = 0; i < count; i++)
            {
                leaderboards[i] = LeaderboardNativeToManaged(new IntPtr(leaderboardsPtr.ToInt64() + IntPtr.Size * i));
            }
            GetCallback<LeaderboardsCallback>(callbackId)(leaderboards);
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardLoadedPInvoke))]
        private static void OnLeaderboardLoaded(IntPtr leaderboardPtr, int callbackId)
        {
            Leaderboard leaderboard = LeaderboardNativeToManaged(leaderboardPtr);
            if (string.IsNullOrEmpty(leaderboard.name))
            {
                throw new Exception("Loaded leaderboard name was empty.");
            }
            GetCallback<LeaderboardCallback>(callbackId)(leaderboard);
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardLoadedPInvoke))]
        private static void OnLeaderboardLoadedWithId(IntPtr leaderboardPtr, int callbackId)
        {
            Leaderboard leaderboard = LeaderboardNativeToManaged(leaderboardPtr);
            if (string.IsNullOrEmpty(leaderboard.id))
            {
                throw new Exception("Loaded leaderboard id was empty.");
            }
            GetCallback<LeaderboardCallback>(callbackId)(leaderboard);
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardValueUpdatedPInvoke))]
        private static void OnLeaderboardValueUpdated(IntPtr leaderboardPtr, float value)
        {
            if (s_ValueUpdatedCallback == null)
            {
                return;
            }
            s_ValueUpdatedCallback(LeaderboardNativeToManaged(leaderboardPtr), value);
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardScoresLoadedPInvoke))]
        private static void OnLeaderboardScoresLoaded(IntPtr leaderboardPtr, IntPtr entriesPtr, int count, int callbackId)
        {
            Leaderboard leaderboard = LeaderboardNativeToManaged(leaderboardPtr);
            LeaderboardEntry[] entries = new LeaderboardEntry[count];
            int size = Marshal.SizeOf(typeof(LeaderboardEntry));
            for (int i = 0; i < count; i++)
            {
                entries[i] = (LeaderboardEntry) Marshal.PtrToStructure(new IntPtr(entriesPtr.ToInt64() + i * size), typeof(LeaderboardEntry));
            }
            GetCallback<LeaderboardEntryCallback>(callbackId)(leaderboard, entries);
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardFriendsScoresLoadedPInvoke))]
        private static void OnLeaderboardFriendsScoresLoaded(IntPtr leaderboardPtr, IntPtr entriesPtr, int count, int callbackId)
        {
            Leaderboard leaderboard = LeaderboardNativeToManaged(leaderboardPtr);
            LeaderboardEntry[] entries = new LeaderboardEntry[count];
            int size = Marshal.SizeOf(typeof(LeaderboardEntry));
            for (int i = 0; i < count; i++)
            {
                entries[i] = (LeaderboardEntry) Marshal.PtrToStructure(new IntPtr(entriesPtr.ToInt64() + i * size), typeof(LeaderboardEntry));
            }
            GetCallback<LeaderboardEntryCallback>(callbackId)(leaderboard, entries);
        }

        [AOT.MonoPInvokeCallback(typeof(LeaderboardPositionLoadedPInvoke))]
        private static void OnLeaderboardPositionLoaded(IntPtr leaderboardPtr, int totalEntries, int position, int callbackId)
        {
            Leaderboard leaderboard = LeaderboardNativeToManaged(leaderboardPtr);
            GetCallback<LeaderboardPositionCallback>(callbackId)(leaderboard, totalEntries, position);
        }
    }
}
