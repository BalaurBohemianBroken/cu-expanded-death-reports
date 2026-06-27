using System.Collections;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using TMPro;
using System;
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
	
	[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.EndSequence))]
	public class EndScreenMovenext {
		// Making this stuff static is really bad form for IEnumerators usually.
		// Often there will be multiple of them, and they'll conflict!
		// But I don't think that's the case with this one.

		// public static bool Prefix(int type, PlayerCamera __instance, ref IEnumerator __result) {
		// 	BepInEx.Logging.Logger.CreateLogSource("ExpandedDeathReports.Patches").LogInfo("IEnumerator init patch");
		// 	__result = BalaurEndSequence(__instance, type);
		// 	return true;
		// }
		
		// How IEnumerator is working here is a total mystery to me.
		// I never assing to __result yet it does overwrite it
		[HarmonyPostfix]
		public static IEnumerator BalaurEndSequence(IEnumerator __result, PlayerCamera __instance, int type) {
			BepInEx.Logging.Logger.CreateLogSource("ExpandedDeathReports.Patches").LogInfo("Running custom end screen");
		    if (type == 0)
		    {
				PlayerPrefs.SetInt("deathcount", PlayerPrefs.GetInt("deathcount") + 1);
				__instance.endScreen.gameObject.SetActive(true);
				__instance.endScreen.sprite = !__instance.body.inWater ? ((double) __instance.body.totalBleedSpeed <= 0.019999999552965164 ? __instance.deathScreenSprites[0] : __instance.deathScreenSprites[1]) : __instance.deathScreenSprites[2];
				int num1 = Mathf.RoundToInt(__instance.body.AverageHappiness() * 0.1f);
				__instance.deathStats.GetChild(0).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenstatus");
				__instance.deathStats.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2XXX-" + DateTime.Now.ToString("MM-dd-HH-mm-ss");
				__instance.deathStats.GetChild(2).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenunresolved");
				__instance.deathStats.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"#{WoundView.specimenId.ToString()}-SAW-01";
				__instance.deathStats.GetChild(4).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenphysical");
				__instance.deathStats.GetChild(5).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreendeceased");
				__instance.deathStats.GetChild(6).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreenmental");
				__instance.deathStats.GetChild(7).GetComponent<TextMeshProUGUI>().text = (Locale.GetOther("moodrange" + ((bool) (UnityEngine.Object) __instance.body.mindWipe ? (object) "wiped" : (object) num1)?.ToString()) + (__instance.body.succesfullyRolledLastStand ? Locale.GetOther("moodrangeyethopeful") : "")).ToUpper();
				__instance.deathStats.GetChild(8).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreendepth");
				TextMeshProUGUI component1 = __instance.deathStats.GetChild(9).GetComponent<TextMeshProUGUI>();
				int depth = (int) WorldGeneration.world.PlayerTotalDepthMeters();
				string str1 = depth + Locale.GetOther("endscreendepthres") + TimeSpan.FromSeconds((double) WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss");
				component1.text = str1;
				__instance.deathStats.GetChild(10).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreencalories");
				__instance.deathStats.GetChild(11).GetComponent<TextMeshProUGUI>().text = $"{__instance.caloriesConsumed}cal";
				__instance.deathStats.GetChild(12).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("endscreencasualties");
				TextMeshProUGUI cas_unknown = __instance.deathStats.GetChild(13).GetComponent<TextMeshProUGUI>();
				cas_unknown.text = $"{Locale.GetOther("endscreenunknown")} + {PlayerPrefs.GetInt("deathcount")}";
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
	}
}