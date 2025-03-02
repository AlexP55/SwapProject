using ChronoArkMod.InUnity.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ChronoArkMod.InUnity
{


    public class AssetMisc
    {
        public static ScriptableObject CreateInstanceAt(string assetPath,Type type) 
        {
            var asset = ScriptableObject.CreateInstance(type);
            if (assetPath == null) return asset;
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
            if (File.Exists(assetPath))
            {
                AssetDatabase.AddObjectToAsset(asset, assetPath);
            }
            else
            {
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            return asset;

        }
        public static T CreateInstanceAt<T>(string assetPath) where T:ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            if(assetPath == null)return asset;
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
            if(File.Exists(assetPath))
            {
                AssetDatabase.AddObjectToAsset(asset, assetPath);
            }
            else
            {
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            return asset;
            
        }


        public static void ChangeToLocalAssembly(GameObject gameobject)
        {
            foreach (var com in gameobject.GetComponentsInChildren<MonoBehaviour>(true))
            {
                
                Type t = com.GetType();
                GameObject go2 = new GameObject();
                var com2 = go2.AddComponent(t);

                var script = MonoScript.FromMonoBehaviour(com2 as MonoBehaviour);
                if (script != null)
                {
                    var path = AssetDatabase.GetAssetPath(script);
                    
                    if (path!=null)
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(com);
                        var go = com.gameObject;
                        Object.DestroyImmediate(com, true);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
                    }
                }
                GameObject.DestroyImmediate(go2, true);



            }
        }
        public static void LoadFromMaterial(MaterialLoader materialLoader, Material material)
        {
            Shader shader = material.shader;
            materialLoader.ShaderName = shader.name;


            materialLoader.ColorNames = new List<string>();
            materialLoader.Colors = new List<Color>();
            materialLoader.VectorNames = new List<string>();
            materialLoader.Vectors = new List<Vector4>();
            materialLoader.FloatNames = new List<string>();
            materialLoader.Floats = new List<float>();
            materialLoader.TextureNames = new List<string>();
            materialLoader.Textures = new List<Texture>();
            materialLoader.TextureFileNames = new List<string>();
            materialLoader.TextureOffsets = new List<Vector2>();
            materialLoader.TextureScales = new List<Vector2>();
            int Count = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < Count; i++)
            {
                var PropertyName = ShaderUtil.GetPropertyName(shader, i);
                var PropertyType = ShaderUtil.GetPropertyType(shader, i);
                switch (PropertyType)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        materialLoader.ColorNames.Add(PropertyName);
                        materialLoader.Colors.Add(material.GetColor(PropertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        materialLoader.VectorNames.Add(PropertyName);
                        materialLoader.Vectors.Add(material.GetVector(PropertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                    case ShaderUtil.ShaderPropertyType.Float:
                        materialLoader.FloatNames.Add(PropertyName);
                        materialLoader.Floats.Add(material.GetFloat(PropertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        materialLoader.TextureNames.Add(PropertyName);
                        var tex = material.GetTexture(PropertyName);
                        materialLoader.Textures.Add(null);
                        materialLoader.TextureFileNames.Add(tex?.name);
                        materialLoader.TextureOffsets.Add(material.GetTextureOffset(PropertyName));
                        materialLoader.TextureScales.Add(material.GetTextureScale(PropertyName));
                        break;


                }
            }

        }
    }
}

