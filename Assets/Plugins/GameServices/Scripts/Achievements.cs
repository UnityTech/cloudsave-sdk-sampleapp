#if UNITY_SOCIAL

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
#define UNITY_SOCIAL_SUPPORTED
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnitySocial
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AchievementStatus
    {
        public string id;
        public float progress;
        public float maxProgress;
        public bool unlocked;
        public bool claimed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AchievementDefinition
    {
        public string id;
        public string platformId;
        public string name;
        public string description;
        public bool permitLaterClaim;
        public int status;
        public int displayOrder;
    }

    public class Achievements
    {
        public static int achievementDefinitionsCount
        {
            get
            {
    #if UNITY_SOCIAL_SUPPORTED
    #if UNITY_ANDROID
                Debug.LogWarning("This method is not yet implemented on Android.");
                return 0;
    #else
                return GameServicesWrapper.AchievementsGetAchievementDefinitionsCount();
    #endif
    #else
                return default(int);
    #endif
            }
        }

        public static AchievementDefinition GetAchievementDefinition(int index)
        {
    #if UNITY_SOCIAL_SUPPORTED
    #if UNITY_ANDROID
            Debug.LogWarning("This method is not yet implemented on Android.");
            return default(AchievementDefinition);
    #else
            IntPtr definitionPtr = GameServicesWrapper.AchievementsGetAchievementDefinition(index);
            if (definitionPtr == IntPtr.Zero)
            {
                Debug.LogError("Failed to get definition at index:" + index);
                return default(AchievementDefinition);
            }
            return (AchievementDefinition) Marshal.PtrToStructure(definitionPtr, typeof(AchievementDefinition));
    #endif
    #else
            return default(AchievementDefinition);
    #endif
        }

        public delegate void AchievementUnlockedCallback(string id);

    #if UNITY_SOCIAL_SUPPORTED
        private static AchievementUnlockedCallback s_UnlockedCallback;
    #endif
    #if UNITY_ANDROID
        class JavaAchievementUnlockedCallback : AndroidJavaProxy
        {
            public JavaAchievementUnlockedCallback() : base("com.unity.gameservices.Achievements$AchievementUnlockedCallback")
            {
            }

            void invoke(string id)
            {
    #if UNITY_SOCIAL_SUPPORTED
                if (s_UnlockedCallback != null)
                {
                    s_UnlockedCallback(id);
                }
    #endif
            }
        }
    #else
        [AOT.MonoPInvokeCallback(typeof(AchievementUnlockedCallback))]
        private static void AchievementsOnUnlockedCallback(string id)
        {
    #if UNITY_SOCIAL_SUPPORTED
            if (s_UnlockedCallback != null)
            {
                s_UnlockedCallback(id);
            }
    #endif
        }

    #endif

        public static void SetAchievementUnlockedCallback(AchievementUnlockedCallback unlockedCallback)
        {
    #if UNITY_SOCIAL_SUPPORTED
            s_UnlockedCallback = unlockedCallback;
    #if UNITY_ANDROID
            Debug.LogWarning("This method is not yet implemented on Android.");
    #else
            GameServicesWrapper.AchievementsSetAchievementUnlockedCallback(AchievementsOnUnlockedCallback);
    #endif
    #else
            return;
    #endif
        }

        public static AchievementStatus GetStatus(string id)
        {
    #if UNITY_SOCIAL_SUPPORTED
    #if UNITY_ANDROID
            Debug.LogWarning("This method is not yet implemented on Android.");
            return default(AchievementStatus);
    #else
            IntPtr statusPtr = GameServicesWrapper.AchievementsGetStatus(id);
            if (statusPtr == IntPtr.Zero)
            {
                Debug.LogError("Failed to get achievement status with id:" + id);
                return default(AchievementStatus);
            }
            AchievementStatus status = (AchievementStatus) Marshal.PtrToStructure(statusPtr, typeof(AchievementStatus));
            Marshal.FreeHGlobal(statusPtr);
            return status;
    #endif
    #else
            return default(AchievementStatus);
    #endif
        }

        public static void ClaimAchievement(string id)
        {
    #if UNITY_SOCIAL_SUPPORTED
    #if UNITY_ANDROID
            Debug.LogWarning("This method is not yet implemented on Android.");
    #else
            GameServicesWrapper.AchievementsClaimAchievement(id);
    #endif
    #endif
        }
    }
}
#endif
