using SemanticVersioning;

using Il2CppCollection = Il2CppSystem.Collections.Generic;

namespace ExtremeRoles.Beta;

public class BetaContentManager
{
	public const string Version = "1.0.0";

	public const string NewTransDataPath = "ExtremeRoles.Beta.Resources.JsonData.TextRevamp.json";

	public const string TransKey = "PublicBetaContent";

	// ここでベータモードで有効になっている物一覧を追加する
	public static void AddContentText(
		SupportedLangs curLang,
		Il2CppCollection.Dictionary<string, string> transData)
	{
		string content = curLang switch
		{
			SupportedLangs.Japanese =>
				"・役職説明のテキストの改善/変更\n・フィードバックシステムの追加\n・「挙手する」ボタンをトグル式に変更",
			_ => "",
		};
		transData.Add(TransKey, content);

		// フィードバックを送るよう
		transData.Add("SendFeedBackToExR", curLang switch
		{
			SupportedLangs.Japanese => "フィードバックを送る",
			_ => "",
		});
	}
}
