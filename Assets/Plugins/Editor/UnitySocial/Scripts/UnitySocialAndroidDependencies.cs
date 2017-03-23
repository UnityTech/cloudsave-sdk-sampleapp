using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnitySocial
{
    [InitializeOnLoad]
    public class UnitySocialAndroidDependencies : AssetPostprocessor
    {
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

        public static void RegisterDependencies()
        {
            Google.VersionHandler.InvokeInstanceMethod(GetSvcSupport(), "ClearDependencies", new object[] {});

            UnitySocialSettings settings = (UnitySocialSettings) Resources.Load("UnitySocialSettings");

            if (settings != null && settings.androidSupportEnabled)
            {
                Debug.Log("Registering UnitySocial dependencies.");

                Google.VersionHandler.InvokeInstanceMethod(
                    GetSvcSupport(), "DependOn",
                    new object[] { "com.google.firebase", "firebase-messaging", "9.8.0" },
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
        }
    }
}
