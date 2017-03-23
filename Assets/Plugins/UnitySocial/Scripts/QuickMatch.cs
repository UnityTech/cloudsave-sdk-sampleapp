using UnitySocial.Internal;

namespace UnitySocial
{
    /// <summary>
    /// Quick Match API
    /// </summary>
    public static class QuickMatch
    {
        /// <summary>
        /// Starts quick match
        /// </summary>
        public static void Start()
        {
            if (Factory.quickMatch != null)
            {
                Factory.quickMatch.Start();
            }
        }

        /// <summary>
        /// Starts quick match with provided challange template ID
        /// </summary>
        /// <param name="challengeTemplateId">Challenge template ID</param>
        public static void Start(string challengeTemplateId)
        {
            if (Factory.quickMatch != null)
            {
                Factory.quickMatch.Start(challengeTemplateId);
            }
        }
    }
}
