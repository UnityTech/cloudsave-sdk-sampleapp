using System.Collections.Generic;

namespace UnitySocialCloudSave
{
    public interface IDataset
    {
        string IdentityId { get; }

        string Name { get; }

        string Get(string key);

        Record GetRecord(string key);

        IList<Record> GetAllRecords();

        IDictionary<string, string> GetAll();

        void Put(string key, string value);

        void PutAll(IDictionary<string, string> values);

        IList<SyncRevision> GetLastSyncRevision();

        void ResolveConflicts(IList<Record> resolvedConflicts);

        void SynchronizeAsync(ISyncCallback callback);

        void SynchronizeOnConnectivityAsync(ISyncCallback callback);

        void SynchronizeOnWifiOnlyAsync(ISyncCallback callback);
    }

    public class SyncRevision
    {
        public string SyncRegion { get; set; }

        public long SyncCount { get; set; }
    }

    public interface ISyncCallback
    {
        // use dataset.resolveConflict() to resolve the conflicts.
        // return true to ingore the remaining conflicts (i.e. use local records for unresolved conflicts) and continue the sync.
        // return false to cancel the sync.
        bool OnConflict(IDataset dataset, IList<SyncConflict> conflicts);

        void OnError(IDataset dataset, DatasetSyncException syncEx);

        void OnSuccess(IDataset dataset);
    }
}