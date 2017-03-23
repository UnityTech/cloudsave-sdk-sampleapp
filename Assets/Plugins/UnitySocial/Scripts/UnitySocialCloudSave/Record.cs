using System;

namespace UnitySocialCloudSave
{
    public sealed class Record
    {
        private readonly string _key;
        private readonly string _value;
        private readonly string _syncRegion;
        private readonly long _syncCount;
        private readonly string _lastModifiedBy;
        private readonly DateTime? _lastModifiedDate;
        private readonly DateTime? _deviceLastModifiedDate;
        private readonly bool _isDirty;

        public string Key
        {
            get { return _key; }
        }

        public string Value
        {
            get { return _value; }
        }

        public string SyncRegion
        {
            get { return _syncRegion; }
        }

        public long SyncCount
        {
            get { return _syncCount; }
        }

        public string LastModifiedBy
        {
            get { return _lastModifiedBy; }
        }

        public DateTime? LastModifiedDate
        {
            get { return _lastModifiedDate; }
        }

        public DateTime? DeviceLastModifiedDate
        {
            get { return _deviceLastModifiedDate; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
        }

        private Record(Builder builder)
        {
            _key = builder.key;

            _value = builder.value;

            _syncRegion = builder.syncRegion;

            _syncCount = builder.syncCount;

            _lastModifiedBy = builder.lastModifiedBy;

            _lastModifiedDate = builder.lastModifiedDate ?? DateTime.Now;

            _deviceLastModifiedDate = builder.deviceLastModifiedDate ?? DateTime.Now;

            _isDirty = builder.isDirty;
        }

        public sealed class Builder
        {
            internal string key { get; private set; }

            internal string value { get; private set; }

            internal string syncRegion { get; private set; }

            internal long syncCount { get; private set; }

            internal string lastModifiedBy { get; private set; }

            internal DateTime? lastModifiedDate { get; private set; }

            internal DateTime? deviceLastModifiedDate { get; private set; }

            internal bool isDirty { get; private set; }

            public Builder(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException("key can't be empty", "key");
                }

                this.key = key;
            }

            public Builder Value(string value)
            {
                this.value = value;
                return this;
            }

            public Builder SyncRegion(string syncRegion)
            {
                this.syncRegion = syncRegion;
                return this;
            }

            public Builder SyncCount(long syncCount)
            {
                this.syncCount = syncCount;
                return this;
            }

            public Builder LastModifiedBy(string lastModifiedBy)
            {
                this.lastModifiedBy = lastModifiedBy;
                return this;
            }

            public Builder LastModifiedDate(DateTime? lastModifiedDate)
            {
                this.lastModifiedDate = lastModifiedDate;
                return this;
            }

            public Builder DeviceLastModifiedDate(DateTime? deviceLastModifiedDate)
            {
                this.deviceLastModifiedDate = deviceLastModifiedDate;
                return this;
            }

            public Builder IsDirty(bool isDirty)
            {
                this.isDirty = isDirty;
                return this;
            }

            public Record Build()
            {
                return new Record(this);
            }
        }
    }
}