using System;

namespace UnitySocialCloudSave
{
    public class SyncConflict
    {
        private readonly string _key;
        private readonly Record _remoteRecord;
        private readonly Record _localRecord;

        public string Key
        {
            get { return _key; }
        }

        public Record RemoteRecord
        {
            get { return _remoteRecord; }
        }

        public Record LocalRecord
        {
            get { return _localRecord; }
        }

        public SyncConflict(Record remoteRecord, Record localRecord)
        {
            if (remoteRecord == null)
            {
                throw new ArgumentNullException("remoteRecord");
            }

            if (localRecord == null)
            {
                throw new ArgumentNullException("localRecord");
            }

            if (remoteRecord.Key != localRecord.Key)
            {
                throw new ArgumentException("remoteRecord.Key != localRecord.Key");
            }

            _key = remoteRecord.Key;
            _remoteRecord = remoteRecord;
            _localRecord = localRecord;
        }

        public Record ResolveWithRemoteRecord()
        {
            var record = new Record.Builder(Key)
                .Value(RemoteRecord.Value)
                .SyncCount(RemoteRecord.SyncCount)
                .SyncRegion(RemoteRecord.SyncRegion)
                .LastModifiedDate(RemoteRecord.LastModifiedDate)
                .LastModifiedBy(RemoteRecord.LastModifiedBy)
                .DeviceLastModifiedDate(RemoteRecord.DeviceLastModifiedDate)
                .IsDirty(false)
                .Build();

            return record;
        }

        public Record ResolveWithLocalRecord()
        {
            var record = new Record.Builder(Key)
                .Value(LocalRecord.Value)
                .SyncCount(RemoteRecord.SyncCount)
                .SyncRegion(RemoteRecord.SyncRegion)
                .LastModifiedDate(LocalRecord.LastModifiedDate)
                .LastModifiedBy(LocalRecord.LastModifiedBy)
                .DeviceLastModifiedDate(LocalRecord.DeviceLastModifiedDate)
                .IsDirty(true)
                .Build();

            return record;
        }

        public Record ResolveWithValue(string newValue)
        {
            var record = new Record.Builder(Key)
                .Value(newValue)
                .SyncCount(RemoteRecord.SyncCount)
                .SyncRegion(RemoteRecord.SyncRegion)
                .LastModifiedDate(LocalRecord.LastModifiedDate)
                .LastModifiedBy(LocalRecord.LastModifiedBy)
                .DeviceLastModifiedDate(LocalRecord.DeviceLastModifiedDate)
                .IsDirty(true)
                .Build();

            return record;
        }

    }
}