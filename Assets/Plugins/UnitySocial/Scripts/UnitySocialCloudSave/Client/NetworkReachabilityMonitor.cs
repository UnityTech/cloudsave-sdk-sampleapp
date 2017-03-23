using UnityEngine;

namespace UnitySocialCloudSave.Client
{
    internal delegate void OnNetworkReachabilityChanged(NetworkReachability networkReachability);

    internal class NetworkReachabilityMonitor : MonoBehaviour
    {
        public static event OnNetworkReachabilityChanged OnNetworkReachabilityChanged;

        private const float UpdateInterval = 0.1f;

        private float _nextUpdateTime;

        private NetworkReachability _currentNetworkReachability;

        private NetworkReachabilityMonitor()
        {
        }

        public void Awake()
        {
            _nextUpdateTime = Time.unscaledTime + UpdateInterval;
        }

        public void Update()
        {
            if (Time.unscaledTime >= _nextUpdateTime)
            {
                UpdateInternal();
                _nextUpdateTime += UpdateInterval;
            }
        }

        private void UpdateInternal()
        {
            var networkReachability = Application.internetReachability;
            if (_currentNetworkReachability != networkReachability)
            {
                _currentNetworkReachability = networkReachability;
                if (OnNetworkReachabilityChanged != null)
                {
                    OnNetworkReachabilityChanged.Invoke(_currentNetworkReachability);
                }
            }
        }
    }
}