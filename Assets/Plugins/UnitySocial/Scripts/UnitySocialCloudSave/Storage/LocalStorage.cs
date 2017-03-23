using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;
using UnitySocialCloudSave.Utils;
using Logger = UnitySocialCloudSave.Utils.Logger;

namespace UnitySocialCloudSave.Storage
{
    internal class LocalStorage
    {
        private const string DbFileName = @"unitysocial_cloudsave.db";

        private readonly string _directoryPath;

        private readonly string _filePath;

        private readonly Logger _logger;

        public LocalStorage()
        {
            _directoryPath = Application.persistentDataPath;

            var filePath = Path.Combine(_directoryPath, DbFileName);

            _filePath = "URI=file:" + filePath;

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            if (!File.Exists(filePath))
            {
                SqliteConnection.CreateFile(filePath);
            }

            _logger = Logger.GetLogger(GetType());

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();
                const string createDatasetTable = @"CREATE TABLE IF NOT EXISTS Datasets (
                                                    IdentityId TEXT NOT NULL,
                                                    Name TEXT NOT NULL,
                                                    SyncRevisions TEXT,
                                                    UNIQUE (IdentityId, Name))";

                using (var command = new SqliteCommand(createDatasetTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                const string createRecordsTable = @"CREATE TABLE IF NOT EXISTS Records (
                                                  IdentityId TEXT NOT NULL,
                                                  Name TEXT NOT NULL,
                                                  Key TEXT NOT NULL,
                                                  Value TEXT,
                                                  SyncCount INTEGER NOT NULL DEFAULT 0,
                                                  SyncRegion TEXT,
                                                  LastModifiedDate TEXT DEFAULT '0',
                                                  DeviceLastModifiedDate TEXT DEFAULT '0',
                                                  LastModifiedBy TEXT,
                                                  IsDirty INTEGER NOT NULL DEFAULT 1,
                                                  UNIQUE (IdentityId, Name, Key))";

                using (var command = new SqliteCommand(createRecordsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            _logger.DebugFormat("INTERNAL LOG - LocalStorage - Completed setup databas");
        }

        public void CreateDataset(string identityId, string datasetName)
        {
            var syncRevision = GetLastSyncRevision(identityId, datasetName);
            if (syncRevision != null) return;

            const string query = @"INSERT INTO Datasets
                               (IdentityId, Name)
                               VALUES (@IdentityId, @Name)";

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdentityId", identityId);
                        command.Parameters.AddWithValue("@Name", datasetName);
                        command.ExecuteNonQuery();
                    }

                    trx.Commit();
                }
            }
        }

