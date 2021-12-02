using UnityEngine;
using UnityEngine.UI;

public class Fittable : MonoBehaviour
	, IFittable
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
	//     IDraggable
	// ----- ----- ----- ----- -----

	void IDraggable.OnPick(Vector3 position)
	{
		gameObject.SetActive(false);
	}

	void IDraggable.OnUpdate(Vector3 position)
	{

	}

	void IDraggable.OnDrop(Vector3 position)
	{
		gameObject.SetActive(true);
	}
}
