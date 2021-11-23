using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	public GameObject root;
	public Text content;

	public void SetVisible(bool state) => root.SetActive(state);

	public string GetContent() => content.text;
	public void SetContent(string text) => content.text = text;
}
