using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
	void Awake()
	{
		Debug.Log(@$"unity application:
- product name:          {Application.productName}
- version:               {Application.version}
- unity version:         {Application.unityVersion}
- company name:          {Application.companyName}
- platform:              {Application.platform}
- system language:       {Application.systemLanguage}
- absolute URL:          {Application.absoluteURL}
- data path:             {Application.dataPath}
- console log path:      {Application.consoleLogPath}
- temporary cache path:  {Application.temporaryCachePath}
- persitent data path:   {Application.persistentDataPath}
- streaming assets path: {Application.streamingAssetsPath}
"
		);
	}

	async void Start()
	{
		await RuntimeConfig.InitStreamingConfig();
		RuntimeConfig.Log();
		TestAddressables();
	}

	private async static void TestAddressables()
	{
		AsyncOperationHandle<SceneInstance> sceneAsyncHandle = Addressables.LoadSceneAsync("Scene_1", LoadSceneMode.Additive);
		SceneInstance sceneInstance = await sceneAsyncHandle.Task;
		SceneManager.SetActiveScene(sceneInstance.Scene);

		AsyncOperationHandle<GameObject> prefavSyncHandle = Addressables.LoadAssetAsync<GameObject>("Prefab");
		GameObject prefab = await prefavSyncHandle.Task;
		GameObject.Instantiate(prefab);
	}
}
