using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnitySocial.Internal;

namespace UnitySocial
{
    public class UnitySocialAndroidDependencies : AssetPostprocessor
    {
        private const string k_ConfigManifestDir = "Plugins/Android/UnitySocialConfigManifest/";
        private static object s_SvcSupport;

        // Run this from command line in batch mode to prepare dependencies.
        // Useful for building on CI.
        public static void PlayServicesImport()
        {
            RegisterDependencies();
            AssetDatabase.ImportAsset("Assets/PlayServicesResolver", ImportAssetOptions.ForceSynchronousImport);
            Google.VersionHandler.UpdateNow();
            GooglePlayServices.PlayServicesResolver.MenuResolve();
        }

        static UnitySocialAndroidDependencies()
        {
            RegisterDependencies();
        }

        public static void RegisterDependencies()
        {
            UnitySocialSettings settings = (UnitySocialSettings) Resources.Load("UnitySocialSettings");

            if (settings != null && settings.androidSupportEnabled)
            {
                Debug.Log("Registering UnitySocial dependencies.");

                Google.VersionHandler.InvokeInstanceMethod(
                    GetSvcSupport(), "DependOn",
                    new object[] { "com.google.firebase", "firebase-messaging", "9+" },
                    namedArgs: new Dictionary<string, object>() {
                    { "packageIds", new string[] { "extra-google-m2repository" } }
                });

                Google.VersionHandler.InvokeInstanceMethod(
                    GetSvcSupport(), "DependOn",
                    new object[] { "com.android.support", "support-v4", "23.4.0" },
                    namedArgs: new Dictionary<string, object>() {
                    { "packageIds", new string[] { "extra-android-m2repository" } }
                });

                Google.VersionHandler.InvokeInstanceMethod(
                    GetSvcSupport(), "DependOn",
                    new object[] { "com.android.support", "appcompat-v7", "23.0.1" },
                    namedArgs: new Dictionary<string, object>() {
                    { "packageIds", new string[] { "extra-android-m2repository" } }
                });

                Google.VersionHandler.InvokeInstanceMethod(
                    GetSvcSupport(), "DependOn",
                    new object[] { "com.android.support", "recyclerview-v7", "23.4.0" },
                    namedArgs: new Dictionary<string, object>() {
                    { "packageIds", new string[] { "extra-android-m2repository" } }
                });
            }
        }

        public static void Resolve()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                return;
            }

            Type playServicesResolverType = Google.VersionHandler.FindClass(
                    "Google.JarResolver", "GooglePlayServices.PlayServicesResolver");
            if (playServicesResolverType == null)
            {
                Debug.LogWarning("Cannot set trigger resolve...");
                return;
            }

