#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR && UNITY_SOCIAL
using Debug = UnityEngine.Debug;
using UnitySocial.Internal;

namespace UnitySocial
{
    public static partial class SocialCore
    {
        private static void Initialize_Internal()
        {
            // If s_UnitySocialInstance is not yet initialized, calling UnitySocialInstance property getter will trigger the initialization
            if (UnitySocialBridge.GetBridge() == null)
            {
                Debug.Log("Unable to initialize Unity Social. Unity Social is not enabled or available for this platform.");
            }
        }

        private static void AddTags_Internal(params string[] tags)
        {
            if (UnitySocialBridge.GetBridge() != null && tags != null && tags.Length > 0)
            {
                UnitySocialBridge.UnitySocialAddTags(UnitySocial.Tools.Json.Serialize(tags));
            }
        }

        private static void RemoveTags_Internal(params string[] tags)
        {
            if (UnitySocialBridge.GetBridge() != null && tags != null && tags.Length > 0)
            {
                UnitySocialBridge.UnitySocialRemoveTags(UnitySocial.Tools.Json.Serialize(tags));
            }
        }
    }
}
#endif
