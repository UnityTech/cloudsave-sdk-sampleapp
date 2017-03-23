using System;
using System.Text;
using UnityEngine;
using UnitySocialCloudSave.Impl;
using UnitySocialCloudSave.Utils;
using Logger = UnitySocialCloudSave.Utils.Logger;
using UnitySocial;
using UnitySocial.Entities;

namespace UnitySocialCloudSave.Client
{
    internal class CloudSaveClient : ICloudSaveClient
    {
        private readonly ICredentialProvider<UnitySocialCredential> _credentialProvider;

        private readonly Logger _logger;

        public CloudSaveClient(ICredentialProvider<UnitySocialCredential> credentialProvider)
        {
            _credentialProvider = credentialProvider;
            _logger = Logger.GetLogger(GetType());
        }

        public void PullRecordsAsync(PullRecordsRequest request, CloudSaveCallback<PullRecordsResponse> callback)
        {
            if (Logger.LoggingConfig.LogHttpOption == LogHttpOption.Always)
                _logger.DebugFormat("INTERNAL LOG - Pull request is : {0}", JsonUtility.ToJson(request));

            _credentialProvider.GetOrRefreshCredentialAsync((err, credential) =>
            {
                if (err != null)
                {
                    var exception = err as Exception;
                    callback(
                        exception != null
                            ? new CredentialException("Failed to GetOrRefresh credential.", exception)
                            : new CredentialException(err.ToString()), null);
                    return;
                }

                SuperAgent.Get(Endpoints.CloudSaveEndpoint + "/identities/" + request.IdentityId + "/datasets/" +
                               request.DatasetName + "/records")
                    .Query("syncCount", request.OldSyncRevisions)
                    .Set("Authorization", "Bearer " + credential.token)
                    .End((agentErr, response) =>
                    {
                        if (agentErr != null)
                        {
                            callback(agentErr, null);
                            return;
                        }

                        // todo:
                        // call _credentialProvider.Logout() in case of invalid token.

                        var jsonStr = Encoding.UTF8.GetString(response.Body);

                        if (Logger.LoggingConfig.LogHttpOption == LogHttpOption.Always)
                            _logger.DebugFormat("Pull response is : {0}", jsonStr);

                        var pullRecordsResponse = JsonUtility.FromJson<PullRecordsResponse>(jsonStr);

                        callback(null, pullRecordsResponse);
                    });
            });
        }

        public void PushRecordsAsync(PushRecordsRequest request, CloudSaveCallback<PushRecordsResponse> callback)
        {
            if (Logger.LoggingConfig.LogHttpOption == LogHttpOption.Always)
                _logger.DebugFormat("INTERNAL LOG - Push request is : {0}", JsonUtility.ToJson(request));

            _credentialProvider.GetOrRefreshCredentialAsync((err, credential) =>
            {
                if (err != null)
                {
                    var exception = err as Exception;
                    callback(
                        exception != null
                            ? new CredentialException("Failed to GetOrRefresh credential.", exception)
                            : new CredentialException(err.ToString()), null);
                    return;
                }

                byte[] requestBody = request.ToByteArray();

                SuperAgent.Post(Endpoints.CloudSaveEndpoint + "/identities/" + request.identityId + "/datasets/" +
                                request.DatasetName)
                    .Set("Authorization", "Bearer " + credential.token)
                    .Set("Content-Type", "application/json; charset=utf-8")
                    .Send(requestBody)
                    .End((agentErr, response) =>
                    {
                        if (agentErr != null)
                        {
                            callback(agentErr, null);
                            return;
                        }

                        // todo:
                        // handle statusCode != 200.
                        // call _credentialProvider.Logout() in case of invalid token.

                        var jsonStr = Encoding.UTF8.GetString(response.Body);

                        if (Logger.LoggingConfig.LogHttpOption == LogHttpOption.Always)
                            _logger.DebugFormat("INTERNAL LOG - Push response is : {0}", jsonStr);

                        var pushRecordsResponse = JsonUtility.FromJson<PushRecordsResponse>(jsonStr);

                        callback(null, pushRecordsResponse);
                    });
            });
        }
    }
}