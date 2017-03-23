using System.Collections.Generic;

namespace UnitySocialCloudSave
{
    public class DatasetUpdates
    {
        public string IdentityId { get; set; }

        public string DatasetName { get; set; }

        public IList<SyncRevision> SyncRevisions { get; set; }

        public string SyncSessionToken { get; set; }

        public List<Record> Records { get; set; }
    }
}