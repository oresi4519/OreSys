using System;
using System.IO;
using System.Text;
using System.Text.Json;
using ArchEmperorLib;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UniverseLib;

namespace OreSys;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("BREAK ARTS III.exe")]
[BepInDependency("ArchEmperorLib", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BasePlugin
{
	internal static new ManualLogSource Log;
	private static readonly bool verboseLogging = false;
	public static ConfigFile config;
	//private static JsonSerializerOptions serializerOptions;
	//public static ConfigData customConfig = new();
	public static UniverseLib.AssetBundle assets;
    public static ConfigEntry<Key> RetryKeybind;
    public static ConfigEntry<Key> PluginRechargeKeybind;
    public static ConfigEntry<Key> HeatPreviewKeybind;
    public static ConfigEntry<Key> SeparatePreviewKeybind;
    public static ConfigEntry<Key> OverdrivePreviewKeybind;
    public static ConfigEntry<Key> PhotomodeKeybind;

    public override void Load()
    { 
        // Plugin startup logic
        Log = base.Log;
		string bundlePath = Path.Combine(Paths.PluginPath, "OreSys", "oresys_assets");
		if (!File.Exists(bundlePath))
			throw new Exception($"AssetBundle could not be located found at: {bundlePath}. Aborting mod initializaton.");

		assets = UniverseLib.AssetBundle.LoadFromFile(bundlePath) ?? throw new Exception("Failed to load assets! Please report this bug to the developer, along with any game logs. Aborting mod initializaton.");
		Log.LogInfo("Loading keybinds...");
        config = Config;
		LoadKeybinds();
        Log.LogInfo("Loading retry patch...");
        Harmony.CreateAndPatchAll(typeof(RetryPatch));
        Log.LogInfo("Loading testplay patch...");
        Harmony.CreateAndPatchAll(typeof(TestplayPatch));
        Log.LogInfo("Loading machine designer patch...");
        Harmony.CreateAndPatchAll(typeof(MachineDesignerPatch));
        Log.LogInfo("Loading photomode patch...");
        Harmony.CreateAndPatchAll(typeof(PhotomodePatch));
        Log.LogInfo("Registering to ArchEmperorLib...");
        ModRegistry.Register(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, RequirementScope.ClientOptional, VersionStrictness.None, null);
        Harmony.CreateAndPatchAll(typeof(RegisterArchCredit));
        Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

	public static void LogInfo(object data) => Log.LogInfo(data);
	public static void LogDebug(object data)
	{
		if (verboseLogging) LogInfo(data);
	}


    public static void LoadKeybinds()
    {
        RetryKeybind = config.Bind("Keybinds", "Retry Keybind", Key.T, "The keybind for retrying a match.");
        PluginRechargeKeybind = config.Bind("Keybinds", "Plugin Recharge Keybind", Key.O, "The keybind for recharging plugins.");
        HeatPreviewKeybind = config.Bind("Keybinds", "Heat Preview Keybind", Key.U, "The keybind for toggling heat preview.");
        SeparatePreviewKeybind = config.Bind("Keybinds", "Separate Preview Keybind", Key.F, "The keybind for toggling separate preview.");
        OverdrivePreviewKeybind = config.Bind("Keybinds", "Overdrive Preview Keybind", Key.K, "The keybind for toggling overdrive preview.");
        PhotomodeKeybind = config.Bind("Keybinds", "Photomode Keybind", Key.P, "The keybind for toggling photomode.");
    }

}
