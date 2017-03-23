using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnitySocial.Internal;

namespace UnitySocial
{
    public class GameServicesEditor
    {
        private const string kLeaderboardsURL = "https://rules.social.unity.com/leaderboards/";

        public static void BakeGameServicesData()
        {
            string upid = Application.cloudProjectId;
            UnitySocialSettings settings = (UnitySocialSettings) Resources.Load("UnitySocialSettings");

            if (settings != null)
            {
                upid = settings.clientId;
            }

            FetchData(kLeaderboardsURL + upid, "Leaderboards", ref settings.bakedLeaderboards);

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private static bool VerifyCertificateOnline(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus status in chain.ChainStatus)
                {
                    if (status.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        chain.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromMinutes(1);

                        if (!chain.Build((X509Certificate2) certificate))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static void FetchData(string url, string dataType, ref string data)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            RemoteCertificateValidationCallback oldCallback = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = VerifyCertificateOnline;

            try
            {
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.LogError(string.Format("Data '{0}' could not be loaded due to server returning: {1}", dataType, response.StatusCode));
                    return;
                }

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string responseString = reader.ReadToEnd();

                        Debug.Log(responseString);
                        data = responseString;
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.LogWarning(string.Format("Error occured while fetching '{0}'. Message: {1}", dataType, ex.Message));
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = oldCallback;
            }
        }
    }
}
