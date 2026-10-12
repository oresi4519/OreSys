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
            UnityEngine.InputSystem.Keyboard.current[Plugin.HeatPreviewKeybind.Value].wasPressedThisFrame)
        {
            NullCheck();
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            statusmanager.UpdateCurrentRanks();
            var isinMatch = statusmanager ? statusmanager.GetRank_Match() != -1 : false;
            if(!isinMatch && !inputActive && !uiinputactive){
                if(heatpreviewactive && statusmanager.CurrentHeat != 0f)
                {
                    Plugin.Log.LogInfo("Disabling heat preview...");
                    __instance.Paint_PreviewHeating_Stop();
                    statusmanager.CurrentHeat = 0f;
                    statusmanager.UpdateHeat();
                    heatpreviewactive = false;
                }
                else
                {
                    Plugin.Log.LogInfo("Activating heat preview...");
                    __instance.Paint_PreviewHeating_Play();
                    statusmanager.CurrentHeat = statusmanager.GetMaxHeat();
                    statusmanager.UpdateHeat();
                    heatpreviewactive = true;
                }
                machinedesignerui.Param_UpdateMachineStatus();
            }
            else
            {
                if(inputActive)
                {
                    Plugin.Log.LogInfo("Heat preview unavailable: architecture input active");
                }
                else if(isinMatch)
                {
                    Plugin.Log.LogInfo("Heat preview unavailable: is in match");
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
        if(Keyboard.current != null && Keyboard.current[Plugin.SeparatePreviewKeybind.Value].wasPressedThisFrame)
        {
            NullCheck();
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            statusmanager.UpdateCurrentRanks();
            var isinMatch = statusmanager ? statusmanager.GetRank_Match() != -1 : false;

            if(!isinMatch && !inputActive && !uiinputactive)
            {
                var modules = machinemanager.GetSeparatableModules();
                for (var i = 0; i < modules.Count; i++)
                {
                    var module = modules[i];
                    if (module != null)
                    {
                        bool isSeparating = module.GetIsSeparating();
                        if(isSeparating)
                        {
                            Plugin.Log.LogInfo("Disabling separator preview...");
                            module.Unseparate(true);
                        }
                        else
                        {
                            Plugin.Log.LogInfo("Activating separator preview...");
                            module.Separate();
                        }
                    }
                }
                machinemanager.UpdateParamOnSeparation();
                machinedesignerui.Param_UpdateMachineStatus();
            } 
            else
            {
                if(inputActive)
                {
                    Plugin.Log.LogInfo("Separator preview unavailable: architecture input active");
                }
                else if(isinMatch)
                {
                    Plugin.Log.LogInfo("Separator preview unavailable: is in match");
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
            UnityEngine.InputSystem.Keyboard.current[Plugin.OverdrivePreviewKeybind.Value].wasPressedThisFrame)
        {
            NullCheck();
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            statusmanager.UpdateCurrentRanks();
            var isinMatch = statusmanager ? statusmanager.GetRank_Match() != -1 : false;

            if(!isinMatch && !inputActive && !uiinputactive)
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
                else if(isinMatch)
                {
                    Plugin.Log.LogInfo("Overdrive preview unavailable: is in match");
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
        if(machinemanager == null)
        {
            machinemanager = player ? player.GetComponent<MachineManager>() : null;
        }
        if(!machinedesignerui)
        {
            machinedesignerui = GameObject.Find("GameSystem/MachineDesigner").GetComponent<MachineDesignerUI>();
        }
        return;
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
        uiinputactive = false;
	}   
    private static GameObject player;
    private static StatusManager statusmanager;
    private static MachineManager machinemanager;
    private static MachineDesignerUI machinedesignerui;
    private static bool heatpreviewactive = false;
    private static bool uiinputactive = false;
    }
}

