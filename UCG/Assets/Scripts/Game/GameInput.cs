using UnityEngine;

public class GameInput : MonoBehaviour
{
	// ----- ----- ----- ----- -----
	//     Dependencies (external)
	// ----- ----- ----- ----- -----

	[SerializeField] private Camera _camera;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	private struct State
	{
		public IHoverable hoverable;
		public IDragContainer dragContainerSource;
		public IDraggable draggable;
	};
	private State _state;

	private void UpdateHover(GameObject hovered, Vector3 position)
	{
		IHoverable currentHoverable = hovered?.GetComponent<IHoverable>();

		if (_state.hoverable != null)
		{
			if (ReferenceEquals(currentHoverable, _state.hoverable))
			{
				_state.hoverable.OnUpdate(_state.draggable, position);
				return;
			}

			_state.hoverable.OnExit(_state.draggable, position);
			_state.hoverable = null;
		}

		if (currentHoverable != null)
		{
			_state.hoverable = currentHoverable;
			currentHoverable.OnEnter(_state.draggable, position);
		}
	}

	private void UpdatePick(GameObject hovered, Vector3 position)
	{
		if (!hovered) { return; }

		_state.dragContainerSource = hovered.GetComponent<IDragContainer>();
		IDraggable draggable = _state.dragContainerSource?.OnPick(position);

		if (draggable != null)
		{
			_state.hoverable?.OnExit(_state.draggable, position);

			_state.draggable = draggable;
			_state.draggable.OnPick(position);

			_state.hoverable?.OnEnter(_state.draggable, position);
		}
	}

	private void UpdateDrag(GameObject hovered, Vector3 position)
	{
		_state.draggable?.OnUpdate(position);
	}

	private void UpdateDrop(GameObject hovered, Vector3 position)
	{
		State state = this._state;
		this._state = default;

		state.hoverable?.OnExit(state.draggable, position);

		bool dropResult = false;
		if (hovered)
		{
			IDragContainer hoveredDragContainer = hovered.GetComponent<IDragContainer>();
			dropResult = hoveredDragContainer?.OnDrop(state.draggable, position) ?? false;
		}

		state.draggable?.OnDrop(position);
		state.dragContainerSource?.OnPickEnd(position, dropResult);
	}

	// ----- ----- ----- ----- -----
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

	private void Update()
	{
		Ray inputRay = _camera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(inputRay, out RaycastHit hit);

		GameObject hovered = hit.transform?.gameObject;
		UpdateHover(hovered, hit.point);

		if (Input.GetMouseButtonDown(0))
		{
			UpdatePick(hovered, hit.point);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			UpdateDrop(hovered, hit.point);
		}
		else
		{
			UpdateDrag(hovered, hit.point);
		}
	}
}
