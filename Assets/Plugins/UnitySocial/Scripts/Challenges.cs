using UnitySocial.Events;

namespace UnitySocial
{
    /// <summary>
    /// Challenges
    /// </summary>
    public static class Challenges
    {
        private static ChallengeStartedEvent s_ChallengeStarted = new ChallengeStartedEvent();

        /// <summary>
        /// Occurs when a new challenge should start
        /// </summary>
        public static ChallengeStartedEvent onChallengeStarted { get { return s_ChallengeStarted; } }
    }
}
