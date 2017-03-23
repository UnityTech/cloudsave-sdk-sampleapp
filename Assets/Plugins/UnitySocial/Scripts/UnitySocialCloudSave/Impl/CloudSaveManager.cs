using System;
using System.Text.RegularExpressions;
using UnitySocialCloudSave.Storage;
using UnitySocialCloudSave.Utils;
using UnitySocial;

namespace UnitySocialCloudSave.Impl
{
    internal class CloudSaveManager
    {
        private readonly ICredentialProvider<UnitySocialCredential> _credentialProvider;

        private readonly LocalStorage _local;

        private readonly RemoteStorage _remote;

        public CloudSaveManager(ICredentialProvider<UnitySocialCredential> credentialProvider)
        {
            if (credentialProvider == null)
            {
                throw new ArgumentNullException("credentialProvider");
            }

            _credentialProvider = credentialProvider;

            _local = new LocalStorage();

            _remote = new RemoteStorage(credentialProvider);
        }

        public IDataset OpenOrCreateDataset(string datasetName)
        {
            if (!Regex.IsMatch(datasetName, "^[a-zA-Z0-9_.:-]{1,128}$"))
            {
                throw new ArgumentException("Invalid dataset name");
            }

            var currCredential = _credentialProvider.currentCredential;
            var identityId = currCredential != null
                ? currCredential.userId + "_" + currCredential.projectId
                : SdkUtils.UnknownIdentityId;
            var isAnonymousUser = currCredential != null && currCredential.isAnonymous;

            _local.CreateDataset(identityId, datasetName);

            return new Dataset(_credentialProvider, identityId, isAnonymousUser, datasetName, _local, _remote);
        }

        public void WipeOut()
        {
            _local.WipeOut();
        }
    }
}