using System.Threading.Tasks;
using UnityEngine;

public static class RuntimeConfig
{
	public static string ProductName   => Application.productName;
	public static string MainScene     => "Assets/Scenes/Main.unity";
	public static string AssetsSubpath => "Assets";
	public static string ConfigSubpath => "Config.json";

	public static StreamingConfig streamingConfig;

	public static string AssetsPath
	{
		get
		{
#if UNITY_EDITOR
			return BuildConfig.AssetsFolder;
#elif UNITY_WEBGL
			return $"{Application.dataPath}/{AssetsSubpath}";
#else
			return AssetsSubpath;
#endif
		}
	}

	public static async Task InitStreamingConfig()
	{
		string streamingConfigText = await StreamingAssets.ReadText(ConfigSubpath);
		if (string.IsNullOrEmpty(streamingConfigText))
		{
			Debug.LogWarning("empty streaming config text, using defaults");
			RuntimeConfig.streamingConfig = new StreamingConfig {
				CDN = "http://127.0.0.1:8000"
			};
		}
		else
		{
			RuntimeConfig.streamingConfig = JsonUtility.FromJson<StreamingConfig>(streamingConfigText);
		}
		Debug.Log($"streaming config:\n{JsonUtility.ToJson(RuntimeConfig.streamingConfig, prettyPrint: true)}");
	}

	public static void Log()
	{
		Debug.Log(@$"runtime config:
- product name:   {ProductName}
- main scene:     {MainScene}
- assets subpath: {AssetsSubpath}
- config subpath: {ConfigSubpath}
- assets path:    {AssetsPath}
"
		);
	}
}
