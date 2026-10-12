using System;
using ArchEmperorLib;
using HarmonyLib;
using UnityEngine;

namespace OreSys
{
	// Token: 0x02000017 RID: 23
	public class RegisterArchCredit
	{
	static bool once = false;
	[HarmonyPostfix]
	[HarmonyPatch(typeof(Scene_MainMenu), nameof(Scene_MainMenu.Start))]
	public static void InitializeMain(ref Scene_MainMenu __instance)
	{
		if (once) return; once = true;
		Cursor.lockState = CursorLockMode.Confined;

		//Localization.RegisterTranslation();
		//ModSettings.InitModConfig();
		SettingBlock settingBlock = new SettingBlock();
		settingBlock.AddSetting("test", "object");
		ModManager.SetCredits(MyPluginInfo.PLUGIN_GUID, MakeCredits());
		ModManager.SetSettings(MyPluginInfo.PLUGIN_GUID, settingBlock);

		//ModManager.OnModSettignsRefresh += guid => { if (guid == MyPluginInfo.PLUGIN_GUID) ModSettings.RefreshEntries(); };
		//ModManager.OnModSettignsCancel += guid => { if (guid == MyPluginInfo.PLUGIN_GUID) ModSettings.RefreshEntries(); };
		//ModManager.OnModSettignsApply += guid => { if (guid == MyPluginInfo.PLUGIN_GUID) ModSettings.OnSettingsApply(); };
	}

		private static CreditBlock MakeCredits()
		{
			CreditBlock creditBlock = new CreditBlock();
			if (Plugin.assets)
			{
				creditBlock.AddLogo(Plugin.assets.LoadAsset<Sprite>("OreSysLogo"));
			}
			//creditBlock.AddText(CreditBlock.FONT_SIZE.TITLE, "ORESYS");
			creditBlock.AddSeparator();
			creditBlock.AddText(CreditBlock.FONT_SIZE.ROLE, "DEVELOPER");
			creditBlock.AddText(CreditBlock.FONT_SIZE.NORMAL, "oresi1034");
			creditBlock.AddSeparator();
			creditBlock.AddText(CreditBlock.FONT_SIZE.ROLE, "SPECIAL THANKS");
			creditBlock.AddText(CreditBlock.FONT_SIZE.NORMAL, "Merilax - For developing the ArchEmperorLib and making this mod possible.");
			creditBlock.AddText(CreditBlock.FONT_SIZE.NORMAL, "CloudCluster - For taking care of the BreakArts Builders Discord.");
			creditBlock.AddText(CreditBlock.FONT_SIZE.NORMAL, "The Break Arts community - For making this possible.");
			return creditBlock;
		}

	}
}
