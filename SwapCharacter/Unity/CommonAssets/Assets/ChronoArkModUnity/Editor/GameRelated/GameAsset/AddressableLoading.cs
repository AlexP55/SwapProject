using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using ChronoArkMod.InUnity;

namespace ChronoArkMod.InUnity
{ 
    public class ChronoArkAddressables
    {
        public static string GameAddressablePath => Path.Combine(ChronoArkGameLocation.GameLocation.GameDataPath, "StreamingAssets", "aa");
        [InitializeOnLoadMethod]
        public static void OnLoad()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= UnloadAllAssetBundles;
            AssemblyReloadEvents.beforeAssemblyReload += UnloadAllAssetBundles;
            
             
            InitializeAddressables();
        }

        [MenuItem("ChronoArk/Game Asset/Fix Asset", priority = -100)]
        public static void UnloadAllAssetBundles()
        {
            foreach(var h in ChronoArkAssetLoader.AllHandle)
            {
                Addressables.Release(h);
            }
            AssetBundle.UnloadAllAssetBundles(true);
        }
        public static void InitializeAddressables()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            Addressables.InternalIdTransformFunc = RedirectInternalIdsToGameDirectory;
            var aop = Addressables.InitializeAsync();
            aop.WaitForCompletion();

        }
        static string RedirectInternalIdsToGameDirectory(IResourceLocation location)
        {
            switch (location.ResourceType)
            {
                case var _ when location.InternalId.StartsWith(Addressables.RuntimePath):
                    return location.InternalId.Replace(Addressables.RuntimePath, GameAddressablePath);
                case var _ when location.InternalId.StartsWith(Addressables.BuildPath):
                    return location.InternalId.Replace(Addressables.BuildPath, GameAddressablePath);
                case var f when f == typeof(IAssetBundleResource):
                    {
                        var iid = location.InternalId;
                        var path = iid.Substring(iid.IndexOf("/aa") + 4);
                        path = Path.Combine(GameAddressablePath, path);
                        return path;
                    }
                default:
                    var result = location.InternalId;
                    return result;
            }
        } 
    }
}