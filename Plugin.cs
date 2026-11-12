using ArchEmperorLib;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public static ConfigEntry<Key> EmitterPreviewKeybind;
    public static ConfigEntry<bool> SeparatePreviewAnimation;
    public static ConfigEntry<string> RetryEncouragementMessages;

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
        RetryEncouragementMessages = config.Bind("Preferences", "Encouragement Messages", string.Join("|", RetryPatch.DefaultEncouragementMessages), "Encouragement messages to log when retrying a match. Leave it empty to disable encouragement messages.");
        SeparatePreviewAnimation = config.Bind("Preferences", "Enable Separation Anim", true, "Enables or disables animation that plays when previewing separator. Enabling it may cause unexpected behaviour when you toggle preview too fast.");
        PluginRechargeKeybind = config.Bind("Keybinds", "Plugin Recharge Keybind", Key.O, "The keybind for recharging plugins.");
        HeatPreviewKeybind = config.Bind("Keybinds", "Heat Preview Keybind", Key.U, "The keybind for toggling heat preview.");
        SeparatePreviewKeybind = config.Bind("Keybinds", "Separate Preview Keybind", Key.F, "The keybind for toggling separate preview.");
        OverdrivePreviewKeybind = config.Bind("Keybinds", "Overdrive Preview Keybind", Key.K, "The keybind for toggling overdrive preview.");
        EmitterPreviewKeybind = config.Bind("Keybinds", "Emitter Preview Keybind", Key.M, "The keybind for viewing emitterload.");
        PhotomodeKeybind = config.Bind("Keybinds", "Photomode Keybind", Key.P, "The keybind for toggling photomode.");
    }
    
}
