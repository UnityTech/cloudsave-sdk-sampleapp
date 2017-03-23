using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnitySocialCloudSave.Client;
using UnitySocialCloudSave.Storage;
using UnitySocialCloudSave.Utils;
using Logger = UnitySocialCloudSave.Utils.Logger;
using UnitySocial;

namespace UnitySocialCloudSave
{
    public class Dataset : IDataset, IDisposable
    {
        private const int MaxRetry = 3;

        private readonly ICredentialProvider<UnitySocialCredential> _credentialProvider;

        public string IdentityId
        {
            get { return _identityId; }
        }

        public string Name
        {
            get { return _name; }
        }

        private readonly LocalStorage _local;

        private readonly IRemoteStorage _remote;

        private ISyncCallback _lastPendingCallback;

        private ISyncCallback _lastPendingCallbackWifiOnly;

        private long _currentSyncSequenceId;

        private bool _disposed;

        private readonly Logger _logger;
        private readonly string _identityId;
        private readonly bool _isAnonymousUser;
        private readonly string _name;

        internal Dataset(
            ICredentialProvider<UnitySocialCredential> credentialProvider,
            string identityId,
            bool isAnonymousUser,
            string name,
            LocalStorage local,
            IRemoteStorage remote)
        {
            _identityId = identityId;
            _isAnonymousUser = isAnonymousUser;
            _name = name;
            _local = local;
            _remote = remote;
            _credentialProvider = credentialProvider;

            _credentialProvider.onIdentityChanged += OnIdentityChanged;
            NetworkReachabilityMonitor.OnNetworkReachabilityChanged += OnNetworkReachabilityChanged;

            _logger = Logger.GetLogger(GetType());
        }

        private void OnIdentityChanged(object err, UnitySocialCredential result)
        {
            if (err != null)
            {
                throw new CredentialException("Identity change with error: " + err);
            }

            if (result.isAnonymous)
            {
                _local.ChangeToAnonymousIdentityId(result.userId + "_" + result.projectId);
            }
        }

        private void OnNetworkReachabilityChanged(NetworkReachability networkreachability)
        {
            var localPendingCallBack = _lastPendingCallback;
            if (localPendingCallBack != null &&
                networkreachability != NetworkReachability.NotReachable)
            {
                _lastPendingCallback = null;
                StartSynchronizeInternalAsync(localPendingCallBack);
            }

            var localPendingCallBackWifiOnly = _lastPendingCallbackWifiOnly;
            if (localPendingCallBackWifiOnly != null &&
                networkreachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                _lastPendingCallbackWifiOnly = null;
                StartSynchronizeInternalAsync(localPendingCallBackWifiOnly);
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                NetworkReachabilityMonitor.OnNetworkReachabilityChanged -= OnNetworkReachabilityChanged;
                _credentialProvider.onIdentityChanged -= OnIdentityChanged;
                _disposed = true;
            }
        }

        public string Get(string key)
        {
            return GetRecord(key) != null ? GetRecord(key).Value : null;
        }

        public Record GetRecord(string key)
        {
            CheckIdentityId(IdentityId);
            return _local.GetRecord(IdentityId, Name, ValidateRecordKey(key));
        }

        public IList<Record> GetAllRecords()
        {
            CheckIdentityId(IdentityId);
            return _local.GetRecords(IdentityId, Name);
        }

        public IDictionary<string, string> GetAll()
        {
            var dict = new Dictionary<string, string>();
            foreach (var record in GetAllRecords())
            {
                dict.Add(record.Key, record.Value);
            }

            return dict;
        }

        public void Put(string key, string value)
        {
            CheckIdentityId(IdentityId);
            _local.PutValue(IdentityId, Name, ValidateRecordKey(key), value);
        }

        public void PutAll(IDictionary<string, string> values)
        {
            CheckIdentityId(IdentityId);
            foreach (var key in values.Keys)
            {
                ValidateRecordKey(key);
            }

            _local.PutAllValues(IdentityId, Name, values);
        }

