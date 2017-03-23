using System.Collections.Generic;

namespace UnitySocial
{
namespace Entities
{
    /// <summary>
    /// Represents a challenge and its status
    /// </summary>
    public class ChallengeStatus
    {
        /// <summary>
        /// Unique identifier of the challenge
        /// </summary>
        public string id;

        /// <summary>
        /// Name of the challenge, as assigned in the challenge template
        /// </summary>
        public string name;

        /// <summary>
        /// Description of the challenge, as assigned in the challenge template
        /// </summary>
        public string description;
        /// <summary>
        /// Unique identifier of the challenge template
        /// </summary>
        public string templateId;

        /// <summary>
        /// The thumbnail image of the challenge, as assigned in the challenge template
        /// </summary>
        public string templateImageURL;

        /// <summary>
        /// Additional free-form data assigned to the challenge template
        /// </summary>
        public Dictionary<string, object> metadata;

        /// <summary>
        /// An array of the opponents of the current user in the challenge
        /// </summary>
        public ChallengeOpponent[] opponents;

        /// <summary>
        /// The current user's best score in the challenge
        /// </summary>
        public ChallengeScore bestScore;

        /// <summary>
        /// Extra information about the challenge
        /// </summary>
        public Dictionary<string, object> extra;
    }
}
}
