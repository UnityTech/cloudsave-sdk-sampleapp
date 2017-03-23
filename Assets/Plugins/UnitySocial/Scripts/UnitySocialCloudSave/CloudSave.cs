using UnitySocialCloudSave.Impl;
using Logger = UnitySocialCloudSave.Utils.Logger;
using UnitySocial;

namespace UnitySocialCloudSave
{
    public static class CloudSave
    {
        private static readonly CloudSaveManager Manager;

        private static readonly Logger _logger = Logger.GetLogger(typeof(CloudSave));

        static CloudSave()
        {
            var credentialProvider = Identity.defaultProvider;
            Manager = new CloudSaveManager(credentialProvider);
        }

        public static IDataset OpenOrCreateDataset(string datasetName)
        {
            return Manager.OpenOrCreateDataset(datasetName);
        }

        public static void WipeOut()
        {
            _logger.DebugFormat("Internal Log - All Data Wiped");
            Manager.WipeOut();
        }
    }
}