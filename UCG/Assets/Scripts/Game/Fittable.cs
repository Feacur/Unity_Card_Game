using UnityEngine;
using UnityEngine.UI;

public class Fittable : MonoBehaviour
	, IContent
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
	//     IComponent
	// ----- ----- ----- ----- -----

	GameObject IComponent.GetGO() => gameObject;
	T IComponent.GetComponent<T>() => GetComponent<T>();

	// ----- ----- ----- ----- -----
	//     ICompatible
	// ----- ----- ----- ----- -----

	int ICompatible.GetTeam() => _team;

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
	//     IContent
	// ----- ----- ----- ----- -----

	void IContent.Set(int team, string text)
	{
		_team = team;
		_content.text = text;
	}

	// ----- ----- ----- ----- -----
	//     IFittable
	// ----- ----- ----- ----- -----

	int IFittable.GetPosition() => transform.GetSiblingIndex();
	void IFittable.SetPosition(int index) => transform.SetSiblingIndex(index);

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
		const float DragHeight = 1;
		float distance = DragHeight / Vector3.Dot(input.direction, Vector3.up);
		Vector3 position = input.target + input.direction * distance;

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
