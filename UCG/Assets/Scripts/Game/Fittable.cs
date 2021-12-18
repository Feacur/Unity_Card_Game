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
	[SerializeField] float _rotationLimit = 20;
	[SerializeField] float _rotationMagnitude = 360;
	[SerializeField] float _rotationSpeed = 16;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	public Vector3 GetSize() => _dimensions.size;

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;
	Vector3 IGameObject.GetVisiblePosition() => transform.position;

	// ----- ----- ----- ----- -----
	//     ICompatible
	// ----- ----- ----- ----- -----

	int ICompatible.GetTeam() => _team;
	void ICompatible.SetTeam(int value) => _team = value;

	// ----- ----- ----- ----- -----
	//     IPreviewable
	// ----- ----- ----- ----- -----

	void IPreviewable.Show(GameInputData input)
	{
		Vector3 offsetDirection = transform.position - input.origin; offsetDirection.Normalize();
		_root.SetPositionAndRotation(
			transform.position + Vector3.forward * 0.5f - offsetDirection * 2,
			Quaternion.identity
		);
	}

	void IPreviewable.Hide(GameInputData input)
	{
		_root.localPosition = Vector3.zero;
		_root.localRotation = Quaternion.identity;
	}

	// ----- ----- ----- ----- -----
	//     IFittable
	// ----- ----- ----- ----- -----

	int IFittable.GetPosition() => transform.GetSiblingIndex();
	void IFittable.SetPosition(int index) => transform.SetSiblingIndex(index);

	string IFittable.GetContent() => _content.text;
	void IFittable.SetContent(string text) => _content.text = text;

	// ----- ----- ----- ----- -----
	//     IDraggable
	// ----- ----- ----- ----- -----

	void IDraggable.OnPick(GameInputData input)
	{
		Vector3 position = input.target - input.direction;
		transform.SetPositionAndRotation(position, Quaternion.identity);
	}

	void IDraggable.OnUpdate(GameInputData input)
	{
		Vector3 position = input.target - input.direction;

		Vector3 move = position - transform.position; move.y = 0;
		float moveMagnitude = move.magnitude;

		Quaternion targetRotation = Quaternion.identity;
		if (moveMagnitude > 0.01f)
		{
			float angle = Mathf.Min(moveMagnitude * _rotationMagnitude, _rotationLimit);
			Vector3 tangent = Vector3.Cross(move, Vector3.down); tangent.Normalize();
			targetRotation = Quaternion.AngleAxis(angle, tangent);
		}

		Quaternion rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
		transform.SetPositionAndRotation(position, rotation);
	}

	void IDraggable.OnDrop(GameInputData input)
	{
		// _root.localPosition = Vector3.zero;
		// _root.localRotation = Quaternion.identity;
	}
}
