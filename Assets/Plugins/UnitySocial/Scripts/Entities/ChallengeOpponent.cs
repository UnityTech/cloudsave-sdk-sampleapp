namespace UnitySocial
{
namespace Entities
{
    /// <summary>
    /// Represents an opponent in a challenge
    /// </summary>
    public class ChallengeOpponent
    {
        /// <summary>
        /// The user object of the opponent
        /// </summary>
        public User user;

        /// <summary>
        /// The score of the opponent
        /// </summary>
        public ChallengeScore score;
    }
}
}
