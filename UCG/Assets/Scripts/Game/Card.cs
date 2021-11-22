using UnityEngine;

public class Card : MonoBehaviour
{
	public GameObject root;
	public TMPro.TextMeshPro content;

	public void SetVisible(bool state) => root.SetActive(state);

	public void SetContent(string text)
	{
		content.text = text;
	}
}
