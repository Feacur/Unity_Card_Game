using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets;

public static class Builder
{
	[InitializeOnLoadMethod]
	private static void Initialize()
	{
		BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
		void BuildPlayerHandler(BuildPlayerOptions options)
		{
			Debug.LogWarning("use [Build/Player/%Configuration%] instead");
		}
	}

	// ----- ----- ----- ----- -----
	//     Build player
	// ----- ----- ----- ----- -----

	[MenuItem("Build/Player/Optimized")]
	public static void Build_Player_Optimized() => BuildPlayer(
		BuildOptions.None
	);

	[MenuItem("Build/Player/Development")]
	public static void Build_Player_Development() => BuildPlayer(
		BuildOptions.Development
	);

	[MenuItem("Build/Player/Development (scripts)")]
	public static void Build_Player_DevelopmentScripts() => BuildPlayer(
		BuildOptions.Development | BuildOptions.BuildScriptsOnly
	);

	[MenuItem("Build/Player/Debug")]
	public static void Build_Player_Debug() => BuildPlayer(
		BuildOptions.Development | BuildOptions.AllowDebugging
	);

	[MenuItem("Build/Player/Debug (scripts)")]
	public static void Build_Player_DebugScripts() => BuildPlayer(
		BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.BuildScriptsOnly
	);

	// ----- ----- ----- ----- -----
	//     Build assets
	// ----- ----- ----- ----- -----

	[MenuItem("Build/Assets/Addressables")]
	public static void Build_Assets_Addressables() => BuildAddressableAssets();

	[MenuItem("Build/Assets/Bundles")]
	public static void Build_Assets_Bundles() => BuildAssetBundles();

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	private static void BuildPlayer(BuildOptions options)
	{
		BuildTarget             target                  = BuildConfig.Target;
		BuildTargetGroup        targetGroup             = GetBuildTargetGroup(target);
		ScriptingImplementation scriptingImplementation = PlayerSettings.GetScriptingBackend(targetGroup);
		ApiCompatibilityLevel   apiCompatibilityLevel   = PlayerSettings.GetApiCompatibilityLevel(targetGroup);

		options = PatchBuildOptions(options, target);

		BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
			target = target,
			targetGroup = targetGroup,
			locationPathName = BuildConfig.PlayerPath,
			scenes = new[] { RuntimeConfig.MainScene },
			options = options,
		});

		//
		double totalSeconds   = report.summary.totalTime.TotalSeconds;
		double totalMegabytes = report.summary.totalSize / (double)(1024*1024);
		string reportString = $"Build Windows 64: {report.summary.result}\n"
			+ $"- time: {totalSeconds:#,0.0} seconds\n"
			+ $"- size: {totalMegabytes:#,0.0} MB\n"
			+ $"- wrn: {report.summary.totalWarnings}\n"
			+ $"- err: {report.summary.totalErrors}\n";

		switch (report.summary.result)
		{
			case BuildResult.Succeeded: Debug.Log(reportString);        break;
			case BuildResult.Cancelled: Debug.LogWarning(reportString); break;
			case BuildResult.Unknown:   Debug.LogWarning(reportString); break;
			case BuildResult.Failed:    Debug.LogError(reportString);   break;
		}
	}

	private static void BuildAddressableAssets()
	{
		AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
		settings.ContentStateBuildPath = BuildConfig.AssetsState;
		EditorUtility.SetDirty(settings);

		AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult report);
		BuildResult reportResult = string.IsNullOrEmpty(report.Error) ? BuildResult.Succeeded : BuildResult.Failed;

		//
		string reportString = $"Build Assets: {reportResult}\n"
			+ $"- time: {report.Duration:#,0.0} seconds\n"
			+ $"- err: {report.Error}\n";

		switch (reportResult)
		{
			case BuildResult.Succeeded: Debug.Log(reportString);        break;
			case BuildResult.Failed:    Debug.LogError(reportString);   break;
		}
	}

	private static void BuildAssetBundles()
	{
		Debug.LogWarning("'BuildAssetBundles()' is not implemented");
	}

	private static BuildOptions PatchBuildOptions(BuildOptions options, BuildTarget target)
	{
		options |= BuildOptions.StrictMode;
		if ((options & BuildOptions.Development) == BuildOptions.Development)
		{
			switch (target)
			{
				case BuildTarget.Android:
					if ((options & BuildOptions.BuildScriptsOnly) == BuildOptions.BuildScriptsOnly)
					{
						options |= BuildOptions.PatchPackage;
					}
					break;

				case BuildTarget.iOS:
					options |= BuildOptions.SymlinkLibraries;
					break;
			}
		}
		return options;
	}

	// ----- ----- ----- ----- -----
	//     Enum convertors
	// ----- ----- ----- ----- -----

	private static string GetBackendName(ScriptingImplementation value)
	{
		switch (value)
		{
			case ScriptingImplementation.Mono2x: return "DotNet";
			case ScriptingImplementation.IL2CPP: return "IL2CPP";
		}
		Debug.LogWarning("unknown backend name");
		return "unknown";
	}

	private static string GetBackendVersion(ApiCompatibilityLevel value)
	{
		switch (value)
		{
			case ApiCompatibilityLevel.NET_Standard_2_0: return "2.0";
			case ApiCompatibilityLevel.NET_4_6:          return "4.x";
		}
		Debug.LogWarning("unknown backend version");
		return "unknown";
	}

	private static BuildTargetGroup GetBuildTargetGroup(BuildTarget value)
	{
		switch (value)
		{
			case BuildTarget.StandaloneWindows64: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneLinux64:   return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneOSX:       return BuildTargetGroup.Standalone;
			case BuildTarget.WebGL:               return BuildTargetGroup.WebGL;
			case BuildTarget.Android:             return BuildTargetGroup.Android;
			case BuildTarget.iOS:                 return BuildTargetGroup.iOS;
		}
		Debug.LogWarning("unknown build target group");
		return BuildTargetGroup.Unknown;
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

	private static string GetConfigurationName(BuildOptions value)
	{
		if ((value & BuildOptions.Development) == BuildOptions.Development)
		{
			if ((value & BuildOptions.AllowDebugging) == BuildOptions.AllowDebugging)
			{
				return "Debug";
			}
			return "Development";
		}
		return "Optimized";
	}
}
