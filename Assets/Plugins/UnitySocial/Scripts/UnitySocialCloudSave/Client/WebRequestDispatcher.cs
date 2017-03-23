using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UnitySocialCloudSave.Client
{
    internal class WebRequestDispatcher : MonoBehaviour
    {
        private const float UpdateInterval = 0.1f;

        private float _nextUpdateTime;

        private WebRequestDispatcher()
        {
        }

        public void Awake()
        {
            _nextUpdateTime = Time.unscaledTime + UpdateInterval;
        }

        public void Update()
        {
            if (Time.unscaledTime >= _nextUpdateTime)
            {
                UpdateInternal();
                _nextUpdateTime += UpdateInterval;
            }
        }

        private void UpdateInternal()
        {
            var webRequest = WebRequestQueue.Instance.Dequeue();

            var agentRequest = webRequest as SuperAgentRequest;
            if (agentRequest != null)
            {
                StartCoroutine(HandleAgentRequest(agentRequest));
            }
        }

        private static IEnumerator HandleAgentRequest(SuperAgentRequest request)
        {
            var url = request.Url;

            if (request.Queries.Count > 0)
            {
                string queryStr = "?";

                foreach (var query in request.Queries)
                {
                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        queryStr += "&";
                    }

                    queryStr += System.Uri.EscapeDataString(query.Key) + "=" + System.Uri.EscapeDataString(query.Value);
                }

                url += queryStr;
            }

            var unityWebRequest = new UnityWebRequest(url, request.Method);

            if (request.Payload != null && request.Payload.Length > 0)
            {
                unityWebRequest.uploadHandler = new UploadHandlerRaw(request.Payload);
            }

            foreach (var header in request.Headers)
            {
                unityWebRequest.SetRequestHeader(header.Key, header.Value);
            }

            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return unityWebRequest.Send();

            if (unityWebRequest.isError)
            {
                request.Callback(new SyncNetworkException(unityWebRequest.error), null);
            }
            else
            {
                if (unityWebRequest.responseCode / 100 != 2)
                {
                    // parse code from unityWebRequest.downloadHandler.data;
                    ErrorResponseException err;
                    try
                    {
                        var errCode =
                            JsonUtility.FromJson<ServerErrorResponse>(
                                Encoding.UTF8.GetString(unityWebRequest.downloadHandler.data)).errorCode;
                        err = new ErrorResponseException(errCode, (HttpStatusCode) unityWebRequest.responseCode);
                    }
                    catch (Exception)
                    {
                        err = new ErrorResponseException("unknown_response",
                            (HttpStatusCode) unityWebRequest.responseCode);
                    }
                    request.Callback(err, null);
                }
                else
                {
                    request.Callback(null, new SuperAgentResponse
                    {
                        HttpStatusCode = (HttpStatusCode) unityWebRequest.responseCode,
                        Headers = unityWebRequest.GetResponseHeaders(),
                        Body = unityWebRequest.downloadHandler.data
                    });
                }
            }
        }
    }
}