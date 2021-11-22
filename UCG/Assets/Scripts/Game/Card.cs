using UnityEngine;

public class Card : MonoBehaviour
{
	public GameObject root;

	public void SetVisible(bool state) => root.SetActive(state);
}
