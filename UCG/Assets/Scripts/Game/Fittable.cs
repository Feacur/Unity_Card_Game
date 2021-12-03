using UnityEngine;
using UnityEngine.UI;

public class Fittable : MonoBehaviour
	, IFittable
	, IDraggable
{
	[SerializeField] private BoxCollider _dimensions;
	[SerializeField] private Text _content;
	[SerializeField] private int _team;
	[SerializeField] Transform _root;

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

	void IDraggable.OnPick(Vector3 position, Vector3 viewDirection)
	{
		// gameObject.SetActive(false);
		_root.SetPositionAndRotation(
			position - viewDirection,
			Quaternion.identity
		);
	}

	void IDraggable.OnUpdate(Vector3 position, Vector3 viewDirection)
	{
		_root.SetPositionAndRotation(
			position - viewDirection,
			Quaternion.identity
		);
	}

	void IDraggable.OnDrop(Vector3 position, Vector3 viewDirection)
	{
		// gameObject.SetActive(true);
		_root.localPosition = Vector3.zero;
		_root.localRotation = Quaternion.identity;
	}
}
