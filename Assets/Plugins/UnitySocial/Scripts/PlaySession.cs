using System.Collections.Generic;
using UnitySocial.Internal;

namespace UnitySocial
{
    /// <summary>
    /// Play Session
    /// </summary>
    public class PlaySession
    {
        public static void SendEvent(Dictionary<string, float> sessionEvent, params string[] tags)
        {
            foreach (KeyValuePair<string, float> entry in sessionEvent)
            {
                PlaySession.SendEvent(entry.Key, entry.Value, tags);
            }
        }

        public static void SendEvent(string key, float value, params string[] tags)
        {
            UnitySocialBridge.PlaySessionSendEvent(key, value, tags);
        }

        /// <summary>
        /// Activates session tag
        /// </summary>
        /// <param name="tag">Tag to activate</param>
        public static void ActivateTag(string tag)
        {
            UnitySocialBridge.PlaySessionActivateTag(tag);
        }

        /// <summary>
        /// Deactivates session tag
        /// </summary>
        /// <param name="tag">Tag to deactivate</param>
        public static void DeactivateTag(string tag)
        {
            UnitySocialBridge.PlaySessionDeactivateTag(tag);
        }

        /// <summary>
        /// Begin game session
        /// </summary>
        public static void Begin()
        {
            UnitySocialBridge.PlaySessionBegin();
        }

        /// <summary>
        /// End game session
        /// </summary>
        public static void End()
        {
            UnitySocialBridge.PlaySessionEnd();
        }

        /// <summary>
        /// Cancel game session
        /// </summary>
        public static void Cancel()
        {
            UnitySocialBridge.PlaySessionCancel();
        }

        /// <summary>
        /// Pause game session
        /// </summary>
        public static void Pause()
        {
            UnitySocialBridge.PlaySessionPause();
        }

        /// <summary>
        /// Resume game session
        /// </summary>
        public static void Resume()
        {
            UnitySocialBridge.PlaySessionResume();
        }

        /// <summary>
        /// Checks whether session is active or not
        /// </summary>
        /// <returns>True of session is active, False is not active</returns>
        public static bool IsActive()
        {
            return UnitySocialBridge.PlaySessionIsActive();
        }
    }
}
