using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using BepInEx.Logging;

namespace OreSys
{
    public class RetryPatch
    {
	[HarmonyPostfix]
	[HarmonyPatch(typeof(MatchManager), "Update")]
    public static void RetryOnT(MatchManager __instance)
    {
        NullCheck();
        
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current[Plugin.RetryKeybind.Value].wasPressedThisFrame)
        {
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            var isOnline = network ? network.GetComponent<NetworkManager>().IsInSteamLobby : false;
            var isPaused = pausemenu ? pausemenu.activeSelf : false;
            var isinMatch = statusmanager ? statusmanager.GetRank_Match() != -1 : false;
            if(inputActive && !isOnline && isinMatch)
            {
                string message = GetRandomEncouragementMessage();
                Plugin.Log.LogInfo((message) + " Retrying Match...");
                resumebutton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                __instance.RetryMatch();
            }
            else 
            {
                if(isOnline)
                {
                    Plugin.Log.LogInfo("Chill out, Artist. (Retry unavailable: is online)");
                }
                else if(!inputActive)
                {
                    Plugin.Log.LogInfo("Chill out, Artist. (Retry unavailable: architecture input inactive)");
                }
                else if(!isinMatch)
                {
                    Plugin.Log.LogInfo("Chill out, Artist. (Retry unavailable: is not in match)");
                }                
                else
                {
                    Plugin.Log.LogError("Chill out, Artist. (Retry unavailable: unknown reason)");
                }
            }
        }

    }
    private static void NullCheck()
    {
        if(!network)
        {
            network = GameObject.Find("GameSystem/Network");
        }
        if(!player)
        {
            player = GameObject.Find("Players").transform.Find("MyPlayer").gameObject;
        }
        if(!statusmanager)
        {
            statusmanager = player ? player.GetComponent<StatusManager>() : null;
        }
        if(!resumebutton)
        {
            pausemenu = GameObject.Find("Canvas_System").transform.Find("Pause").gameObject;
            resumebutton = pausemenu.transform.Find("Container/inner/PauseButton").gameObject;
        }
        return;
    }
    private static GameObject player;
    private static StatusManager statusmanager;
    private static GameObject network;
    private static GameObject pausemenu;
    private static GameObject resumebutton;
    private static readonly string[] encouragementMessages = new string[]
        {
            "Every retry is a step closer to the ideal.",
            "Believe in yourself, Artist.",
            "Each mistake carves your ideal course.",
            "One more time.",
            "Remember to breathe, Artist.",
            "Remember to take breaks, Artist."
        };
    private static readonly System.Random random = new System.Random();
    public static string GetRandomEncouragementMessage()
        {
            int index = random.Next(encouragementMessages.Length);
            return encouragementMessages[index];
        }
    }


}

