using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
	void Awake()
	{
		Debug.Log("Test says hi!");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SceneManager.LoadScene(RuntimeConfig.MainScene);
		}
	}
}
