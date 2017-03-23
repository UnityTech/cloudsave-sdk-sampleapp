using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnitySocial.Internal;

namespace UnitySocial
{
    [CustomEditor(typeof(UnitySocialSettings))]
    public class UnitySocialSettingsEditor : Editor
    {
        private const decimal kMinimumRequirediOSVersion = 7.0m;
        private const string kSettingsFile = "UnitySocialSettings";
        private const string kSettingsFileExtension = ".asset";

        private static GUIContent s_LabelClientId = new GUIContent("Project ID");
        private static GUIContent s_LabelSupport_iOS = new GUIContent("iOS enabled [?]", "Check to enable Unity Social on iOS devices");
        private static GUIContent s_LabelSupport_Android = new GUIContent("Android enabled [?]", "Check to enable Unity Social on Android devices");
        private static GUIContent s_LabelAndroidPushNotifications = new GUIContent("Push Notifications");
        private static GUIContent s_LabelAndroidFCMSenderID = new GUIContent("Sender ID");

        private static string[] s_AndroidPushNotificationBackends = new string[]
        {
            "Disabled", "Firebase",
        };

        private UnitySocialSettings m_CurrentSettings = null;
        private bool m_IOSSupportEnabled;
        private bool m_AndroidSupportEnabled;

        [MenuItem("Edit/Unity Social/Settings")]
        public static void ShowSettings()
        {
            UnitySocialSettings settingsInstance = LoadSettings();

            if (settingsInstance == null)
            {
                settingsInstance = CreateSettings();
            }

            if (settingsInstance != null)
            {
                Selection.activeObject = settingsInstance;
            }
        }

        public static UnitySocialSettings LoadSettings()
        {
            return (UnitySocialSettings) Resources.Load(kSettingsFile);
        }