        internal void UpdateOrInsertRecord(SqliteConnection conn, string identityId, string datasetName, Record record)
        {
            const string checkRecordquery = @"SELECT COUNT(*) FROM Records WHERE
                                        IdentityId = @IdentityId AND
                                        Name = @Name AND
                                        Key = @Key";

            bool recordFound = false;

            using (var command = new SqliteCommand(checkRecordquery, conn))
            {
                command.Parameters.AddWithValue("@IdentityId", identityId);
                command.Parameters.AddWithValue("@Name", datasetName);
                command.Parameters.AddWithValue("@Key", record.Key);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        recordFound = reader.GetInt32(0) > 0;
                    }
                }
            }

            if (recordFound)
            {
                const string updateRecordQuery =
                    @"UPDATE Records SET Value = @Value, SyncCount = @SyncCount, SyncRegion = @SyncRegion,
                                           LastModifiedDate = @LastModifiedDate, DeviceLastModifiedDate = @DeviceLastModifiedDate,
                                           LastModifiedBy = @LastModifiedBy, IsDirty = @IsDirty WHERE IdentityId = @IdentityId AND
                                           Name = @Name AND Key = @Key";

                using (var command = new SqliteCommand(updateRecordQuery, conn))
                {
                    command.Parameters.AddWithValue("@Value", record.Value);
                    command.Parameters.AddWithValue("@SyncCount", record.SyncCount);
                    command.Parameters.AddWithValue("@SyncRegion", record.SyncRegion);
                    var date = record.LastModifiedDate.HasValue
                        ? SdkUtils.ConvertDateTimeToTicks(record.LastModifiedDate.Value)
                        : SdkUtils.ConvertDateTimeToTicks(DateTime.Now);
                    command.Parameters.AddWithValue("@LastModifiedDate", date);
                    command.Parameters.AddWithValue("@DeviceLastModifiedDate",
                        SdkUtils.ConvertDateTimeToTicks(DateTime.Now));
                    command.Parameters.AddWithValue("@LastModifiedBy", record.LastModifiedBy);
                    command.Parameters.AddWithValue("@IsDirty", record.IsDirty ? 1 : 0);
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    command.Parameters.AddWithValue("@Key", record.Key);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                const string insertQuery =
                    @"INSERT INTO Records (IdentityId, Name, Key, Value, SyncCount, SyncRegion,
                                     LastModifiedDate, DeviceLastModifiedDate, LastModifiedBy, IsDirty)
                                     VALUES (@IdentityId, @Name, @Key, @Value,
                                     @SyncCount, @SyncRegion, @LastModifiedDate, @DeviceLastModifiedDate, @LastModifiedBy, @IsDirty)";

                using (var command = new SqliteCommand(insertQuery, conn))
                {
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    command.Parameters.AddWithValue("@Key", record.Key);
                    command.Parameters.AddWithValue("@Value", record.Value);
                    command.Parameters.AddWithValue("@SyncCount", record.SyncCount);
                    command.Parameters.AddWithValue("@SyncRegion", record.SyncRegion);
                    var date = record.LastModifiedDate.HasValue
                        ? SdkUtils.ConvertDateTimeToTicks(record.LastModifiedDate.Value)
                        : SdkUtils.ConvertDateTimeToTicks(DateTime.Now);
                    command.Parameters.AddWithValue("@LastModifiedDate", date);
                    command.Parameters.AddWithValue("@DeviceLastModifiedDate",
                        SdkUtils.ConvertDateTimeToTicks(DateTime.Now));
                    command.Parameters.AddWithValue("@LastModifiedBy", record.LastModifiedBy);
                    command.Parameters.AddWithValue("@IsDirty", record.IsDirty ? 1 : 0);
                    command.ExecuteNonQuery();
                }
            }
        }


        public Record GetRecord(string identityId, string datasetName, string key)
        {
            Record record = null;

            const string query = @"SELECT Key, Value, SyncCount, SyncRegion, LastModifiedDate, DeviceLastModifiedDate,
                           LastModifiedBy, IsDirty FROM Records
                           WHERE IdentityId = @IdentityId
                           AND Name = @Name
                           AND Key = @Key";

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    command.Parameters.AddWithValue("@Key", key);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            record = new Record.Builder(reader.GetString(0))
                                .Value(reader.IsDBNull(1) ? string.Empty : reader.GetString(1))
                                .SyncCount(reader.GetInt64(2))
                                .SyncRegion(reader.GetString(3))
                                .LastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(4)))
                                .DeviceLastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(5)))
                                .LastModifiedBy(reader.GetString(6))
                                .IsDirty(reader.GetInt32(7) == 1)
                                .Build();
                        }
                    }
                }
            }

            return record;
        }

        public IList<Record> GetRecords(string identityId, string datasetName)
        {
            const string query = @"SELECT Key, Value, SyncCount, SyncRegion, LastModifiedDate, DeviceLastModifiedDate,
                           LastModifiedBy, IsDirty FROM Records
                           WHERE IdentityId = @IdentityId
                           AND Name = @Name";

            List<Record> records = new List<Record>();

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.HasRows && reader.Read())
                        {
                            records.Add(
                                new Record.Builder(reader.GetString(0))
                                    .Value(reader.IsDBNull(1) ? string.Empty : reader.GetString(1))
                                    .SyncCount(reader.GetInt64(2))
                                    .SyncRegion(reader.GetString(3))
                                    .LastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(4)))
                                    .DeviceLastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(5)))
                                    .LastModifiedBy(reader.GetString(6))
                                    .IsDirty(reader.GetInt32(7) == 1)
                                    .Build()
                            );
                        }
                    }
                }
            }

            return records;
        }

        public IList<Record> GetDirtyRecords(string identityId, string datasetName)
        {
            var syncRevisions = GetLastSyncRevision(identityId, datasetName);

            var records = new List<Record>();
            if (syncRevisions == null) return records;

            const string queryTrueDirty =
                @"SELECT Key, Value, SyncCount, SyncRegion, LastModifiedDate, DeviceLastModifiedDate,
                         LastModifiedBy, IsDirty FROM Records
                         WHERE IdentityId = @IdentityId AND Name = @Name AND IsDirty = @IsDirty";
            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var command = new SqliteCommand(queryTrueDirty, connection))
                {
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    command.Parameters.AddWithValue("@IsDirty", 1);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.HasRows && reader.Read())
                        {
                            records.Add(
                                new Record.Builder(reader.GetString(0))
                                    .Value(reader.IsDBNull(1) ? string.Empty : reader.GetString(1))
                                    .SyncCount(reader.GetInt64(2))
                                    .SyncRegion(reader.GetString(3))
                                    .LastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(4)))
                                    .DeviceLastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(5)))
                                    .LastModifiedBy(reader.GetString(6))
                                    .IsDirty(reader.GetInt32(7) == 1)
                                    .Build()
                            );
                        }
                    }
                }

            }

            foreach (var syncRevision in syncRevisions)
            {
                var datasetSyncCount = syncRevision.SyncCount;
                var datasetSyncRegion = syncRevision.SyncRegion;
                const string query =
                    @"SELECT Key, Value, SyncCount, SyncRegion, LastModifiedDate, DeviceLastModifiedDate,
                            LastModifiedBy, IsDirty FROM Records
                            WHERE IdentityId = @IdentityId
                            AND Name = @Name
                            AND IsDirty = @IsDirty
                            AND SyncRegion = @datasetSyncRegion AND SyncCount > @datasetSyncCount";

                using (var connection = new SqliteConnection(_filePath))
                {
                    connection.Open();

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdentityId", identityId);
                        command.Parameters.AddWithValue("@Name", datasetName);
                        command.Parameters.AddWithValue("@IsDirty", 0);
                        command.Parameters.AddWithValue("@datasetSyncCount", datasetSyncCount);
                        command.Parameters.AddWithValue("@datasetSyncRegion", datasetSyncRegion);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.HasRows && reader.Read())
                            {
                                records.Add(
                                    new Record.Builder(reader.GetString(0))
                                        .Value(reader.IsDBNull(1) ? string.Empty : reader.GetString(1))
                                        .SyncCount(reader.GetInt64(2))
                                        .SyncRegion(reader.GetString(3))
                                        .LastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(4)))
                                        .DeviceLastModifiedDate(SdkUtils.ConvertTicksToDateTime(reader.GetString(5)))
                                        .LastModifiedBy(reader.GetString(6))
                                        .IsDirty(reader.GetInt32(7) == 1)
                                        .Build()
                                );
                            }
                        }
                    }
                }
            }

            return records;
        }

        internal void PutValueInternal(SqliteConnection conn, string identityId, string datasetName, string key, string value)
        {
            Record record = GetRecord(identityId, datasetName, key);

            if (record != null && string.Equals(record.Value, value))
            {
                return;
            }

            bool recordFound = record != null;

            if (recordFound)
            {
                const string updateRecordQuery =
                    @"UPDATE Records SET Value = @Value, DeviceLastModifiedDate = @DeviceLastModifiedDate, IsDirty = @IsDirty
                    WHERE IdentityId = @IdentityId AND Name = @Name AND Key = @Key";

                using (var command = new SqliteCommand(updateRecordQuery, conn))
                {
                    command.Parameters.AddWithValue("@Value", value);
                    command.Parameters.AddWithValue("@DeviceLastModifiedDate", SdkUtils.ConvertDateTimeToTicks(DateTime.Now));
                    command.Parameters.AddWithValue("@IsDirty", 1);
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    command.Parameters.AddWithValue("@Key", key);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                const string insertQuery =
                    @"INSERT INTO Records (IdentityId, Name, Key, Value, SyncCount, SyncRegion,
                                     DeviceLastModifiedDate, LastModifiedBy, IsDirty) VALUES (@IdentityId, @Name,
                                     @Key, @Value, @SyncCount, @SyncRegion, @DeviceLastModifiedDate, @LastModifiedBy, @IsDirty)";
                using (var command = new SqliteCommand(insertQuery, conn))
                {
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    command.Parameters.AddWithValue("@Key", key);
                    command.Parameters.AddWithValue("@Value", value);
                    command.Parameters.AddWithValue("@SyncCount", 0);
                    command.Parameters.AddWithValue("@SyncRegion", "Unknown");
                    command.Parameters.AddWithValue("@DeviceLastModifiedDate", SdkUtils.ConvertDateTimeToTicks(DateTime.Now));
                    command.Parameters.AddWithValue("@LastModifiedBy", string.Empty);
                    command.Parameters.AddWithValue("@IsDirty", 1);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void PutValue(string identityId, string datasetName, string key, string value)
        {
            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                PutValueInternal(connection, identityId, datasetName, key, value);
            }
        }

        public void PutAllValues(string identityId, string datasetName, IDictionary<string, string> values)
        {
            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {
                    foreach (KeyValuePair<string, string> entry in values)
                    {
                        PutValueInternal(connection, identityId, datasetName, entry.Key, entry.Value);
                    }

                    trx.Commit();
                }
            }
        }

        public void PutRecords(string identityId, string datasetName, IList<Record> records)
        {
            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {
                    foreach (Record record in records)
                    {
                        UpdateOrInsertRecord(connection, identityId, datasetName, record);
                    }

                    trx.Commit();
                }
            }
        }

        /// <summary>
        /// After local push to remote, remote will return a list of Records which have been updated by remote
        /// since last SyncCount. Some records may not exist in local and some may be changed by local during this time.
        /// Of course, some will update their SyncCount.
        /// This function deals with these conditions.
        /// </summary>
        public void ConditionallyPutRecords(string identityId, string datasetName,
            IList<Record> remoteRecords, IList<Record> localRecords)
        {
            var localRecordMap = new Dictionary<string, Record>();
            foreach (var record in localRecords)
            {
                localRecordMap[record.Key] = record;
            }

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {

                    foreach (var remoteRecord in remoteRecords)
                    {
                        var currentRecord = GetRecord(identityId, datasetName, remoteRecord.Key);
                        Record localRecord;
                        localRecordMap.TryGetValue(remoteRecord.Key, out localRecord);


                        if (localRecord != null && currentRecord != null)
                        {
                            if (currentRecord.SyncCount != localRecord.SyncCount || !string.Equals(currentRecord.SyncRegion, localRecord.SyncRegion))
                            {
                                continue;
                            }

                            if (!string.Equals(localRecord.Value, currentRecord.Value))
                            {
                                if (string.Equals(remoteRecord.Value, localRecord.Value))
                                {
                                    UpdateOrInsertRecord(connection, identityId, datasetName,
                                        new Record.Builder(remoteRecord.Key)
                                            .Value(currentRecord.Value)
                                            .SyncRegion(remoteRecord.SyncRegion)
                                            .SyncCount(remoteRecord.SyncCount)
                                            .LastModifiedBy(remoteRecord.LastModifiedBy)
                                            .LastModifiedDate(remoteRecord.LastModifiedDate)
                                            .DeviceLastModifiedDate(currentRecord.DeviceLastModifiedDate)
                                            .IsDirty(true)
                                            .Build());
                                }
                            }
                            else
                            {
                                UpdateOrInsertRecord(connection, identityId, datasetName, remoteRecord);
                            }
                        }
                        else
                        {
                            UpdateOrInsertRecord(connection, identityId, datasetName, remoteRecord);
                        }
                    }

                    trx.Commit();
                }
            }
        }

        public void WipeOut()
        {
            const string wipeRecordQuery = @"DELETE FROM Records";
            const string wipeDatasetQuery = @"DELETE FROM Datasets";

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {

                    using (var command = new SqliteCommand(wipeDatasetQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SqliteCommand(wipeRecordQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    trx.Commit();

                }
            }
        }

        public void UpdateDatasetAfterSync(string identityId, string datasetName, IList<SyncRevision> syncRevisionList )
        {
            const string query =
                @"UPDATE Datasets SET SyncRevisions = @SyncRevisions, WHERE IdentityId = @IdentityId AND Name = @Name";

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {

                    using (var command = new SqliteCommand(query, connection))
                    {
                        var syncRevisionsStr = SdkUtils.ConvertSyncRevisionToString(syncRevisionList);
                        command.Parameters.AddWithValue("@SyncRevisions", syncRevisionsStr);
                        command.Parameters.AddWithValue("@IdentityId", identityId);
                        command.Parameters.AddWithValue("@Name", datasetName);
                        command.ExecuteNonQuery();
                    }

                    trx.Commit();

                }
            }
        }

        public IList<SyncRevision> GetLastSyncRevision(string identityId, string datasetName)
        {
            const string query = "SELECT SyncRevisions " +
                                 "FROM Datasets " +
                                 "WHERE IdentityId = @IdentityId AND Name = @Name";

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdentityId", identityId);
                    command.Parameters.AddWithValue("@Name", datasetName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var syncRevisionsStr = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            return SdkUtils.ConvertStringToSyncRevision(syncRevisionsStr);
                        }

                        return null;
                    }
                }
            }
        }

        public void UpdateLastSyncRevision(string identityId, string name, IList<SyncRevision> syncRevisionList)
        {
            var localSyncRevision = GetLastSyncRevision(identityId, name);
            var syncRevisionsStr = SdkUtils.ConvertSyncRevisionToString(syncRevisionList);

            if (localSyncRevision == null)
            {
                const string query = @"INSERT INTO Datasets
                               (IdentityId, Name, SyncRevisions)
                               VALUES (@IdentityId, @Name, @SyncRevisions)";

                using (var connection = new SqliteConnection(_filePath))
                {
                    connection.Open();

                    using (var trx = connection.BeginTransaction())
                    {

                        using (var command = new SqliteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IdentityId", identityId);
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@SyncRevisions", syncRevisionsStr);
                            command.ExecuteNonQuery();
                        }

                        trx.Commit();
                    }
                }
            }
            else
            {
                const string query =
                    @"UPDATE Datasets SET SyncRevisions = @SyncRevisions
                        WHERE IdentityId = @IdentityId AND Name = @Name";

                using (var connection = new SqliteConnection(_filePath))
                {
                    connection.Open();

                    using (var trx = connection.BeginTransaction())
                    {

                        using (var command = new SqliteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@SyncRevisions", syncRevisionsStr);
                            command.Parameters.AddWithValue("@IdentityId", identityId);
                            command.Parameters.AddWithValue("@Name", name);
                            command.ExecuteNonQuery();
                        }

                        trx.Commit();

                    }
                }
            }
        }

        public void ChangeToAnonymousIdentityId(string anonymousIdentityId)
        {
            _logger.DebugFormat("Change IdentityId to {0}.", anonymousIdentityId);
            var unknownIdentityId = SdkUtils.UnknownIdentityId;

            var currDatasetNames = GetDatasetNames(anonymousIdentityId);
            if (currDatasetNames.Count > 0)
            {
                _logger.DebugFormat("Anonymous Id {0} is already existed, should not copy data from unknown identity.",
                    anonymousIdentityId);
                return;
            }

            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var trx = connection.BeginTransaction())
                {
                    string updateIdentityDatasetQuery = "UPDATE Datasets " +
                                                        "SET IdentityId = @IdentityId " +
                                                        "WHERE IdentityId = @unknownIdentityId";
                    using (var command = new SqliteCommand(updateIdentityDatasetQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IdentityId", anonymousIdentityId);
                        command.Parameters.AddWithValue("@unknownIdentityId", unknownIdentityId);
                    }

                    string updateIdentityRecordsQuery = "UPDATE Records " +
                                                        "SET IdentityId = @IdentityId " +
                                                        "WHERE IdentityId = @unknownIdentityId";
                    using (var command = new SqliteCommand(updateIdentityRecordsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IdentityId", anonymousIdentityId);
                        command.Parameters.AddWithValue("@unknownIdentityId", unknownIdentityId);
                    }

                    trx.Commit();
                }
            }
        }

        private List<string> GetDatasetNames(string identityId)
        {
            const string query = "SELECT Name " +
                                 "FROM Datasets " +
                                 "WHERE IdentityId = @IdentityId";

            var datasetNames = new List<string>();
            using (var connection = new SqliteConnection(_filePath))
            {
                connection.Open();

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Identity", identityId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.HasRows && reader.Read())
                        {
                            datasetNames.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return datasetNames;
        }
    }
}