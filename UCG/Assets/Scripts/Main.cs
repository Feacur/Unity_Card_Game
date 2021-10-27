using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
	public string NextScene = "Scenes/Battle.unity";

	static Main()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
	}

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

		if (gameObject.scene.path != RuntimeConfig.MainScene)
		{
			Debug.LogError($"this script is meant for '{RuntimeConfig.MainScene}'");
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}
	}

	async void Start()
	{
		await RuntimeConfig.InitStreamingConfig();
		RuntimeConfig.Log();
		LoadSceneAsync(NextScene);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SceneManager.LoadScene(RuntimeConfig.MainScene);
		}
	}

	private async static void LoadSceneAsync(string name)
	{
		AsyncOperationHandle<SceneInstance> sceneAsyncHandle = Addressables.LoadSceneAsync(name, LoadSceneMode.Additive);
		SceneInstance sceneInstance = await sceneAsyncHandle.Task;
		SceneManager.SetActiveScene(sceneInstance.Scene);
	}
}
