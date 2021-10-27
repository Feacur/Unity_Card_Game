using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class StreamingAssets
{
	public static async Task<string> ReadText(string subPath)
	{
		string path = $"{Application.streamingAssetsPath}/{subPath}";
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
		await Task.Yield();
		return File.ReadAllText(path);
#elif UNITY_WEBGL || UNITY_ANDROID
		return await WebRequest.ReadText(path);
#else
		await Task.Yield();
		Debug.LogWarning("non-implemented streaming assets reader");
		return null;
#endif
	}
}
