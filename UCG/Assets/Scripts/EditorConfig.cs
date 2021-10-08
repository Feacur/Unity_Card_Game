#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class EditorConfig
{
	public static BuildTarget Target => EditorUserBuildSettings.activeBuildTarget;

	public static string BuildFolder       => $"../{Config.Name}_Build";
	public static string BuildPlayerFolder => $"{BuildFolder}/{GetTargetName(Target)}";
	public static string BuildPlayerPath   => $"{BuildPlayerFolder}/{GetPackageName(Target)}";

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
			case BuildTarget.StandaloneWindows64: return Config.Name + ".exe";
			case BuildTarget.StandaloneLinux64:   return Config.Name;
			case BuildTarget.StandaloneOSX:       return Config.Name + ".app";
			case BuildTarget.WebGL:               return Config.Name;
			case BuildTarget.Android:             return Config.Name + ".apk";
			case BuildTarget.iOS:                 return Config.Name;
		}
		Debug.LogWarning("unknown package name");
		return "unknown";
	}
}
#endif
