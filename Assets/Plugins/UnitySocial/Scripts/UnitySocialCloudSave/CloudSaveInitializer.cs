using System;
using UnityEngine;
using UnitySocialCloudSave.Client;

namespace UnitySocialCloudSave
{
    public class CloudSaveInitializer : MonoBehaviour
    {
        private static CloudSaveInitializer _instance = null;

        private CloudSaveInitializer()
        {
        }

        public static void AttachToGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            gameObject.AddComponent<CloudSaveInitializer>();
            Debug.Log("Attached CloudSave Initializer to " + gameObject.name);
        }

        public static CloudSaveInitializer Instance
        {
            get { return _instance; }
        }

        public void Awake()
        {
            if (_instance == null)
            {
                // singleton instance
                _instance = this;

                DontDestroyOnLoad(this);

                gameObject.AddComponent<NetworkReachabilityMonitor>();
                gameObject.AddComponent<WebRequestDispatcher>();
            }
            else
            {
                if (this != _instance)
                {
                    Destroy(this);
                }
            }
        }
    }
}