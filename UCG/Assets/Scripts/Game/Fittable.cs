using UnityEngine;
using UnityEngine.UI;

public class Fittable : MonoBehaviour
	, IFittable
	, IInteractable
	, IDraggable
{
	[SerializeField] private BoxCollider dimensions;
	[SerializeField] private Text content;
	[SerializeField] private int team;

	public Vector3 GetSize() => dimensions.size;

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     ICompatible
	// ----- ----- ----- ----- -----

	int ICompatible.GetTeam() => team;
	void ICompatible.SetTeam(int value) => team = value;

	// ----- ----- ----- ----- -----
	//     IFittable
	// ----- ----- ----- ----- -----

	string IFittable.GetContent() => content.text;
	void IFittable.SetContent(string text) => content.text = text;

	// ----- ----- ----- ----- -----
	//     IInteractable
	// ----- ----- ----- ----- -----

	bool IInteractable.GetState() => dimensions.enabled;
	void IInteractable.SetState(bool state) => dimensions.enabled = state;

	// ----- ----- ----- ----- -----
	//     IDraggable
	// ----- ----- ----- ----- -----

	void IDraggable.OnPick(Vector3 position)
	{

	}

	void IDraggable.OnUpdate(Vector3 position)
	{

	}

	void IDraggable.OnDrop(Vector3 position)
	{

	}
}