        public IList<SyncRevision> GetLastSyncRevision()
        {
            CheckIdentityId(IdentityId);
            return _local.GetLastSyncRevision(IdentityId, Name);
        }

        public void ResolveConflicts(IList<Record> resolvedConflicts)
        {
            CheckIdentityId(IdentityId);
            _local.PutRecords(IdentityId, Name, resolvedConflicts);
        }

        private void CancelPendingCallbacks()
        {
            var localPendingCallback = _lastPendingCallback;
            if (localPendingCallback != null)
            {
                _lastPendingCallback = null;
                localPendingCallback.OnError(this, new SyncCanceledException());
            }

            var localPendingCallbackWifiOnly = _lastPendingCallbackWifiOnly;
            if (localPendingCallbackWifiOnly != null)
            {
                _lastPendingCallbackWifiOnly = null;
                localPendingCallbackWifiOnly.OnError(this, new SyncCanceledException());
            }
        }

        public void SynchronizeAsync(ISyncCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            CancelPendingCallbacks();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                callback.OnError(this, new SyncCanceledException());
                return;
            }


            StartSynchronizeInternalAsync(callback);
        }

        public void SynchronizeOnConnectivityAsync(ISyncCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            CancelPendingCallbacks();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _lastPendingCallback = callback;
                return;
            }

