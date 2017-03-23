using UnitySocial.Events;

namespace UnitySocial
{
    /// <summary>
    /// Interface of credential provider
    /// </summary>
    public interface ICredentialProvider<TCredential>
    {
        /// <summary>
        /// Gets the current credential. It could return null which means not authenticated yet.
        /// It will be cached in the local storage. So it will persist between game restarts.
        /// </summary>
        TCredential currentCredential { get; }

        /// <summary>
        /// callback will be invoked with CurrentCredential at first.
        /// Afterwards, when the CurrentCredential changes (i.e. either UserId or ProjectId changes), callback is invoked.
        /// </summary>
        event GenericCallback<TCredential> onIdentityChanged;

        /// <summary>
        /// Gets or refreshes the current credential.
        /// If the access token is going to expire, refresh token is used to get a new access token.
        /// </summary>
        /// <param name="callback"></param>
        void GetOrRefreshCredentialAsync(GenericCallback<TCredential> callback);
    }
}
