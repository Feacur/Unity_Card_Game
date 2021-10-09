using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequest
{
	public static async Task<string> ReadText(string path)
	{
		using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
		{
			UnityWebRequestAsyncOperation webRequestAsyncOp = webRequest.SendWebRequest();
			await webRequestAsyncOp;
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.Success:
					return webRequest.downloadHandler.text;

				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.ProtocolError:
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError(webRequest.error);
					break;
			}
		}
		return null;
	}
}
