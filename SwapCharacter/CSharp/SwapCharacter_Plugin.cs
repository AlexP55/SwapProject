using System;
using ChronoArkMod.Plugin;
using Debug = UnityEngine.Debug;
using HarmonyLib;
using ChronoArkMod;
using ChronoArkMod.ModData.Settings;
using ChronoArkMod.ModData;

namespace SwapCharacter
{
    [PluginConfig(modname, author, version)]
    public class SwapCharacter_Plugin : ChronoArkPlugin
    {
        public const string GUID = "org.Alex.ChronoArk.SwapCharacter";

        public const string modname = "SwapCharacter";

        public const string version = "0.4.1";

        public const string author = "surprise4u";

        private static readonly Harmony harmony = new Harmony(GUID);

        /// <summary>
        /// Option for 5th member summoning
        /// </summary>
        public static bool FifthMember
        {
            get
            {
                return ThisMod.GetSetting<ToggleSetting>("FifthMember").Value;
            }
            set
            {
                var mod = ThisMod;
                mod.GetSetting<ToggleSetting>("FifthMember").Value = value;
                mod.SaveSetting();
            }
        }

        /// <summary>
        /// Option for swap tutorial
        /// </summary>
        public static bool SwapTutorial
        {
            get
            {
                return ThisMod.GetSetting<ToggleSetting>("SwapTutorial").Value;
            }
            set
            {
                var mod = ThisMod;
                mod.GetSetting<ToggleSetting>("SwapTutorial").Value = value;
                mod.SaveSetting();
            }
        }

        /// <summary>
        /// Option for buff tutorial
        /// </summary>
        public static bool BuffTutorial
        {
            get
            {
                return ThisMod.GetSetting<ToggleSetting>("BuffTutorial").Value;
            }
            set
            {
                var mod = ThisMod;
                mod.GetSetting<ToggleSetting>("BuffTutorial").Value = value;
                mod.SaveSetting();
            }
        }

        public static ModInfo ThisMod => ModManager.getModInfo(modname);

        public override void Dispose()
        {
            if (harmony != null)
            {
                Debug.Log("Unloading " + modname + "...");
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            Debug.Log("Initializing " + modname + "...");
            try
            {
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}