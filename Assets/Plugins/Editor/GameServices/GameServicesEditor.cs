using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

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

        private static void FetchData(string url, string dataType, ref string data)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);

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
                Debug.LogWarning(ex.Message);
            }
        }
    }
}
