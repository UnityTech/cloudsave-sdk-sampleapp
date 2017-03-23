using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnitySocialCloudSave.Client
{
    public class PullRecordsRequest
    {
        public string IdentityId { get; set; }
        public string DatasetName { get; set; }
        public string OldSyncRevisions { get; set; }
    }

    [Serializable]
    public class PullRecordsResponse
    {
        public string identityId;
        public string name;
        public string syncCount;
        public string syncSessionToken;
        public Record[] records;

        public List<Record> Records
        {
            get
            {
                List<Record> list = new List<Record>();
                foreach (var record in records)
                {
                    list.Add(record);
                }

                return list;
            }
        }
    }

    [Serializable]
    public class Record
    {
        public string key;
        public string value;
        public long syncCount;
        public string lastModifiedRegion;
        public string lastModifiedBy;
        public string lastModifiedDate;
        public string deviceLastModifiedDate;
    }

    [Serializable]
    public class PushRecordsRequest
    {
        public string identityId;
        public string deviceId;
        public string syncCount;
        // TODO: disable syncSessionToken for now.
//        public string syncSessionToken;
        public RecordPatch[] recordPatches;

        public string DatasetName { get; set; }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(this));
        }
    }

    [Serializable]
    public class RecordPatch
    {
        public string key;
        public string value;
        public string deviceLastModifiedDate;
    }

    [Serializable]
    public class PushRecordsResponse
    {
        public string syncCount;
        public Record[] records;
    }

    [Serializable]
    public class ServerErrorResponse
    {
        public string errorCode;

        public string message;

        public string target;

        public string details;
    }
}
