using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using BepInEx.Logging;

namespace OreSys
{
    public class CLASSNAME
    {
	[HarmonyPostfix]
	[HarmonyPatch(typeof(TYPE), "Update")]
    public static void METHODNAME(TYPE __instance)
    {
        if(Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.oKey.wasPressedThisFrame)
        {
            NullCheck();
            if(__instance != statusmanager)
            {
                return;
            }
            var inputActive = player ? player.GetComponent<LocalHumanInput>().IsEnable : false;
            var isOnline = network ? network.GetComponent<NetworkManager>().IsInSteamLobby : false;
            var isinMatch = statusmanager ? statusmanager.GetRank_Match() != -1 : false;
            if(inputActive && !isOnline && !isinMatch)
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
                else if(!inputActive)
                {
                    Plugin.Log.LogInfo("Recharge unavailable: architecture input inactive");
                }
                else if(isinMatch)
                {
                    Plugin.Log.LogInfo("Recharge unavailable: is in match");
                }                
                else
                {
                    Plugin.Log.LogError("Recharge unavailable: unknown reason");
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
        return;
    }
    private static GameObject player;
    private static StatusManager statusmanager;
    private static GameObject network; 
    }   
}

