using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocial.Entities;
using UnitySocial.Events;
using UnitySocial.Internal;
using UnitySocial.Tools;

namespace UnitySocial
{
    /// <summary>
    /// Social overlay
    /// </summary>
    public static class SocialOverlay
    {
        private static bool s_EntryPointUpdatesEnabled = false;
        private static int s_EntryPointImageSize = 128;
        private static NotificationLocation s_NotificationActorLocation = NotificationLocation.LeftTop;
        private static int s_NotificationActorOffset = 0;
        private static bool s_UpdatingEntryPointSettings = false;
        private static bool s_UpdatingNotificationSettings = false;
        private static EntryPointStateUpdatedEvent s_EntryPointStateUpdated = new EntryPointStateUpdatedEvent();

        /// <summary>
        /// Notification actor location
        /// </summary>
        public enum NotificationLocation { Hidden = 0, LeftTop, LeftBottom, RightTop, RightBottom };

        /// <summary>
        /// Occurs when entry point should update
        /// </summary>
        public static EntryPointStateUpdatedEvent onEntryPointStateUpdated { get { return s_EntryPointStateUpdated; } }

        /// <summary>
        /// Gets entry point state
        /// </summary>
        public static EntryPointState currentEntryPointState
        {
            get
            {
                string json = UnitySocialBridge.UnitySocialGetEntryPointState();
                if (!string.IsNullOrEmpty(json))
                {
                    Dictionary<string, object> dict = UnitySocial.Tools.DictionaryExtensions.JsonToDictionary(json);
                    EntryPointState newState = new EntryPointState();

                    if (UnitySocial.Tools.DictionaryExtensions.TryGetValue(dict, "imageURL", out newState.imageURL) &&
                        UnitySocial.Tools.DictionaryExtensions.TryGetValue(dict, "notificationCount", out newState.notificationCount))
                    {
                        return newState;
                    }
                }
                return default(EntryPointState);
            }
        }

        /// <summary>
        /// Gets or sets entry point image size
        /// </summary>
        public static bool entryPointUpdatesEnabled
        {
            get
            {
                return s_EntryPointUpdatesEnabled;
            }
            set
            {
                s_EntryPointUpdatesEnabled = value;
                UpdateEntryPointSettings();
            }
        }

        /// <summary>
        /// Gets or sets entry point image size in pixels
        /// </summary>
        public static int entryPointImageSize
        {
            get
            {
                return s_EntryPointImageSize;
            }
            set
            {
                s_EntryPointImageSize = value;
                UpdateEntryPointSettings();
            }
        }

        /// <summary>
        /// Gets or sets the notification origin
        /// </summary>
        public static NotificationLocation notificationActorLocation
        {
            get
            {
                return s_NotificationActorLocation;
            }
            set
            {
                s_NotificationActorLocation = value;
                UpdateNotificationSettings();
            }
        }

        /// <summary>
        /// Gets or sets the notification y offset from it's origin in pixels
        /// </summary>
        public static int notificationActorOffset
        {
            get
            {
                return s_NotificationActorOffset;
            }
            set
            {
                s_NotificationActorOffset = value;
                UpdateNotificationSettings();
            }
        }

        /// <summary>
        /// EntryPointClicked
        /// </summary>
        public static void EntryPointClicked()
        {
            UnitySocialBridge.UnitySocialEntryPointClicked();
        }

        /// <summary>
        /// Set color theme
        /// </summary>
        /// <param name="theme">Colors for theme</param>
        public static void SetColorTheme(Dictionary<string, string> theme)
        {
            UnitySocialBridge.UnitySocialSetColorTheme((theme != null) ?  DictionaryExtensions.DictionaryToJson(theme) : null);
        }

        private static void UpdateEntryPointSettings()
        {
            if (!s_UpdatingEntryPointSettings)
            {
                if (UnitySocialBridge.GetBridge() != null)
                {
                    UnitySocialBridge.GetBridge().StartCoroutine(BatchEntryPointSettings());
                }
                s_UpdatingEntryPointSettings = true;
            }
        }

        private static IEnumerator BatchEntryPointSettings()
        {
            yield return new WaitForEndOfFrame();
            if (entryPointUpdatesEnabled)
            {
                UnitySocialBridge.UnitySocialEnableEntryPointUpdatesWithImageSize(s_EntryPointImageSize);
            }
            else
            {
                UnitySocialBridge.UnitySocialDisableEntryPointUpdates();
            }
            s_UpdatingEntryPointSettings = false;
        }

        private static void UpdateNotificationSettings()
        {
            if (!s_UpdatingNotificationSettings)
            {
                if (UnitySocialBridge.GetBridge() != null)
                {
                    UnitySocialBridge.GetBridge().StartCoroutine(BatchNotificationSettings());
                }
                s_UpdatingNotificationSettings = true;
            }
        }

        private static IEnumerator BatchNotificationSettings()
        {
            yield return new WaitForEndOfFrame();

            switch (s_NotificationActorLocation)
            {
            case NotificationLocation.LeftTop:
                UnitySocialBridge.UnitySocialShowNotificationActorOnLeftTop(s_NotificationActorOffset);
                break;

            case NotificationLocation.LeftBottom:
                UnitySocialBridge.UnitySocialShowNotificationActorOnLeftBottom(s_NotificationActorOffset);
                break;

            case NotificationLocation.RightTop:
                UnitySocialBridge.UnitySocialShowNotificationActorOnRightTop(s_NotificationActorOffset);
                break;

            case NotificationLocation.RightBottom:
                UnitySocialBridge.UnitySocialShowNotificationActorOnRightBottom(s_NotificationActorOffset);
                break;

            default:
                UnitySocialBridge.UnitySocialHideNotifications();
                break;
            }

            s_UpdatingNotificationSettings = false;
        }
    }
}
