#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class BuildConfig
{
	public static BuildTarget Target => EditorUserBuildSettings.activeBuildTarget;

	public static string Folder       => $"../{Config.ProductName}_Build";
	public static string PlayerFolder => $"{Folder}/{GetTargetName(Target)}";
	public static string PlayerPath   => $"{PlayerFolder}/{GetPackageName(Target)}";
	public static string AssetsFolder => $"{PlayerFolder}/{GetAssetsSubpath(Target)}";
	public static string AssetsState  => $"{Folder}/ContentState";

	// ----- ----- ----- ----- -----
	//     Enum convertors
	// ----- ----- ----- ----- -----

	private static string GetTargetName(BuildTarget value)
	{
		switch (Target)
		{
			case BuildTarget.StandaloneWindows64: return "Windows";
			case BuildTarget.StandaloneLinux64:   return "Linux";
			case BuildTarget.StandaloneOSX:       return "OSX";
			case BuildTarget.WebGL:               return "WebGL";
			case BuildTarget.Android:             return "Android";
			case BuildTarget.iOS:                 return "iOS";
		}
		Debug.LogWarning("unknown target name");
		return "unknown";
	}

	private static string GetPackageName(BuildTarget value)
	{
		switch (value)
		{
			case BuildTarget.StandaloneWindows64: return Config.ProductName + ".exe";
			case BuildTarget.StandaloneLinux64:   return Config.ProductName;
			case BuildTarget.StandaloneOSX:       return Config.ProductName + ".app";
			case BuildTarget.WebGL:               return Config.ProductName;
			case BuildTarget.Android:             return Config.ProductName + ".apk";
			case BuildTarget.iOS:                 return Config.ProductName;
		}
		Debug.LogWarning("unknown package name");
		return "unknown";
	}

	private static string GetAssetsSubpath(BuildTarget value)
	{
		switch (value)
		{
			case BuildTarget.StandaloneWindows64: return "Assets";
			case BuildTarget.StandaloneLinux64:   return "Assets";
			case BuildTarget.StandaloneOSX:       return "Assets";
			case BuildTarget.WebGL:               return $"{Config.ProductName}/Assets";
			case BuildTarget.Android:             return "Assets";
			case BuildTarget.iOS:                 return "Assets";
		}
		Debug.LogWarning("unknown assets subpath");
		return "unknown";
	}
}
#endif
