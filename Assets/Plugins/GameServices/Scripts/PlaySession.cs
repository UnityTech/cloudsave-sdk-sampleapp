using System.Collections.Generic;
using UnitySocialInternal;

namespace UnitySocial
{
    public class PlaySession
    {
        public static void SendEvent(Dictionary<string, float> sessionEvent, params string[] tags)
        {
            UnitySocialBridge.PlaySessionSendEvent(sessionEvent, tags);
        }

        public static void SendEvent(string key, float value, params string[] tags)
        {
            UnitySocialBridge.PlaySessionSendEvent(key, value, tags);
        }

        public static void ActivateTag(string tag)
        {
            UnitySocialBridge.PlaySessionActivateTag(tag);
        }

        public static void DeactivateTag(string tag)
        {
            UnitySocialBridge.PlaySessionDeactivateTag(tag);
        }

        public static void Begin()
        {
            UnitySocialBridge.PlaySessionBegin();
        }

        public static void End()
        {
            UnitySocialBridge.PlaySessionEnd();
        }

        public static void Cancel()
        {
            UnitySocialBridge.PlaySessionCancel();
        }

        public static void Pause()
        {
            UnitySocialBridge.PlaySessionPause();
        }

        public static void Resume()
        {
            UnitySocialBridge.PlaySessionResume();
        }

        public static bool IsActive()
        {
            return UnitySocialBridge.PlaySessionIsActive();
        }
    }
}
