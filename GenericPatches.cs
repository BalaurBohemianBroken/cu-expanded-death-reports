using System.Collections;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BalaurBohemianBroken {

	// This patch is special because it targets an IEnumerator.
	// There's a rough guide on how to patch those located here:
	// https://github.com/BepInEx/HarmonyX/wiki/Enumerator-patches
	// [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.EndSequence), MethodType.Enumerator)]
	// public class EndScreen {
	// 	// Making this stuff static is really bad form for IEnumerators usually.
	// 	// Often there will be multiple of them, and they'll conflict!
	// 	// But I don't think that's the case with this one.
	// 	// Assigned when IEnum is created.
	// 	public static PlayerCamera pc;
	// 	public static int argument_type;
	// 	public static int enum_state = 0;
	// 	public static float timer2 = 0;
	//
	// 	public static bool Prefix(IEnumerator __instance) {
	// 		BepInEx.Logging.Logger.CreateLogSource("ExpandedDeathReports.Patches").LogInfo("IEnumerator MoveNext patch");
	// 		// return true;
	// 		pc.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false, force: true);
	// 		if (argument_type == 0)
	// 		{
	// 			PlayerPrefs.SetInt("deathcount", PlayerPrefs.GetInt("deathcount") + 1);
	// 			pc.endScreen.gameObject.SetActive(value: true);
	// 			if (pc.body.inWater)
	// 			{
	// 				pc.endScreen.sprite = pc.deathScreenSprites[2];
	// 			}
	// 			else if (pc.body.totalBleedSpeed > 0.02f)
	// 			{
	// 				pc.endScreen.sprite = pc.deathScreenSprites[1];
	// 			}
	// 			else
	// 			{
	// 				pc.endScreen.sprite = pc.deathScreenSprites[0];
	// 			}
	// 			int num = Mathf.RoundToInt(pc.body.AverageHappiness() * 0.1f);
	// 			pc.deathStats.GetChild(0).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenstatus");
	// 			pc.deathStats.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2XXX-" + DateTime.Now.ToString("MM-dd-HH-mm-ss");
	// 			pc.deathStats.GetChild(2).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenunresolved");
	// 			pc.deathStats.GetChild(3).GetComponent<TextMeshProUGUI>().text = "#" + WoundView.specimenId + "-SAW-01";
	// 			pc.deathStats.GetChild(4).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenphysical");
	// 			pc.deathStats.GetChild(5).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreendeceased");
	// 			pc.deathStats.GetChild(6).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenmental");
	// 			pc.deathStats.GetChild(7).GetComponent<TextMeshProUGUI>().text = (Locale.GetOther("moodrange" + (pc.body.mindWipe ? "wiped" : ((object)num))) + (pc.body.succesfullyRolledLastStand ? Locale.GetOther("moodrangeyethopeful") : "")).ToUpper();
	// 			pc.deathStats.GetChild(8).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreendepth");
	// 			pc.deathStats.GetChild(9).GetComponent<TextMeshProUGUI>().text = (int)WorldGeneration.world.PlayerTotalDepthMeters() + Locale.GetOther("endscreendepthres") + TimeSpan.FromSeconds(WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss");
	// 			pc.deathStats.GetChild(10).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreencalories");
	// 			pc.deathStats.GetChild(11).GetComponent<TextMeshProUGUI>().text = $"{pc.caloriesConsumed}cal";
	// 			pc.deathStats.GetChild(12).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreencasualties");
	// 			pc.deathStats.GetChild(13).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenunknown") + " + " + PlayerPrefs.GetInt("deathcount");
	// 			
	// 			__instance.GetType().GetField("current", BindingFlags.Instance).SetValue(__instance, new WaitForSecondsRealtime(3.5f));
	// 			Vector2 origPos2 = new Vector2(-336f, -1000f);
	// 			Vector2 newPos2 = new Vector2(-336f, -934f);
	// 			Sound.Play("receiptPrint", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.5f, 1f, noReverb: true, ignoreMixer: true);
	// 			float timer2 = 1f;
	// 			while (timer2 > 0f)
	// 			{
	// 				timer2 -= Time.unscaledDeltaTime * 1.5f;
	// 				pc.deathStats.anchoredPosition = Vector2.LerpUnclamped(origPos2, newPos2, 1f - timer2);
	// 				//yield return null;
	// 			}
	// 			pc.deathStats.anchoredPosition = newPos2;
	// 			bool clicked = false;
	// 			pc.deathStats.GetComponent<Button>().onClick.AddListener(delegate
	// 			{
	// 				clicked = true;
	// 			});
	// 			while (!clicked)
	// 			{
	// 				//yield return null;
	// 			}
	// 			Sound.Play("receiptRip", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.5f, 1f, noReverb: true, ignoreMixer: true);
	// 			timer2 = 1f;
	// 			origPos2 = new Vector2(-336f, -900f);
	// 			newPos2 = new Vector2(-400f, 0f);
	// 			while (timer2 > 0f)
	// 			{
	// 				timer2 -= Time.unscaledDeltaTime * 1.8f;
	// 				pc.deathStats.anchoredPosition = Vector2.LerpUnclamped(origPos2, newPos2, Utils.OutQuart(1f - timer2));
	// 				//yield return null;
	// 			}
	// 			pc.deathStats.anchoredPosition = newPos2;
	// 		}
	// 		pc.endScreen.gameObject.SetActive(value: true);
	// 		//yield return null;
	// 	}
	// }
	
    [HarmonyPatch]
	public class EndScreenMovenext {
		// Originally tried to get distance between the first two lines using this, but it was wrong. So I manually found a value.
		// __instance.deathStats.GetChild(4).position.y - y_start;
		static float line_height = -36;
		private static float font_mult = 0.6f;
		private static string variable_color = "#5f1717";
		
		// How IEnumerator is working here is a total mystery to me.
		// I never assign to __result yet it does overwrite it
		// Anyway, this effectively skips the existing method.
		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.EndSequence))]
		public static IEnumerator BalaurEndSequence(IEnumerator __result, PlayerCamera __instance, int type) {
			BepInEx.Logging.Logger.CreateLogSource("ExpandedDeathReports.Patches").LogInfo("Running custom end screen");
		    if (type == 0) {
			    var stats = ExpandedDeathReports.StatTrackersBoring;
				PlayerPrefs.SetInt("deathcount", PlayerPrefs.GetInt("deathcount") + 1);
				__instance.endScreen.gameObject.SetActive(true);
				// TODO: Add stats that the game tracks to my stats: Time, depth, death count, calories, mental state. Death background?
				SetEndBackground(__instance);
				
				int mental_state_integer = Mathf.RoundToInt(__instance.body.AverageHappiness() * 0.1f);
				// Headers
				var tm = __instance.deathStats.GetChild(0).GetComponent<TextMeshProUGUI>();
				tm.richText = true;
				tm.text = Locale.GetOther("endscreenstatus") + $"#{ColorVar(PlayerPrefs.GetInt("deathcount"))}";
				__instance.deathStats.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2XXX-" + DateTime.Now.ToString("MM-dd-HH-mm-ss");
				__instance.deathStats.GetChild(2).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenunresolved");

				Transform first_line = __instance.deathStats.GetChild(3);
				float y_start = first_line.position.y;
				List<TextMeshProUGUI> lines = new List<TextMeshProUGUI>();
				lines.Add(first_line.GetComponent<TextMeshProUGUI>());
				
				// Clear existing text objects and create my own.
				for (int i = 4; i < 14; i++) {
					UnityEngine.Object.Destroy(__instance.deathStats.GetChild(i).gameObject);
				}
				
				for (int i = 4; i < 23; i++) {
					Transform obj = UnityEngine.Object.Instantiate(first_line, first_line.parent);
					Vector3 pos = obj.position;
					pos.y += line_height * (i - 3);
					obj.position = pos;
					var tmp = obj.GetComponent<TextMeshProUGUI>();
					tmp.text = "";
					lines.Add(tmp);
				}

				foreach (var line in lines) {
					line.fontSize *= font_mult;
					line.richText = true;
				}

				lines[0].text = $"#{WoundView.specimenId.ToString()}-SAW-01";  // TODO: Creature name.

				// TODO: This is awful for translation. If it gets interest, move this stuff over to translation files.
				string mood = "moodrange";
				if ((bool)(UnityEngine.Object)__instance.body.mindWipe) 
					mood += "wiped";
				else
					mood += mental_state_integer;
				if (__instance.body.succesfullyRolledLastStand)
					mood += Locale.GetOther("moodrangeyethopeful");
				mood = Locale.GetOther(mood).ToUpper();
				lines[1].text = "MENTALS: " + ColorVar(mood);

				int depth = (int)WorldGeneration.world.PlayerTotalDepthMeters();
				int layer = 0;  // TODO: implement.
				string time = TimeSpan.FromSeconds(WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss");
				lines[2].text = $"RESULTS: {ColorVar(depth)}M, LAYER {ColorVar(layer)}, IN {ColorVar(time)}";
				// lines[3]
				lines[4].text = $"CONSUMPTION: {ColorVar(__instance.caloriesConsumed)} KCAL, {GetStat(stats, "FluidsConsumed")} ML";
				lines[5].text = $"QUALITIES: {ColorVar(8)} INT, {ColorVar(10)} STR, {ColorVar(11)} RES";  // TODO: This.
				// lines[6]
				lines[7].text = $"PAIN AVERAGE: {GetStat(stats, "PainSufferedAverage")}%";
				lines[8].text = $"FRACTURES: {GetStat(stats, "BonesFractured")}";
				lines[9].text = $"CUTS/SHRAPNEL/INFECTIONS: {ColorVar(32)}/{ColorVar(10)}/{ColorVar(5)}";  // TODO: This.
				lines[10].text = $"DAMAGE RECEIVED/RECOVERED: {ColorVar(523)}/{ColorVar(402)}";  // TODO: This.
				// lines[11]
				lines[12].SetText($"<align=\"center\"><b>FURTHER NOTES</b>");  // TODO: Center justification.
				// __instance.deathStats.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"#{WoundView.specimenId.ToString()}-SAW-01";
				// __instance.deathStats.GetChild(4).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenphysical");
				// __instance.deathStats.GetChild(5).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreendeceased");
				// __instance.deathStats.GetChild(6).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenmental");
				// __instance.deathStats.GetChild(7).GetComponent<TextMeshProUGUI>().text = (Locale.GetOther("moodrange" + ((bool) (UnityEngine.Object) __instance.body.mindWipe ? (object) "wiped" : (object) mental_state_integer)?.ToString()) + (__instance.body.succesfullyRolledLastStand ? Locale.GetOther("moodrangeyethopeful") : "")).ToUpper();
				// __instance.deathStats.GetChild(8).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreendepth");
				// TextMeshProUGUI component1 = __instance.deathStats.GetChild(9).GetComponent<TextMeshProUGUI>();
				// int depth = (int) WorldGeneration.world.PlayerTotalDepthMeters();
				// string str1 = depth + Locale.GetOther("endscreendepthres") + TimeSpan.FromSeconds((double) WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss");
				// component1.text = str1;
				// __instance.deathStats.GetChild(10).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreencalories");
				// __instance.deathStats.GetChild(11).GetComponent<TextMeshProUGUI>().text = $"{__instance.caloriesConsumed} KCAL, {stats["FluidsConsumed"].GetValue()} ML";
				// __instance.deathStats.GetChild(12).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreencasualties");
				// TextMeshProUGUI cas_unknown = __instance.deathStats.GetChild(13).GetComponent<TextMeshProUGUI>();
				// cas_unknown.text = $"{Locale.GetOther("endscreenunknown")} + {PlayerPrefs.GetInt("deathcount")}";
				// TODO: Neither of these are working. It's minor, and most people probably won't notice. But I should figure it out.
				// __result.GetType().GetField("current").SetValue(__result, new WaitForSecondsRealtime(3.5f));
				// yield return new WaitForSecondsRealtime(3.5f);
				float timer = 3.5f;
				while (timer > 0.0f) {
				  timer -= Time.unscaledDeltaTime * 1.5f;
				  yield return null;
				}
				Vector2 origPos = new Vector2(-336f, -1000f);
				Vector2 newPos = new Vector2(-336f, -934f);
				Sound.Play("receiptPrint", Vector2.zero, true, false, volume: 0.5f, noReverb: true, ignoreMixer: true);
				timer = 1f;
				while (timer > 0.0f) {
					timer -= Time.unscaledDeltaTime * 1.5f;
					__instance.deathStats.anchoredPosition = Vector2.LerpUnclamped(origPos, newPos, 1f - timer);
					yield return null;
				}
				__instance.deathStats.anchoredPosition = newPos;
				bool clicked = false;
				__instance.deathStats.GetComponent<Button>().onClick.AddListener((UnityAction) (() => clicked = true));
				while (!clicked)
					yield return null;
				Sound.Play("receiptRip", Vector2.zero, true, false, volume: 0.5f, noReverb: true, ignoreMixer: true);
				timer = 1f;
				origPos = new Vector2(-336f, -900f);
				newPos = new Vector2(-400f, 0.0f);
				while (timer > 0.0f) {
					timer -= Time.unscaledDeltaTime * 1.8f;
					__instance.deathStats.anchoredPosition = Vector2.LerpUnclamped(origPos, newPos, Utils.OutQuart(1f - timer));
					yield return null;
				}
				__instance.deathStats.anchoredPosition = newPos;
		    }
		    __instance.endScreen.gameObject.SetActive(true);
		    yield return null;
		}

		private static void SetEndBackground(PlayerCamera __instance) {
			// Drowned
			if (__instance.body.inWater)
				__instance.endScreen.sprite = __instance.deathScreenSprites[2];
			// Bleeding
			else if ((double)__instance.body.totalBleedSpeed <= 0.019999999552965164)
				__instance.endScreen.sprite = __instance.deathScreenSprites[0];
			// Normal
			else
				__instance.endScreen.sprite = __instance.deathScreenSprites[1];
		}

		private static string GetStat(Dictionary<string, IStat> stats, string name, int decimal_place = 0) {
			// TODO: Empty checking
			return ColorVar(stats[name].GetValue(decimal_place));
		}
		
		private static string ColorVar(object variable) {
			return $"<color={variable_color}>{variable}</color>";
		}
	}
}