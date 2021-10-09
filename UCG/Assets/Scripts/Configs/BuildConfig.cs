#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BuildConfig
{
	public static BuildTarget Target => EditorUserBuildSettings.activeBuildTarget;

	public static string Folder       => $"../{RuntimeConfig.ProductName}_Build";
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
			case BuildTarget.StandaloneWindows64: return RuntimeConfig.ProductName + ".exe";
			case BuildTarget.StandaloneLinux64:   return RuntimeConfig.ProductName;
			case BuildTarget.StandaloneOSX:       return RuntimeConfig.ProductName + ".app";
			case BuildTarget.WebGL:               return RuntimeConfig.ProductName;
			case BuildTarget.Android:             return RuntimeConfig.ProductName + ".apk";
			case BuildTarget.iOS:                 return RuntimeConfig.ProductName;
		}
		Debug.LogWarning("unknown package name");
		return "unknown";
	}

	private static string GetAssetsSubpath(BuildTarget value)
	{
		switch (value)
		{
			case BuildTarget.StandaloneWindows64: return RuntimeConfig.AssetsSubpath;
			case BuildTarget.StandaloneLinux64:   return RuntimeConfig.AssetsSubpath;
			case BuildTarget.StandaloneOSX:       return RuntimeConfig.AssetsSubpath;
			case BuildTarget.WebGL:               return $"{RuntimeConfig.ProductName}/{RuntimeConfig.AssetsSubpath}";
			case BuildTarget.Android:             return RuntimeConfig.AssetsSubpath;
			case BuildTarget.iOS:                 return RuntimeConfig.AssetsSubpath;
		}
		Debug.LogWarning("unknown assets subpath");
		return "unknown";
	}
}
#endif
