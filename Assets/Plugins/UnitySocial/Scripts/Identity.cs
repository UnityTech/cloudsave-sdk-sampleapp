using System.Collections.Generic;
using UnitySocialInternal;

namespace UnitySocial
{
    public delegate void Callback<TResult>(object err, TResult result);

    /// <summary>
    /// Represents a user in Unity Social
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier of the user
        /// </summary>
        public string id;

        /// <summary>
        /// The username of the user
        /// </summary>
        public string username;

        /// <summary>
        /// URL of the avatar image of the user
        /// </summary>
        public string avatarURL;
    }

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
        event Callback<TCredential> onIdentityChanged;

        /// <summary>
        /// Gets or refreshes the current credential.
        /// If the access token is going to expire, refresh token is used to get a new access token.
        /// </summary>
        /// <param name="callback"></param>
        void GetOrRefreshCredentialAsync(Callback<TCredential> callback);
    }

    public class UnitySocialCredential
    {
        public string userId { get; set; }

        public string username { get; set; }

        public string projectId { get; set; }

        public string token { get; set; }

        public bool isAnonymous { get; set; }
    }


    public class Identity : ICredentialProvider<UnitySocialCredential>
    {
        private static Identity s_DefaultProvider;
        private static Dictionary<uint, Callback<UnitySocialCredential>> s_GetOrRefreshCredentialAsyncCallbacks;
        private static uint s_CurrentCallbackId = 1;
        private static UnitySocialCredential s_CurrentCredential = null;

        public event Callback<UnitySocialCredential> onIdentityChanged;

        public UnitySocialCredential currentCredential
        {
            get
            {
                return s_CurrentCredential;
            }
        }

        public static Identity DefaultProvider()
        {
            if (s_DefaultProvider == null)
            {
                s_DefaultProvider = new Identity();
            }
            return s_DefaultProvider;
        }

        public void SetCredential(object error, UnitySocialCredential credential)
        {
            s_CurrentCredential = credential;

            if (onIdentityChanged != null)
            {
                onIdentityChanged(error, currentCredential);
            }
        }

        public void GetOrRefreshCredentialAsync(Callback<UnitySocialCredential> callback)
        {
            if (s_GetOrRefreshCredentialAsyncCallbacks == null)
            {
                s_GetOrRefreshCredentialAsyncCallbacks = new Dictionary<uint, Callback<UnitySocialCredential>>();
            }

            uint callbackId = s_CurrentCallbackId++;
            s_GetOrRefreshCredentialAsyncCallbacks.Add(callbackId, callback);

            #if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            UnitySocialBridge.UnitySocialUnityMessageReceived("getOrRenewCredentials", "{\"callbackId\":" + callbackId + "}");
            #endif
        }

        public void CallGetOrRefreshCredentialAsyncCallback(uint id, object err, UnitySocialCredential credential)
        {
            if (s_GetOrRefreshCredentialAsyncCallbacks.ContainsKey(id))
            {
                s_GetOrRefreshCredentialAsyncCallbacks[id](err, credential);
                s_GetOrRefreshCredentialAsyncCallbacks.Remove(id);
            }
            s_CurrentCredential = credential;
        }
    }
}
