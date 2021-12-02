using UnityEngine;
using UnityEngine.UI;

public class Fittable : MonoBehaviour
	, IFittable
	, IInteractable
	, IDraggable
{
	[SerializeField] private BoxCollider _dimensions;
	[SerializeField] private Text _content;
	[SerializeField] private int _team;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	public Vector3 GetSize() => _dimensions.size;

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     ICompatible
	// ----- ----- ----- ----- -----

	int ICompatible.GetTeam() => _team;
	void ICompatible.SetTeam(int value) => _team = value;

	// ----- ----- ----- ----- -----
	//     IFittable
	// ----- ----- ----- ----- -----

	string IFittable.GetContent() => _content.text;
	void IFittable.SetContent(string text) => _content.text = text;

	// ----- ----- ----- ----- -----
	//     IInteractable
	// ----- ----- ----- ----- -----

	bool IInteractable.GetState() => _dimensions.enabled;
	void IInteractable.SetState(bool state) => _dimensions.enabled = state;

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
