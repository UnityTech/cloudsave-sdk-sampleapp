using UnitySocial.Entities;
using UnitySocial.Events;
using UnitySocial.Internal;

namespace UnitySocial
{
    /// <summary>
    /// Identity
    /// </summary>
    public class Identity : ICredentialProvider<UnitySocialCredential>
    {
        private static Identity s_DefaultProvider;
        private static UnitySocialCredential s_CurrentCredential = null;

        /// <summary>
        /// Occurs when identy has changed
        /// </summary>
        public event GenericCallback<UnitySocialCredential> onIdentityChanged;

        /// <summary>
        /// Instance of default credential provider
        /// </summary>
        public static ICredentialProvider<UnitySocialCredential> defaultProvider
        {
            get
            {
                if (s_DefaultProvider == null)
                {
                    s_DefaultProvider = new Identity();
                }
                return s_DefaultProvider;
            }
        }

        /// <summary>
        /// Gets current credentials
        /// </summary>
        public UnitySocialCredential currentCredential
        {
            get
            {
                return s_CurrentCredential;
            }
        }

        /// <summary>
        /// Gets or refreshes the current credential.
        /// If the access token is going to expire, refresh token is used to get a new access token.
        /// </summary>
        /// <param name="callback"></param>
        public void GetOrRefreshCredentialAsync(GenericCallback<UnitySocialCredential> callback)
        {
            #if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            uint callbackId = Factory.callbackManager.Push(callback);
            UnitySocialBridge.UnitySocialUnityMessageReceived("getOrRenewCredentials", "{\"callbackId\":" + callbackId + "}");
            #else
            callback("Unity Social is not enabled", null);
            #endif
        }

        internal void SetCredential(object error, UnitySocialCredential credential)
        {
            s_CurrentCredential = credential;

            if (onIdentityChanged != null)
            {
                onIdentityChanged(error, currentCredential);
            }
        }

        internal void CallGetOrRefreshCredentialAsyncCallback(uint id, object err, UnitySocialCredential credential)
        {
            GenericCallback<UnitySocialCredential> callback;
            if (Factory.callbackManager.TryPop<GenericCallback<UnitySocialCredential>>(id, out callback))
            {
                callback(err, credential);
            }

            SetCredential(err, credential);
        }
    }
}