            Google.VersionHandler.InvokeStaticMethod(
                playServicesResolverType, "MenuResolve", new object[] {});
        }

        private static object InstantiateSvcSupport()
        {
            // Setup the resolver using reflection as the module may not be
            // available at compile time.
            Type playServicesSupport = Google.VersionHandler.FindClass(
                    "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
            if (playServicesSupport == null)
            {
                Debug.LogWarning("Cannot find Google.JarResolver...");
                return null;
            }

            return Google.VersionHandler.InvokeStaticMethod(
                playServicesSupport, "CreateInstance",
                new object[] {
                "UnitySocialAndroid",
                EditorPrefs.GetString("AndroidSdkRoot"),
                "ProjectSettings"
            });
        }

        private static object GetSvcSupport()
        {
            s_SvcSupport = s_SvcSupport ?? InstantiateSvcSupport();
            return s_SvcSupport;
        }

        // Handle delayed loading of the dependency resolvers.
        private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromPath)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.Contains("IOSResolver") || asset.Contains("JarResolver"))
                {
                    RegisterDependencies();
                    break;
                }
            }

            UnitySocialSettings settings = UnitySocialSettingsEditor.LoadSettings();
            if (settings == null)
            {
                return;
            }

            PluginImporter[] pluginImporters = PluginImporter.GetAllImporters();
            foreach (PluginImporter pluginImporter in pluginImporters)
            {
                if (pluginImporter.assetPath.Contains("Plugins/UnitySocial/Native/Android"))
                {
                    pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, settings.androidSupportEnabled);
                }
            }

            SetAndroidManifestConfig(settings);
        }

        public static void SetAndroidManifestConfig(UnitySocialSettings settings)
        {
            string configDir = Path.Combine(Application.dataPath, k_ConfigManifestDir);
            if (Directory.Exists(configDir))
            {
                Directory.Delete(configDir, true);
            }

            if (settings.androidSupportEnabled)
            {
                Directory.CreateDirectory(configDir);
                CreateConfigManifestXML(settings);
                CreateConfigProjectProperties();
            }
        }

        private static void CreateConfigManifestXML(UnitySocialSettings settings)
        {
            string configDir = Path.Combine(Application.dataPath, k_ConfigManifestDir);
            string androidManifestXmlPath = Path.Combine(configDir, "AndroidManifest.xml");

            XmlDocument doc = new XmlDocument();

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("android", "android");

            XmlNode manifest = doc.CreateElement("manifest");
            XmlAttribute manifestXmlnsAttribute = doc.CreateAttribute("xmlns:android");
            manifestXmlnsAttribute.Value = "http://schemas.android.com/apk/res/android";
            manifest.Attributes.Append(manifestXmlnsAttribute);
            XmlAttribute manifestPackageAttribute = doc.CreateAttribute("package");
            manifestPackageAttribute.Value = "com.unity.unitysocial.internal";
            manifest.Attributes.Append(manifestPackageAttribute);
            doc.AppendChild(manifest);

            XmlNode application = doc.CreateElement("application");
            manifest.AppendChild(application);

            XmlElement activity = doc.CreateElement("activity");
            XmlAttribute activityNameAttribute = doc.CreateAttribute(ns.LookupNamespace("android"), "name", "http://schemas.android.com/apk/res/android");
            activityNameAttribute.Value = "com.unity.unitysocial.UriHandlerActivity";
            activity.Attributes.Append(activityNameAttribute);
            XmlAttribute activityNoHistoryAttribute = doc.CreateAttribute(ns.LookupPrefix("android"), "noHistory", "http://schemas.android.com/apk/res/android");
            activityNoHistoryAttribute.Value = "true";
            activity.Attributes.Append(activityNoHistoryAttribute);
            application.AppendChild(activity);

            XmlNode intentFilter = doc.CreateElement("intent-filter");
            activity.AppendChild(intentFilter);

            XmlNode action = doc.CreateElement("action");
            XmlAttribute actionNameAttribute = doc.CreateAttribute(ns.LookupNamespace("android"), "name", "http://schemas.android.com/apk/res/android");
            actionNameAttribute.Value = "android.intent.action.VIEW";
            action.Attributes.Append(actionNameAttribute);
            intentFilter.AppendChild(action);

            XmlNode category0 = doc.CreateElement("category");
            XmlAttribute category0NameAttribute = doc.CreateAttribute(ns.LookupNamespace("android"), "name", "http://schemas.android.com/apk/res/android");
            category0NameAttribute.Value = "android.intent.category.DEFAULT";
            category0.Attributes.Append(category0NameAttribute);
            intentFilter.AppendChild(category0);

            XmlNode category1 = doc.CreateElement("category");
            XmlAttribute category1NameAttribute = doc.CreateAttribute(ns.LookupNamespace("android"), "name", "http://schemas.android.com/apk/res/android");
            category1NameAttribute.Value = "android.intent.category.BROWSABLE";
            category1.Attributes.Append(category1NameAttribute);
            intentFilter.AppendChild(category1);

            XmlNode data = doc.CreateElement("data");
            XmlAttribute dataSchemeAttribute = doc.CreateAttribute(ns.LookupNamespace("android"), "scheme", "http://schemas.android.com/apk/res/android");
            dataSchemeAttribute.Value = "us" + settings.clientId;
            data.Attributes.Append(dataSchemeAttribute);
            intentFilter.AppendChild(data);

            doc.Save(androidManifestXmlPath);
        }

        private static void CreateConfigProjectProperties()
        {
            string configDir = Path.Combine(Application.dataPath, k_ConfigManifestDir);
            string propertiesFilePath = Path.Combine(configDir, "project.properties");

            string[] lines = { "# Generated file - Don't edit", "android.library=true" };
            File.WriteAllLines(propertiesFilePath, lines);
        }
    }
}
