using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ChronoArkMod.InUnity
{

    
    public class LocalizationChronoArk
    {
        public static bool Inited;
        [InitializeOnLoadMethod]
        public static void LoadSystemLocalization()
        {
            if (!Inited&&!string.IsNullOrEmpty(ChronoArkGameLocation.GameLocation.GameDataPath))
            {
                Inited = true;
                var Source = new LanguageSourceData();
                string path = Path.Combine(ChronoArkGameLocation.GameLocation.GameDataPath, "StreamingAssets", "LangSystemDB.csv");
                string csvstring = LocalizationReader.ReadCSVfile(path, Encoding.UTF8);
                Source.Import_CSV(string.Empty, csvstring, eSpreadsheetUpdateMode.Replace, ',');
                LocalizationManager.InitializeIfNeeded();
                LocalizationManager.Sources.Add(Source);
                for (int i = 0; i < Source.mLanguages.Count<LanguageData>(); i++)
                {
                    Source.mLanguages[i].SetLoaded(true);
                }
                if (Source.mDictionary.Count == 0)
                {
                    Source.UpdateDictionary(true);
                }
                Source.UpdateAssetDictionary();
                LocalizationManager.LocalizeAll(true);
            }
        }

        
    }

    
}