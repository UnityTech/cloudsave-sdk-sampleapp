using System.Collections.Generic;

namespace UnitySocialCloudSave.Storage
{
    internal interface IRemoteStorage
    {

        void ListUpdatesAsync(string identityId, string datasetName, IList<SyncRevision> syncRevisions,
            CloudSaveCallback<DatasetUpdates> callback);

        void PutRecordsAsync(string identityId, string datasetName, string syncRevisions, string syncSessionToken,
            IList<Record> records, CloudSaveCallback<DatasetPutResult> callback);
    }
}