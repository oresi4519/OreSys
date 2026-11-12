using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using BepInEx.Logging;

namespace OreSys
{
    public class MachineDesignerPatch
    {
	[HarmonyPostfix]
	[HarmonyPatch(typeof(MachineDesignerMain), "Update")]
    public static void HeatPreviewOnU(MachineDesignerMain __instance)
    {
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current[Plugin.HeatPreviewKeybind.Value].wasPressedThisFrame &&
            !PhotomodePatch.isinDiorama)
        {
            NullCheck();
            machinedesignermain = __instance;
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            if(isindesigner && !inputActive && !uiinputactive){
                if(heatpreviewactive && statusmanager.CurrentHeat != 0f)
                {
                    DisableHeatPreview();
                }
                else
                {
                    EnableHeatPreview();
                }
                machinedesignerui.Param_UpdateMachineStatus();
            }
            else
            {
                if(inputActive)
                {
                    Plugin.Log.LogInfo("Heat preview unavailable: architecture input active");
                }
                else if(!isindesigner)
                {
                    Plugin.Log.LogInfo("Heat preview unavailable: is not in designer");
                }
                else if(uiinputactive)
                {
                    Plugin.Log.LogInfo("Heat preview unavailable: ui input active");
                }                
                else
                {
                    Plugin.Log.LogError("Heat preview unavailable: unknown reason");
                }
            }
        }
    }
    [HarmonyPostfix]
	[HarmonyPatch(typeof(MachineDesignerMain), "Update")]
    public static void SeparatePreviewOnF()
    {
        if(Keyboard.current != null && Keyboard.current[Plugin.SeparatePreviewKeybind.Value].wasPressedThisFrame &&
        !PhotomodePatch.isinDiorama)
        {
            NullCheck();
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;

            if(isindesigner && !inputActive && !uiinputactive)
            {
                if(!separatepreviewactive)
                {
                    Plugin.Log.LogInfo("Enabling separator preview...");
                    EnableSeparatePreview();
                }
                else
                {
                    Plugin.Log.LogInfo("Disabling separator preview...");
                    DisableSeparatePreview();
                }
            } 
            else
            {
                if(inputActive)
                {
                    Plugin.Log.LogInfo("Separator preview unavailable: architecture input active");
                }
                else if(!isindesigner)
                {
                    Plugin.Log.LogInfo("Separator preview unavailable: is not in designer");
                }
                else if(uiinputactive)
                {
                    Plugin.Log.LogInfo("Separator preview unavailable: ui input active");
                }                
                else
                {
                    Plugin.Log.LogError("Separator preview unavailable: unknown reason");
                }
            }
        }
    }
    [HarmonyPostfix]
	[HarmonyPatch(typeof(MachineDesignerMain), "Update")]
    public static void OverdrivePreviewOnK()
    {
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current[Plugin.OverdrivePreviewKeybind.Value].wasPressedThisFrame &&
            !PhotomodePatch.isinDiorama)
        {
            NullCheck();
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;

            if(isindesigner && !inputActive && !uiinputactive)
            {
                if(statusmanager.IsOverdrive == true)
                {
                    Plugin.Log.LogInfo("Disabling overdrive preview...");
                }
                else
                {
                    Plugin.Log.LogInfo("Activating overdrive preview...");
                }
                statusmanager.IsOverdrive = !statusmanager.IsOverdrive;
                machinedesignerui.Param_UpdateMachineStatus();
            }
            else
            {
                if(inputActive)
                {
                    Plugin.Log.LogInfo("Overdrive preview unavailable: architecture input active");
                }
                else if(!isindesigner)
                {
                    Plugin.Log.LogInfo("Overdrive preview unavailable: is not in designer");
                }
                else if(uiinputactive)
                {
                    Plugin.Log.LogInfo("Overdrive preview unavailable: ui input active");
                }                
                else
                {
                    Plugin.Log.LogError("Overdrive preview unavailable: unknown reason");
                }
            }
        }
    }
    /*[HarmonyPostfix]
	[HarmonyPatch(typeof(MachineDesignerMain), "Update")]
    public static void UpdateThrusterInfo()
    {
        UI_ThrusterInfoContainer thrusterInfoContainer = GameObject.Find("Canvas_MachineParam").transform.Find("Thruster").GetComponent<UI_ThrusterInfoContainer>();
        NullCheck();
        if(thrusterInfoContainer != null)
        {
            Plugin.Log.LogInfo($"{thrusterInfoContainer.TI_Forward._Acc_Force.text}");
            Plugin.Log.LogInfo($"{MultiplySpeedString(thrusterInfoContainer.TI_Forward._Acc_Force.text, statusmanager.GetActionSpeed_forThruster())}");
        }
    }
    public static string MultiplySpeedString(string input, float multiplier)
    {
        // 1. Use Regex to find the number (including decimals if present)
        Match match = Regex.Match(input, @"\d+");
        
        if (match.Success)
        {
            // 2. Parse the string number into a numeric type
            if (float.TryParse(match.Value, out float originalNumber))
            {
                // 3. Multiply the number
                float newNumber = originalNumber * multiplier;

                // 4. Replace the old number in the original string with the new one
                // (Using custom formatting to avoid decimals if it's a whole number)
                string newNumberStr = newNumber.ToString("0.##"); 
                return input.Replace(match.Value, newNumberStr);
            }
        }

        // Return original string if no number was found
        return input; 
    }*/
    [HarmonyPostfix]
	[HarmonyPatch(typeof(StatusManager), "Update")]
    public static void RechargePluginsOnO(StatusManager __instance)
    {
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current[Plugin.PluginRechargeKeybind.Value].wasPressedThisFrame &&
            !PhotomodePatch.isinDiorama)
        {
            NullCheck();
            if(__instance != statusmanager)
            {
                return;
            }
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            var isOnline = network ? network.GetComponent<NetworkManager>().IsInSteamLobby : false;
            if(inputActive && !isOnline && isindesigner)
            {
                Plugin.Log.LogInfo("Recharging Plugins...");
                statusmanager.Plugins_Recharging[1] = 1.0f;
                statusmanager.Plugins_Recharging[2] = 1.0f;
            }
            else
            {
                if(isOnline)
                {
                    Plugin.Log.LogInfo("Recharge unavailable: is online");
                }
                else if(!isindesigner)
                {
                    Plugin.Log.LogInfo("Recharge unavailable: is not in designer");
                }
                else if(!inputActive)
                {
                    Plugin.Log.LogInfo("Recharge unavailable: architecture input inactive");
                }       
                else
                {
                    Plugin.Log.LogError("Recharge unavailable: unknown reason");
                }
            }
        }
    }
    [HarmonyPostfix]
	[HarmonyPatch(typeof(MachineDesignerMain), "Update")]
    public static void EmitterloadViewOnM()
    {
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current[Plugin.EmitterPreviewKeybind.Value].wasPressedThisFrame &&
            !PhotomodePatch.isinDiorama)
        {
            NullCheck();
            if(isindesigner && !uiinputactive)
            {
                Plugin.Log.LogInfo("Getting all emitterload...");
                Transform[] allChildren = AC.GetComponentsInChildren<Transform>(true);
                //Plugin.Log.LogInfo("AC =" + $"{AC}");
                //Plugin.Log.LogInfo("allChildren =" + $"{allChildren}");
                for(int i = 0; i < allChildren.Length; i++){
                    Transform child = allChildren[i];
                    //Plugin.Log.LogInfo("child = " + $"{child}");
                    if(child != null && child.name.Contains("Wep_")){
                        UsingController UC = child.GetComponent<UsingController>();
                        if(UC != null){
                            Plugin.Log.LogInfo($"{child.name} : " + $"{UC._GenerationLoad}, " + $"{UC._GenerationLoad / Config.EmitterGenerationReduceCoe} seconds");
                        }
                    }
                }
            }
            else
            {
                if(!isindesigner)
                {
                    Plugin.Log.LogInfo("Emitterload view unavailable: is not in designer");
                }           
                else
                {
                    Plugin.Log.LogError("Emitterload view unavailable: unknown reason");
                }
            }
        }
    }
    private static void NullCheck()
    {
        if(!player)
        {
            player = GameObject.Find("Players").transform.Find("MyPlayer").gameObject;
        }
        if(!statusmanager)
        {
            statusmanager = player ? player.GetComponent<StatusManager>() : null;
        }
        if(!network)
        {
            network = GameObject.Find("GameSystem/Network");
        }
        if(!machinemanager)
        {
            machinemanager = player ? player.GetComponent<MachineManager>() : null;
        }
        if(!machinedesignerui)
        {
            machinedesignerui = GameObject.Find("GameSystem/MachineDesigner").GetComponent<MachineDesignerUI>();
        }
        if(!AC)
        {
            AC = player.transform.Find("ArchitectureComplex");
        }
        return;
    }
    private static void DisableHeatPreview()
    {
        Plugin.Log.LogInfo("Disabling heat preview...");
        machinedesignermain.Paint_PreviewHeating_Stop();         
        statusmanager.CurrentHeat = 0f;                 
        statusmanager.UpdateHeat();                     
        heatpreviewactive = false;       
    }
    private static void EnableHeatPreview()
    {
        Plugin.Log.LogInfo("Activating heat preview...");
        machinedesignermain.Paint_PreviewHeating_Play();
        statusmanager.CurrentHeat = statusmanager.GetMaxHeat();
        statusmanager.UpdateHeat();
        heatpreviewactive = true;
    }
    private static void DisableSeparatePreview()
    {
        var modules = machinemanager.GetSeparatableModules();
        if(modules.Count > 0)
        {
            for (var i = 0; i < modules.Count; i++)
            {
                separatepreviewactive = false;
                var module = modules[i];
                if (module != null)
                {
                    bool isSeparating = module.GetIsSeparating();
                    if(isSeparating)
                    {
                        module.Unseparate(Plugin.SeparatePreviewAnimation.Value);                                    
                    }
                }
            }
            machinemanager.UpdateParamOnSeparation();
            machinedesignerui.Param_UpdateMachineStatus();
        }
    }
    private static void EnableSeparatePreview()
    {
        var modules = machinemanager.GetSeparatableModules();
        if(modules.Count > 0)
        {
            separatepreviewactive = true;
            for (var i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module != null)
                {
                    bool isSeparating = module.GetIsSeparating();
                    if(!isSeparating)
                    {
                        module.Separate();                                    
                    }
                }
            }
            machinemanager.UpdateParamOnSeparation();
            machinedesignerui.Param_UpdateMachineStatus();
        }
    }
    [HarmonyPostfix]
	[HarmonyPatch(typeof(MachineDesignerUI), nameof(MachineDesignerUI.Start))]
	public static void MachineDesignerUI_Start(MachineDesignerUI __instance)
	{
		__instance.Misc_MachineNameInput.onSelect.AddListener((Action<string>)((str) => { uiinputactive = true; })); // designerUI?.modNav.active = false;
		__instance.Misc_MachineNameInput.onDeselect.AddListener((Action<string>)((str) => { uiinputactive = false; }));
		__instance.Misc_MachineDescriptionInput.onSelect.AddListener((Action<string>)((str) => { uiinputactive = true; }));
		__instance.Misc_MachineDescriptionInput.onDeselect.AddListener((Action<string>)((str) => { uiinputactive = false; }));
	}
    [HarmonyPrefix]
	[HarmonyPatch(typeof(Scene_MainMenu), nameof(Scene_MainMenu.Designer_CloseDesigner))]
	public static void OnCloseDesigner()
	{
        NullCheck();
        uiinputactive = false;
        isindesigner = false;
        DisableSeparatePreview();
        if(heatpreviewactive)
        {
            DisableHeatPreview();
        }
	}
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Scene_MainMenu), nameof(Scene_MainMenu.Designer_Design))]
    public static void OnOpenDesigner()
    {
        isindesigner = true;
    }   
    private static GameObject player;
    private static StatusManager statusmanager;
    private static GameObject network;
    private static MachineManager machinemanager;
    private static MachineDesignerUI machinedesignerui;
    private static MachineDesignerMain machinedesignermain;
    private static Transform AC;
    private static bool heatpreviewactive = false;
    private static bool separatepreviewactive = false;
    private static bool uiinputactive = false;
    public static bool isindesigner = false;
    }
}

               