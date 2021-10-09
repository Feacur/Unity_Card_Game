using System.IO;
using System.Threading.Tasks;

public static class StreamingAssets
{
	public static async Task<string> ReadText(string path)
	{
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
		await Task.Yield();
		return File.ReadAllText(path);
#elif UNITY_WEBGL || UNITY_ANDROID
		return await WebRequest.ReadText(path);
#else
		await Task.Yield();
		Debug.LogWarning("non-implemented config reader");
		return null;
#endif
	}
}
