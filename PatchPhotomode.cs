using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using BepInEx.Logging;

namespace OreSys
{
    public class PhotomodePatch
    {
	[HarmonyPostfix]
	[HarmonyPatch(typeof(InGamePause), "Update")]
    public static void RechargePluginsOnO(InGamePause __instance)
    {
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current[Plugin.PhotomodeKeybind.Value].wasPressedThisFrame)
        {
            NullCheck();
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            var isOnline = network ? network.GetComponent<NetworkManager>().IsInSteamLobby : false;
            if(inputActive)
            {
                if(!isinPhotomode)
                {
                    Plugin.Log.LogInfo("Entering Photomode...");
                    /*if(!isOnline)
                    {
                        UnityEngine.Time.timeScale = 0f;
                    }*/
                    ingamepause.Pause();
                    photomodeenterbutton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                    isinPhotomode = true;
                }
                else
                {
                    Plugin.Log.LogInfo("Exiting Photomode...");
                    photomodeexitbutton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                    isinPhotomode = false;
                    resumebutton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                }
            }
            else
            {
                if(!inputActive)
                {
                    Plugin.Log.LogInfo("Photomode unavailable: architecture input inactive");
                }              
                else
                {
                    Plugin.Log.LogError("Photomode unavailable: unknown reason");
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
        if(!network)
        {
            network = GameObject.Find("GameSystem/Network");
        }
        if(!resumebutton)
        {
            pausemenu = GameObject.Find("Canvas_System").transform.Find("Pause").gameObject;
            resumebutton = pausemenu.transform.Find("Container/inner/PauseButton").gameObject;
            photomodeenterbutton = pausemenu.transform.Find("Container/inner/PauseButton (2)").gameObject;
        }
        if(!photomodeexitbutton)
        {
            photomodeexitbutton = GameObject.Find("PhotoMode_Canvas").transform.Find("main/ExitPhotoMode").gameObject;
        }
        if(!ingamepause)
        {
            ingamepause = GameObject.Find("GameSystem/Match").GetComponent<InGamePause>();
        }
        return;
    }
    private static GameObject player;
    private static GameObject network;
    private static GameObject pausemenu;
    private static GameObject resumebutton;
    private static GameObject photomodeenterbutton;
    private static GameObject photomodeexitbutton;
    private static InGamePause ingamepause;
    private static bool isinPhotomode = false;
    [HarmonyPostfix]
	[HarmonyPatch(typeof(PhotoSettingManager), "DisablePhotoMode")]
    public static void ToggleVarOnExit(PhotoSettingManager __instance)
    {
        NullCheck();
        if(ingamepause.timeScaleTemp == 0f)
        {
            ingamepause.timeScaleTemp = 1f;
        }
        isinPhotomode = false;
    }
    }   
}

