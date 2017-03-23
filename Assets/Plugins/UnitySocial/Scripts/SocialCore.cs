using UnityEngine.Events;
using UnitySocial.Events;
using UnitySocial.Internal;

namespace UnitySocial
{
    /// <summary>
    /// Social core
    /// </summary>
    public partial class SocialCore
    {
        private static UnityEvent s_GameShouldPause = new UnityEvent();
        private static UnityEvent s_GameShouldResume = new UnityEvent();
        private static SocialCoreInitializedEvent s_Initialized = new SocialCoreInitializedEvent();
        private static SocialCoreRewardClaimedEvent s_RewardClaimed = new SocialCoreRewardClaimedEvent();

        /// <summary>
        /// Occurs when Unity Social view is opened
        /// </summary>
        public static UnityEvent onGameShouldPause { get { return s_GameShouldPause; } }

        /// <summary>
        /// Occurs when Unity Social view is hidden
        /// </summary>
        public static UnityEvent onGameShouldResume { get { return s_GameShouldResume; } }

        /// <summary>
        /// Occurs when Unity Social is initialized
        /// </summary>
        public static SocialCoreInitializedEvent onInitialized { get { return s_Initialized; } }

        /// <summary>
        /// Occurs when the user has earned a reward
        /// </summary>
        public static SocialCoreRewardClaimedEvent onRewardClaimed { get { return s_RewardClaimed; } }

        /// <summary>
        /// Gets or sets a boolean that determines if Unity player should be paused automatically when Unity Social view is opened
        /// </summary>
        public static bool pauseEngineAutomatically
        {
            get;
            set;
        }

        /// <summary>
        /// Is Unity Social supported on this device
        /// </summary>
        public static bool isSupported
        {
            get
            {
                return UnitySocialBridge.UnitySocialIsSupported();
            }
        }

        /// <summary>
        /// Is Unity Social initialized and ready to be used
        /// </summary>
        public static bool isReady
        {
            get
            {
                return UnitySocialBridge.UnitySocialIsReady();
            }
        }

        static SocialCore()
        {
            pauseEngineAutomatically = true;
        }

        /// <summary>
        /// Initializes Unity Social
        /// </summary>
        public static void Initialize()
        {
            Initialize_Internal();
        }

        /// <summary>
        /// Set manifest server URL
        /// </summary>
        /// <param name="manifestServer">URL for manifest server</param>
        public static void SetManifestServer(string manifestServer)
        {
            UnitySocialBridge.UnitySocialSetManifestServer(manifestServer);
        }

        /// <summary>
        /// Add tags
        /// </summary>
        /// <param name="tags">Array of tags</param>
        public static void AddTags(params string[] tags)
        {
            AddTags_Internal(tags);
        }

        /// <summary>
        /// Remove tags
        /// </summary>
        /// <param name="tags">Array of tags</param>
        public static void RemoveTags(params string[] tags)
        {
            RemoveTags_Internal(tags);
        }
    }
}
