using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Main : MonoBehaviour
{
	void Awake()
	{
		Debug.Log("Unity Card Game");
		Debug.Log($"assets path: {Config.AssetsPath}");
		TestAddressables();
	}

	private async static void TestAddressables()
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Prefab");
		await handle.Task;
		GameObject.Instantiate(handle.Result);
	}
}
