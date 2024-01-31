using ExtremeRoles.Extension.Controller;
using ExtremeRoles.Module;
using HarmonyLib;

using UnityEngine;

namespace ExtremeRoles.Beta;

[HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowDefaultNavigation))]
public static class FeedBackSystem
{
	public static void Postfix(EndGameNavigation __instance)
	{
		if (!PublicBeta.Instance.IsEnableWithMode) { return; }

		var button = Resources.Loader.CreateSimpleButton(
			__instance.ExitButton.transform);

        button.ClickedEvent.AddListener(
			() => Application.OpenURL("https://forms.gle/gfyspotFxYQ2zXR1A"));
        button.transform.localPosition = new Vector2(8.25f, 3.75f);
		button.Scale = new Vector3(0.75f, 0.5f, 1.0f);
		button.Text.text = TranslationControllerExtension.GetString("SendFeedBackToExR");
		button.Text.fontSize = button.Text.fontSizeMax = button.Text.fontSizeMin = 2.75f;
		button.gameObject.SetActive(true);
	}
}
