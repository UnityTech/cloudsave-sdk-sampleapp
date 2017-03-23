#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_ANDROID) || !UNITY_SOCIAL
namespace UnitySocial
{
    public static partial class SocialCore
    {
        private static void Initialize_Internal() {}

        private static void AddTags_Internal(params string[] tags) {}

        private static void RemoveTags_Internal(params string[] tags) {}
    }
}
#endif
