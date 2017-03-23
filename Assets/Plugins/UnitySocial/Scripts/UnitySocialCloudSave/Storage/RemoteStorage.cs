using System.Collections.Generic;
using UnityEngine;
using UnitySocialCloudSave.Client;
using UnitySocialCloudSave.Utils;
using UnitySocial;

namespace UnitySocialCloudSave.Storage
{
    internal class RemoteStorage : IRemoteStorage
    {
        private readonly ICloudSaveClient _client;

        public RemoteStorage(ICredentialProvider<UnitySocialCredential> credentialProvider)
        {
            _client = new CloudSaveClient(credentialProvider);
        }

        public void ListUpdatesAsync(string identityId, string datasetName, IList<SyncRevision> syncRevisions,
            CloudSaveCallback<DatasetUpdates> callback)
        {
            var lastSyncRevisionStr = SdkUtils.ConvertSyncRevisionToString(syncRevisions);
            var request = new PullRecordsRequest
            {
                IdentityId = identityId,
                DatasetName = datasetName,
                OldSyncRevisions = lastSyncRevisionStr,
            };

            _client.PullRecordsAsync(request, (err, pullRecordsResponse) =>
            {
                if (err != null)
                {
                    callback(err, null);
                    return;
                }

                var records = pullRecordsResponse.Records;
                var datasetRecords = new List<Record>();
                if (records.Count > 0)
                {
                    foreach (var remoteRecord in records)
                    {
                        datasetRecords.Add(new Record.Builder(remoteRecord.key)
                            .Value(remoteRecord.value)
                            .SyncRegion(remoteRecord.lastModifiedRegion)
                            .SyncCount(remoteRecord.syncCount)
                            .LastModifiedBy(remoteRecord.lastModifiedBy)
                            .LastModifiedDate(SdkUtils.ConvertFromUnixEpochSeconds(remoteRecord.lastModifiedDate))
                            .DeviceLastModifiedDate(SdkUtils.ConvertFromUnixEpochSeconds(remoteRecord.deviceLastModifiedDate))
                            .Build()
                        );
                    }
                }
                callback(null, new DatasetUpdates
                {
                    IdentityId = pullRecordsResponse.identityId,
                    DatasetName = pullRecordsResponse.name,
                    SyncSessionToken = pullRecordsResponse.syncSessionToken,
                    SyncRevisions = SdkUtils.ConvertStringToSyncRevision(pullRecordsResponse.syncCount),
                    Records = datasetRecords
                });
            });
        }

        public void PutRecordsAsync(string identityId, string datasetName, string syncRevisions, string syncSessionToken,
            IList<Record> records, CloudSaveCallback<DatasetPutResult> callback)
        {
            var request = new PushRecordsRequest
            {
                identityId = identityId,
                syncCount = syncRevisions,
                DatasetName = datasetName,
                deviceId = SystemInfo.deviceUniqueIdentifier,
                // TODO: disable syncSession for now.
//                syncSessionToken = syncSessionToken,
            };

            var patches = new List<RecordPatch>();
            foreach (var record in records)
            {
                patches.Add(RecordToPatch(record));
            }

            request.recordPatches = patches.ToArray();

            _client.PushRecordsAsync(request, (err, result) =>
            {
                if (err != null)
                {
                    callback(err, null);
                    return;
                }

                var datasetRecords = new List<Record>();
                foreach (var record in result.records)
                {
                    datasetRecords.Add(new Record.Builder(record.key)
                            .Value(record.value)
                            .SyncCount(record.syncCount)
                            .SyncRegion(record.lastModifiedRegion)
                            .LastModifiedBy(record.lastModifiedBy)
                            .LastModifiedDate(SdkUtils.ConvertFromUnixEpochSeconds(record.lastModifiedDate))
                            .Build()
                    );
                }

                callback(null, new DatasetPutResult
                {
                    Records = datasetRecords,
                    SyncRevisions = SdkUtils.ConvertStringToSyncRevision(result.syncCount)
                });
            });
        }

        private static RecordPatch RecordToPatch(Record record)
        {
            var patch = new RecordPatch
            {
                deviceLastModifiedDate = SdkUtils.ConvertToUnixEpochMilliSeconds(record.DeviceLastModifiedDate),
                key = record.Key,
                value = record.Value
            };

            return patch;
        }
    }
}