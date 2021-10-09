using UnityEngine;

public static partial class Config
{
	public static string ProductName => Application.productName;
	public static string MainScene   => "Assets/Scenes/Main.unity";

	public static string AssetsPath
	{
		get
		{
#if UNITY_EDITOR
			return BuildConfig.AssetsFolder;
#else
			return "Assets";
#endif
		}
	}

// 	private static string TargetName
// 	{
// 		get
// 		{
// #if UNITY_STANDALONE_WIN && UNITY_64
// 			return "Windows";
// #elif UNITY_STANDALONE_LINUX && UNITY_64
// 			return "Linux";
// #elif UNITY_STANDALONE_OSX && UNITY_64
// 			return "OSX";
// #elif UNITY_WEBGL
// 			return "WebGL";
// #elif UNITY_ANDROID
// 			return "Android";
// #elif UNITY_IOS
// 			return "iOS";
// #else
// 		Debug.LogWarning("unknown target name");
// 		return "unknown";
// #endif
// 		}
// 	}
}