        public override void OnInspectorGUI()
        {
            try
            {
                // Might be null when this gui is open and this file is being reimported
                if (target == null)
                {
                    Selection.activeObject = null;
                    return;
                }

                m_CurrentSettings = (UnitySocialSettings) target;

                if (m_CurrentSettings != null)
                {
                    bool settingsValid = m_CurrentSettings.isValid;

                    EditorGUILayout.HelpBox("1) Enter your game credentials", MessageType.None);

                    if (!m_CurrentSettings.isValid)
                    {
                        EditorGUILayout.HelpBox("Invalid or missing game credentials, Unity Social disabled.", MessageType.Error);
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(s_LabelClientId, GUILayout.Width(108), GUILayout.Height(18));
                    string newClientId = TrimmedText(EditorGUILayout.TextField(m_CurrentSettings.clientId));
                    if (m_CurrentSettings.clientId != newClientId)
                    {
                        m_CurrentSettings.clientId = newClientId;
                        GameServicesEditor.BakeGameServicesData();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("2) Enable Unity Social on these platforms", MessageType.None);

                    EditorGUILayout.BeginVertical();

                    bool validityChanged = m_CurrentSettings.isValid != settingsValid;
                    bool selectedPlatformsChanged = false;

                    m_IOSSupportEnabled = EditorGUILayout.Toggle(s_LabelSupport_iOS, m_CurrentSettings.iosSupportEnabled);
                    m_AndroidSupportEnabled = EditorGUILayout.Toggle(s_LabelSupport_Android, m_CurrentSettings.androidSupportEnabled);

                    if (GUILayout.Button("Refresh Baked Game Services Data"))
                    {
                        GameServicesEditor.BakeGameServicesData();
                    }

                    if (m_IOSSupportEnabled != m_CurrentSettings.iosSupportEnabled)
                    {
                        selectedPlatformsChanged = true;
                        m_CurrentSettings.iosSupportEnabled = m_IOSSupportEnabled;

                        if (m_IOSSupportEnabled)
                        {
                            FixiOSVersion();
                        }

                        GameServicesEditor.BakeGameServicesData();
                        EditorUtility.SetDirty(m_CurrentSettings);
                    }
                    else if (m_AndroidSupportEnabled != m_CurrentSettings.androidSupportEnabled)
                    {
                        EnableAndroidSupport(m_AndroidSupportEnabled);
                        UnitySocialAndroidDependencies.SetAndroidManifestConfig(m_CurrentSettings);
                        selectedPlatformsChanged = true;
                        m_CurrentSettings.androidSupportEnabled = m_AndroidSupportEnabled;

                        GameServicesEditor.BakeGameServicesData();
                        if (m_AndroidSupportEnabled)
                        {
                            AndroidSdkVersions sdkVer = PlayerSettings.Android.minSdkVersion;

                            if (sdkVer < AndroidSdkVersions.AndroidApiLevel16)
                            {
                                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
                                Debug.Log("Unity Social requires minimum Android API level 16. API minSdkVersion updated to 16.");
                            }
                        }

                        EditorUtility.SetDirty(m_CurrentSettings);
                    }

                    if (m_AndroidSupportEnabled)
                    {
                        AndroidPushNotificationSettings();

                        if (GUILayout.Button("Force Import Android dependencies"))
                        {
                            UnitySocialAndroidDependencies.PlayServicesImport();
                        }
                    }

                    if (validityChanged || selectedPlatformsChanged)
                    {
                        UnitySocialPostprocessor.ValidateState(m_CurrentSettings);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            catch (Exception)
            {
            }
        }

        private static void FixiOSVersion()
        {
            #if UNITY_5_3 || UNITY_5_4
            iOSTargetOSVersion v = PlayerSettings.iOS.targetOSVersion;

            if (v < iOSTargetOSVersion.iOS_7_0 || v == iOSTargetOSVersion.Unknown)
            {
                PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_7_0;
                Debug.Log("Unity Social requires minimum iOS 7.0. Target OS version updated to 7.0");
            }
            #else
            decimal version;

            if (!decimal.TryParse(PlayerSettings.iOS.targetOSVersionString, out version) || version < kMinimumRequirediOSVersion)
            {
                PlayerSettings.iOS.targetOSVersionString = kMinimumRequirediOSVersion.ToString();
                Debug.Log(string.Format("Unity Social requires minimum iOS {0}. Target OS version updated to {0}", kMinimumRequirediOSVersion));
            }
            #endif
        }

        private static string TrimmedText(string txt)
        {
            if (txt != null)
            {
                return txt.Trim();
            }
            return "";
        }

        private static UnitySocialSettings CreateSettings()
        {
            UnitySocialSettings everyplaySettings = (UnitySocialSettings) ScriptableObject.CreateInstance(typeof(UnitySocialSettings));

            if (everyplaySettings != null)
            {
                if (!Directory.Exists(System.IO.Path.Combine(Application.dataPath, "Plugins/UnitySocial/Resources")))
                {
                    AssetDatabase.CreateFolder("Assets/Plugins/UnitySocial", "Resources");
                }

                AssetDatabase.CreateAsset(everyplaySettings, "Assets/Plugins/UnitySocial/Resources/" + kSettingsFile + kSettingsFileExtension);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return everyplaySettings;
            }

            return null;
        }

        private static void EnableAndroidSupport(bool enabled)
        {
            PluginImporter[] pluginImporters = PluginImporter.GetAllImporters();
            foreach (PluginImporter pluginImporter in pluginImporters)
            {
                if (pluginImporter.assetPath.Contains("Plugins/UnitySocial/Native/Android"))
                {
                    pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, enabled);
                }
            }
        }

        private string ParseGoogleServicesJson()
        {
            #if UNITY_SOCIAL
            try
            {
                string file = EditorUtility.OpenFilePanel("Open google-services.json", "", "json");
                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    string fileContent = File.ReadAllText(file);
                    var dict = UnitySocial.Tools.DictionaryExtensions.JsonToDictionary(fileContent);

                    try
                    {
                        var clients = dict["client"] as List<object>;
                        var client = clients[0] as Dictionary<string, object>;
                        var clientInfo = client["client_info"] as Dictionary<string, object>;
                        var androidClientInfo = clientInfo["android_client_info"] as Dictionary<string, object>;
                        var packageName = androidClientInfo["package_name"] as string;

                        #if UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                        var currentPackageName = PlayerSettings.bundleIdentifier;
                        #else
                        var currentPackageName = PlayerSettings.applicationIdentifier;
                        #endif

                        if (packageName != currentPackageName)
                        {
                            string msg = string.Format("Bundle name mismatch in {0} (Unity Project: {1} JSON: {2}). Ignoring.",
                                    file, currentPackageName, packageName);
                            throw new NotSupportedException(msg);
                        }

                        var projectInfo = dict["project_info"] as Dictionary<string, object>;
                        return (string) projectInfo["project_number"];
                    }
                    catch (KeyNotFoundException e)
                    {
                        Debug.LogWarningFormat("Error parsing file {0}. {1}", file, e.Message);
                    }
                    catch (NotSupportedException e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Something went wrong... {0}", e.Message);
            }
            #else
            Debug.Log("Unity Social is not enabled. Make sure you have selected iOS or Android as the Unity build platform.");
            #endif

            return null;
        }

        private void OnDisable()
        {
            if (m_CurrentSettings != null)
            {
                EditorUtility.SetDirty(m_CurrentSettings);
                m_CurrentSettings = null;
            }
        }

        private void AndroidPushNotificationSettings()
        {
            EditorGUILayout.LabelField(s_LabelAndroidPushNotifications, GUILayout.Width(108), GUILayout.Height(18));
            string backend = m_CurrentSettings.androidPushNotificationBackend;
            int index = Array.FindIndex(s_AndroidPushNotificationBackends, name => name.Equals(backend));
            if (index == -1)
            {
                index = 0;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Backend");
            index = EditorGUILayout.Popup(index, s_AndroidPushNotificationBackends);
            EditorGUILayout.EndHorizontal();
            if (index > 0)
            {
                backend = s_AndroidPushNotificationBackends[index];
            }
            else
            {
                backend = "";
            }
            if (backend != m_CurrentSettings.androidPushNotificationBackend)
            {
                m_CurrentSettings.androidPushNotificationBackend = backend;
                EditorUtility.SetDirty(m_CurrentSettings);
            }

            string senderId = m_CurrentSettings.androidPushNotificationSenderId;
            if (backend.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(s_LabelAndroidFCMSenderID, GUILayout.ExpandWidth(true));
                senderId = TrimmedText(EditorGUILayout.TextField(senderId));
                EditorGUILayout.EndHorizontal();
            }
            if (backend.Equals("Firebase") && GUILayout.Button("Load from google-services.json"))
            {
                senderId = ParseGoogleServicesJson();
            }

            if (senderId != null && senderId != m_CurrentSettings.androidPushNotificationSenderId)
            {
                m_CurrentSettings.androidPushNotificationSenderId = senderId;
                EditorUtility.SetDirty(m_CurrentSettings);
            }
        }
    }
}
