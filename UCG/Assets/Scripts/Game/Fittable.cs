using UnityEngine;

public class Fittable : MonoBehaviour
{
	public BoxCollider dimensions;
	public GameObject root;

	public void SetVisible(bool state) => root.SetActive(state);
}
