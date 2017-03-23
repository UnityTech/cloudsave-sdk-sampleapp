using System;
using System.Collections.Generic;

namespace UnitySocial
{
namespace Internal
{
    internal class CallbackManager
    {
        private Dictionary<uint, Delegate> s_Callbacks = new Dictionary<uint, Delegate>();
        private uint s_NextCallbackId = 1;

        public uint Push(Delegate callback)
        {
            lock (s_Callbacks)
            {
                s_Callbacks.Add(s_NextCallbackId, callback);

                return s_NextCallbackId++;
            }
        }

        public bool TryPop<T>(uint callbackId, out T callback) where T : class
        {
            Delegate rawCallback;
            bool hasCallback = false;

            lock (s_Callbacks)
            {
                hasCallback = s_Callbacks.TryGetValue(callbackId, out rawCallback) && rawCallback.GetType().IsAssignableFrom(typeof(T));

                if (hasCallback)
                {
                    s_Callbacks.Remove(callbackId);
                }
            }

            if (hasCallback)
            {
                callback = (T) (object) rawCallback;
                return true;
            }

            callback = null;
            return false;
        }
    }
}
}
