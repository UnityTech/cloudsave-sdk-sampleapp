namespace UnitySocial
{
namespace Entities
{
    /// <summary>
    /// Structure for user social credentials
    /// </summary>
    public class UnitySocialCredential
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Project ID
        /// </summary>
        public string projectId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Is user Anonymous or not
        /// </summary>
        public bool isAnonymous { get; set; }
    }
}
}
