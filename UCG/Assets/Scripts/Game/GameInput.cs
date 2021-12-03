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

	private void UpdateHover(GameObject hovered, GameInputData input)
	{
		IHoverable currentHoverable = hovered?.GetComponent<IHoverable>();

		if (_state.hoverable != null)
		{
			if (ReferenceEquals(currentHoverable, _state.hoverable))
			{
				_state.hoverable.OnUpdate(_state.draggable, input);
				return;
			}

			_state.hoverable.OnExit(_state.draggable, input);
			_state.hoverable = null;
		}

		if (currentHoverable != null)
		{
			_state.hoverable = currentHoverable;
			currentHoverable.OnEnter(_state.draggable, input);
		}
	}

	private void UpdatePick(GameObject hovered, GameInputData input)
	{
		if (!hovered) { return; }

		_state.dragContainerSource = hovered.GetComponent<IDragContainer>();
		IDraggable draggable = _state.dragContainerSource?.OnPick(input);

		if (draggable != null)
		{
			_state.hoverable?.OnExit(_state.draggable, input);

			_state.draggable = draggable;
			_state.draggable.OnPick(input);

			_state.hoverable?.OnEnter(_state.draggable, input);
		}
	}

	private void UpdateDrag(GameObject hovered, GameInputData input)
	{
		_state.draggable?.OnUpdate(input);
	}

	private void UpdateDrop(GameObject hovered, GameInputData input)
	{
		State state = this._state;
		this._state = default;

		state.hoverable?.OnExit(state.draggable, input);

		bool dropResult = false;
		if (hovered)
		{
			IDragContainer hoveredDragContainer = hovered.GetComponent<IDragContainer>();
			dropResult = hoveredDragContainer?.OnDrop(state.draggable, input) ?? false;
		}

		state.draggable?.OnDrop(input);
		state.dragContainerSource?.OnPickEnd(input, dropResult);
	}

	// ----- ----- ----- ----- -----
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

	private void Update()
	{
		Ray worldRay = _camera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(worldRay, out RaycastHit hit);

		GameInputData input = new GameInputData {
			origin = _camera.transform.position,
			direction = worldRay.direction,
			target = hit.point,
		};

		GameObject hovered = hit.transform?.gameObject;
		UpdateHover(hovered, input);

		if (Input.GetMouseButtonDown(0))
		{
			UpdatePick(hovered, input);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			UpdateDrop(hovered, input);
		}
		else
		{
			UpdateDrag(hovered, input);
		}
	}
}
