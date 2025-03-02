using ChronoArkMod.InUnity.Dialogue;
using Dialogical;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine.Unity;
namespace ChronoArkMod.InUnity
{
    public class GameObjectLoaderWindow : EditorWindow
    {
        public static GameObjectLoaderWindow _Instance;
        [MenuItem("ChronoArk/Game Asset/GameObject Loader Window", false, -102)]
        public static void OpenEditorWindow()
        {
            _ = Instance;
        }
        public static GameObjectLoaderWindow Instance
        {
            get
            {
                if( _Instance == null)
                {
                    _Instance = GetWindow<GameObjectLoaderWindow>("GameObject Loader", true);
                    _Instance.Init();
                }
                _Instance.Show();
                _Instance.Focus();
                return _Instance;
            }

        }
        public static Vector2 MousePosition;
        public void Init()
        {
            Instance.minSize = new Vector2(200, 0);
        }
        public string AssetPath;

        public void MemorizeInfomation(GameObject gameObject)
        {

            if(UseSprite)
            {
                MemorizeSprite(gameObject);
            }
            if (UseParticleSystemMesh)
            {
                MemorizeParticleSystemMesh(gameObject);
            }
            if(UseBattleMapSkyBox)
            {
                MemorizeBattleMapSkyBox(gameObject);
            }
            if(UseSkeletonDataAsset)
            {
                MemorizeSkeletonDataAsset(gameObject);
            }
            if (UseMesh)
            {
                MemorizeMesh(gameObject);
            }
            if (UseMaterial)
            {
                MemorizeMaterial(gameObject);
            }
        }


        public bool UseMaterial = true;
        public void MemorizeMaterial(GameObject gameObject)
        {
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                if (r.enabled == false) continue;
                if (r.sharedMaterial == null) continue;
                var loader = r.gameObject.AddComponent<MaterialLoader>();
                AssetMisc.LoadFromMaterial(loader, r.sharedMaterial);
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };


            }
        }


        public bool UseSprite = true;
        public void MemorizeSprite(GameObject gameObject)
        {
            foreach (var r in gameObject.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (r.enabled == false) continue;
                if (r.sprite == null) continue;
                var loader = r.gameObject.AddComponent<SpriteLoader>();
                loader.SpriteFileName = r.sprite.name;
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };


            }
            foreach (var r in gameObject.GetComponentsInChildren<Image>(true))
            {
                if (r.enabled == false) continue;
                if (r.sprite == null) continue;
                var loader = r.gameObject.AddComponent<SpriteLoader>();
                loader.SpriteFileName = r.sprite.name;
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };


            }
        }


        public bool UseParticleSystemMesh = false;
        public void MemorizeParticleSystemMesh(GameObject gameObject)
        {
            foreach (var r in gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                if (r.enabled == false) continue;
                if (r.mesh == null) continue;
                var loader = r.gameObject.AddComponent<ParticleMeshLoader>();
                loader.RendererMesh = null;
                loader.RendererMeshFileName = r.mesh.name;
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };



            }
            foreach (var p in gameObject.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (p.shape.mesh == null) continue;
                var loader = p.gameObject.GetComponent<ParticleMeshLoader>();
                if (loader == null) loader = p.gameObject.AddComponent<ParticleMeshLoader>();
                loader.ShapeMesh = null;
                loader.ShapeMesh = p.shape.mesh;
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };



            }
        }


        public bool UseBattleMapSkyBox = false;
        public void MemorizeBattleMapSkyBox(GameObject gameObject)
        {
            foreach (var r in gameObject.GetComponentsInChildren<BattleMap>(true))
            {
                if (r.enabled == false) continue;
                if (r.SkyBox == null) continue;
                var loader = r.gameObject.AddComponent<BattleMapSkyBoxLoader>();
                AssetMisc.LoadFromMaterial(loader, r.SkyBox);
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };


            }
        }

        public bool UseSkeletonDataAsset = false;
        public void MemorizeSkeletonDataAsset(GameObject gameObject)
        {
            foreach (var r in gameObject.GetComponentsInChildren<SkeletonAnimation>(true))
            {
                
                var loader = r.gameObject.AddComponent<SkeletonDataAssetLoader>();
                loader.dataname = r.skeletonDataAsset?.name;
                loader.dataAsset = null;
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };



            }
        }
        public bool UseMesh = false;
        public void MemorizeMesh(GameObject gameObject)
        {
            foreach (var r in gameObject.GetComponentsInChildren<MeshFilter>(true))
            {

                var loader = r.gameObject.AddComponent<MeshLoader>();
                loader.MeshFileName = r.mesh?.name;
                loader.RendererMesh = null;
                loader.RelatedChronoArkGameObjects = new List<string>() { AssetPath };



            }
        }


        public void OnGUI()
        {
            EditorGUILayout.LabelField("GameObject Path");
            AssetPath = EditorGUILayout.TextField(AssetPath);

            UseMaterial = EditorGUILayout.Toggle("Use Material", UseMaterial);
            UseSprite = EditorGUILayout.Toggle("Use Sprite", UseSprite);
            UseParticleSystemMesh = EditorGUILayout.Toggle("Use Particle System Mesh", UseParticleSystemMesh);
            UseBattleMapSkyBox = EditorGUILayout.Toggle("Use Battle Map Sky Box", UseBattleMapSkyBox);
            UseSkeletonDataAsset = EditorGUILayout.Toggle("Use Skeleton Data Asset", UseSkeletonDataAsset);
            UseMesh = EditorGUILayout.Toggle("Use Mesh (MeshFilter)", UseMesh);
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("Load"))
            {
                GameObject gameObject =  Load(AssetPath);
                MemorizeInfomation(gameObject);
            }
            if (Event.current.type == EventType.Repaint)
            {

                Rect lastRect = GUILayoutUtility.GetLastRect();
                minSize = new Vector2(minSize.x, lastRect.y + lastRect.height + 4);
            }
        }
        public GameObject Load(string path)
        {
            var loading = Addressables.LoadAssetAsync<GameObject>(path);
            loading.WaitForCompletion();
            
            ChronoArkAssetLoader.AllHandle.Add(loading);
            var asset = loading.Result;
            if (asset == null)
            {
                new GameObject().AddComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>(path).WaitForCompletion();
            }
            var gameObject = GameObject.Instantiate(asset);
            AssetMisc.ChangeToLocalAssembly(gameObject);
            return gameObject;
            
        }
    }
}

