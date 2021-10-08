public static partial class Config
{
	public const string Name = "UCG";
	public const string MainScene = "Assets/Scenes/Main.unity";

	public static string AssetsPath
	{
		get
		{
#if UNITY_EDITOR
			return EditorConfig.BuildAssetsFolder;
#else
			return "Assets";
#endif
		}
	}
}
