using System.Collections;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using TMPro;
using System;
using System.Collections.Generic;
using BalaurBohemianBroken.StatTrackers;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BalaurBohemianBroken {
    [HarmonyPatch]
	public class EndScreenPatch {
        public static DeathReport reportInstance;
		// How IEnumerator is working here is a total mystery to me.
		// I never assign to __result yet it does overwrite it
		// Anyway, this effectively skips the existing method.
		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.EndSequence))]
		public static IEnumerator BalaurEndSequence(IEnumerator __result, PlayerCamera __instance, int type) {
			// TODO: Prevent duplicate notes and names across reports.
			BepInEx.Logging.Logger.CreateLogSource("ExpandedDeathReports.Patches").LogInfo("Running custom end screen");
		    if (type == 0) {
                int deaths = PlayerPrefs.GetInt("deathcount") + 1;
                ((DeathIndex)ExpandedDeathReports.StatTrackersAll["DeathIndex"]).value = deaths;
				PlayerPrefs.SetInt("deathcount", deaths);
				__instance.endScreen.gameObject.SetActive(true);
				SetEndBackground(__instance);
                reportInstance = new DeathReport(__instance.deathStats.gameObject, ExpandedDeathReports.StatTrackersAll);
                
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
                    reportInstance.deathStats.anchoredPosition = Vector2.LerpUnclamped(origPos, newPos, 1f - timer);
					yield return null;
				}
                reportInstance.deathStats.anchoredPosition = newPos;
				bool clicked = false;
                reportInstance.deathStats.GetComponent<Button>().onClick.AddListener((UnityAction) (() => clicked = true));
				while (!clicked)
					yield return null;
				Sound.Play("receiptRip", Vector2.zero, true, false, volume: 0.5f, noReverb: true, ignoreMixer: true);
				timer = 1f;
				origPos = new Vector2(-336f, -900f);
				newPos = new Vector2(-400f, 0.0f);
				while (timer > 0.0f) {
					timer -= Time.unscaledDeltaTime * 1.8f;
                    reportInstance.deathStats.anchoredPosition = Vector2.LerpUnclamped(origPos, newPos, Utils.OutQuart(1f - timer));
					yield return null;
				}
                reportInstance.deathStats.anchoredPosition = newPos;
		    }
		    // __instance.endScreen.gameObject.SetActive(true);
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
	}
}