            StartSynchronizeInternalAsync(callback);
        }

        public void SynchronizeOnWifiOnlyAsync(ISyncCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            CancelPendingCallbacks();

            if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                _lastPendingCallbackWifiOnly = callback;
                return;
            }

            StartSynchronizeInternalAsync(callback);
        }

        private void StartSynchronizeInternalAsync(ISyncCallback callback)
        {
            if (SdkUtils.UnknownIdentityId == _identityId || this._isAnonymousUser)
            {
                _logger.DebugFormat("Sync is cancelled since the user has not full login.");
                callback.OnError(this, new SyncCanceledException("Sync is cancelled since user has not login yet."));
                return;
            }
            _currentSyncSequenceId++;
            SynchronizeInternalAsync(callback, _currentSyncSequenceId, MaxRetry);
        }

        private void SynchronizeInternalAsync(ISyncCallback callback, long syncSequenceId, int retry)
        {
            if (retry < 0)
            {
                var e = new SyncConflictingException();

                if (Logger.LoggingConfig.LogInnerMostError)
                    _logger.Error(e, "Sync failed due to retry less than 0");

                callback.OnError(this, new SyncConflictingException());
                return;
            }

            // pull from remote

            var lastSyncRevisions = _local.GetLastSyncRevision(IdentityId, Name);
            _remote.ListUpdatesAsync(IdentityId, Name, lastSyncRevisions,
                (listUpdatesErr, datasetUpdates) =>
                {
                    if (syncSequenceId != _currentSyncSequenceId)
                    {
                        var e = new SyncCanceledException();
                        callback.OnError(this, e);

                        if(Logger.LoggingConfig.LogInnerMostError)
                            _logger.Error(e, "INTERNAL LOG - Sync Failed due to inconsistent syncSequenceId.");

                        return;
                    }

                    if (listUpdatesErr != null)
                    {
                        var e = new SyncNetworkException("Failed to pull from remote", listUpdatesErr);

                        if (Logger.LoggingConfig.LogInnerMostError)
                            _logger.Error(e, "INTERNAL LOG - Failed to pull from remote");

                        callback.OnError(this, e);
                        return;
                    }

                    if (!MergeRemoteRecords(callback, datasetUpdates))
                    {
                        return;
                    }

                    // push to remote.
                    // includes the records whose region is different.
                    var localChanges = _local.GetDirtyRecords(IdentityId, Name);

                    if (localChanges.Count == 0)
                    {
                        callback.OnSuccess(this);
                        return;
                    }

                    _remote.PutRecordsAsync(IdentityId, Name,
                        SdkUtils.ConvertSyncRevisionToString(datasetUpdates.SyncRevisions),
                        datasetUpdates.SyncSessionToken, localChanges, (putErr, putResult) =>
                        {
                            if (syncSequenceId != _currentSyncSequenceId)
                            {
                                var e = new SyncCanceledException();

                                if (Logger.LoggingConfig.LogInnerMostError)
                                    _logger.Error(e, "INTERNAL LOG - Sync failed due to inconsistency of syncSequenceId");

                                callback.OnError(this, e);
                                return;
                            }

                            if (putErr != null)
                            {
                                if (Logger.LoggingConfig.LogInnerMostError)
                                    _logger.Error(putErr, "INTERNAL LOG - Failed to push to remote: {0}", putErr.Message);

                                if (ErrorResponseException.IsDatasetConflict(putErr))
                                {
                                    SynchronizeInternalAsync(callback, syncSequenceId, --retry);
                                    return;
                                }

                                callback.OnError(this, new SyncNetworkException("Failed to push to remote", putErr));
                                return;
                            }

                            _local.ConditionallyPutRecords(IdentityId, Name, putResult.Records, localChanges);
                            _local.UpdateLastSyncRevision(IdentityId, Name, putResult.SyncRevisions);

                            callback.OnSuccess(this);

                        });
                });
        }

        private bool MergeRemoteRecords(ISyncCallback callback, DatasetUpdates datasetUpdates)
        {
            var conflicts = new List<SyncConflict>();
            var remoteUpdates = new List<Record>();
            foreach (var remoteRecord in datasetUpdates.Records)
            {
                var localRecord = _local.GetRecord(IdentityId, Name, remoteRecord.Key);
                if (localRecord == null)
                {
                    remoteUpdates.Add(remoteRecord);
                    continue;
                }

                if (remoteRecord.SyncRegion != localRecord.SyncRegion) // always a conflict if region is different.
                {
                    conflicts.Add(new SyncConflict(remoteRecord, localRecord));
                    continue;
                }

                if (remoteRecord.SyncCount <= localRecord.SyncCount)
                {
                    // ignore remote record if we know for sure remote record is older.
                    continue;
                }

                if (localRecord.IsDirty)
                {
                    conflicts.Add(new SyncConflict(remoteRecord, localRecord));
                }
                else
                {
                    remoteUpdates.Add(remoteRecord);
                }
            }

            _logger.DebugFormat("INTERNAL LOG - {0} records in conflict", conflicts.Count);
            if (conflicts.Count != 0 && !callback.OnConflict(this, conflicts))
            {
                var e = new SyncConflictingException();

                if (Logger.LoggingConfig.LogInnerMostError)
                    _logger.Error(e,"INTERNAL LOG - Conflict resolution failed/cancelled.");

                callback.OnError(this, e);
                return false;
            }

            if (remoteUpdates.Count != 0)
            {
                _logger.DebugFormat("INTERNAL LOG - Save {0} records to local", remoteUpdates.Count);
                _local.PutRecords(IdentityId, Name, remoteUpdates);
            }

            _local.UpdateLastSyncRevision(IdentityId, Name, datasetUpdates.SyncRevisions);

            _logger.DebugFormat("INTERNAL LOG - Update SyncRevision of Dataset to {0}", SdkUtils.ConvertSyncRevisionToString(datasetUpdates.SyncRevisions));
            return true;
        }

        private static string ValidateRecordKey(string key)
        {
            if (string.IsNullOrEmpty(key) || Encoding.UTF8.GetByteCount(key) > 120)
            {
                throw new ArgumentException("Invalid record key: " + key);
            }

            return key;
        }

        private void CheckIdentityId(string identityId)
        {
            var currentCredential = _credentialProvider.currentCredential;
            string currentIdentityId;

            if (currentCredential == null || currentCredential.userId == null || currentCredential.projectId == null)
            {
                currentIdentityId = SdkUtils.UnknownIdentityId;
            }
            else {
                currentIdentityId = string.Format("{0}_{1}", currentCredential.userId, currentCredential.projectId);
            }

            if (!identityId.Equals(currentIdentityId))
            {
                throw new ArgumentException(string.Format("Cannot access local stroage because of wrong identityId: current {0} and want to create {1}", currentIdentityId, identityId));
            }
        }
    }
}