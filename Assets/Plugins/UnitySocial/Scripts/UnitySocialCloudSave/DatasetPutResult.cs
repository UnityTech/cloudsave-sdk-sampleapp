using System.Collections.Generic;

namespace UnitySocialCloudSave
{
    public class DatasetPutResult
    {
       //TODO: may be need IdentityId

        public IList<SyncRevision> SyncRevisions { get; set; }

        public List<Record> Records { get; set; }
    }
}