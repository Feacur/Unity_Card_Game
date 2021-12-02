using UnityEngine;

public class GameInput : MonoBehaviour
{
	// ----- ----- ----- ----- -----
	//     Dependencies
	// ----- ----- ----- ----- -----

	public new Camera camera;

	public Fitter[] suspendables;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	private struct State
	{
		public IHoverable hoverable;
		public IDragContainer dragContainerSource;
		public IDraggable draggable;
	};
	private State state;

	private void UpdateHover(GameObject hovered, Vector3 position)
	{
		IHoverable currentHoverable = hovered?.GetComponent<IHoverable>();

		if (state.hoverable != null)
		{
			if (ReferenceEquals(currentHoverable, state.hoverable))
			{
				state.hoverable.OnUpdate(state.draggable, position);
				return;
			}

			state.hoverable.OnExit(state.draggable, position);
			state.hoverable = null;
		}

		if (currentHoverable != null)
		{
			state.hoverable = currentHoverable;
			currentHoverable.OnEnter(state.draggable, position);
		}
	}

	private void UpdatePick(GameObject hovered, Vector3 position)
	{
		if (!hovered) { return; }
		state.dragContainerSource = hovered.GetComponent<IDragContainer>();

		IDraggable draggable = state.dragContainerSource?.OnPick(position);
		if (draggable != null)
		{
			state.hoverable?.OnExit(state.draggable, position);

			state.draggable = draggable;
			state.draggable.OnPick(position);

			state.hoverable?.OnEnter(state.draggable, position);
		}
	}

	private void UpdateDrag(GameObject hovered, Vector3 position)
	{
		if (state.draggable == null) { return; }
		state.draggable.OnUpdate(position);
	}

	private void UpdateDrop(GameObject hovered, Vector3 position)
	{
		State state = this.state;
		this.state = default;

		bool dropResult = false;
		if (hovered)
		{
			state.hoverable?.OnExit(state.draggable, position);

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
		Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
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
