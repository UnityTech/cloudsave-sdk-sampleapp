using Logger = UnitySocialCloudSave.Utils.Logger;

namespace UnitySocialCloudSave.Impl
{
    internal static class Endpoints
    {
#if DEBUG
        public static string CloudSaveEndpoint;
#else
        public static readonly string CloudSaveEndpoint;
#endif

        public static Logger _logger = Logger.GetLogger(typeof(Endpoints));

        static Endpoints()
        {
#if CLOUDSAVE_PROD
            CloudSaveEndpoint = "https://cloudsave.unity.com";
#elif CLOUDSAVE_INT
            CloudSaveEndpoint = "https://cloudsave-staging.unity.com";
            Debug.Log("CloudSave endpoint is " + CloudSaveEndpoint);
#elif CLOUDSAVE_DEV
            CloudSaveEndpoint = "https://cloudsave-dev.unity.com";
            _logger.DebugFormat("CloudSave endpoint is {0}", CloudSaveEndpoint);
#else
            CloudSaveEndpoint = "http://localhost:8080";
            _logger.DebugFormat("CloudSave endpoint is {0}", CloudSaveEndpoint);
#endif
        }
    }
}