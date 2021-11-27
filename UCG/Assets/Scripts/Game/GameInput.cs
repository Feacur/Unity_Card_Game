using UnityEngine;

public class GameInput : MonoBehaviour
{
	public new Camera camera;

	public Fitter[] suspendables;

	//

	private struct State
	{
		public IHoverable hoverable;
		public IDraggable picked;
		public int pickedIndex;
		public Fitter pickedParent;
	};
	private State state;

	private void UpdateHover(GameObject hovered, Vector3 position)
	{
		IHoverable currentHoverable = hovered?.GetComponent<IHoverable>();

		if (state.hoverable != null)
		{
			if (ReferenceEquals(currentHoverable, state.hoverable))
			{
				state.hoverable.OnUpdate(position);
				return;
			}

			state.hoverable.OnExit(position);
			state.hoverable = null;
		}

		if (currentHoverable != null)
		{
			state.hoverable = currentHoverable;
			currentHoverable.OnEnter(position);
		}
	}

	private void UpdatePick(GameObject hovered, Vector3 position)
	{
		if (!hovered) { return; }

		IDraggable draggable = hovered.GetComponent<IDraggable>();
		if (draggable == null) { return; }

		state.pickedParent = hovered.GetComponentInParent<Fitter>();
		if (!state.pickedParent) { return; }

		foreach (Fitter it in suspendables)
		{
			it.SetElementsInteractable(state: false);
		}

		state.picked = draggable;
		state.picked.OnPick(position);
		state.pickedIndex = state.picked.GetGO().transform.GetSiblingIndex();
		state.picked.GetGO().SetActive(false);

		if (state.pickedParent.yankOnSelect)
		{
			state.picked.GetGO().transform.parent = null;
		}
	}

	private void UpdateDrag(GameObject hovered, Vector3 position)
	{
		if (state.picked == null) { return; }
		state.picked.OnUpdate(position);
	}

	private void UpdateDrop(GameObject hovered, Vector3 position)
	{
		State state = this.state;
		this.state = default;

		state.hoverable?.OnExit(position);

		if (state.picked == null) { return; }
		state.picked.OnDrop(position);
		state.picked.GetGO().SetActive(true);

		if (state.pickedParent.yankOnSelect && (state.hoverable == null || hovered == state.pickedParent.gameObject))
		{
			state.pickedParent.EmplaceActive(
				state.picked.GetGO().GetComponent<Fittable>(),
				state.hoverable != null
					? state.pickedParent.CalculateFittableIndex(state.pickedParent.GetActiveCount() + 1, position.x)
					: state.pickedIndex
			);
			state.pickedParent.AdjustPositions();
			goto finalize;
		}

		if (state.hoverable == null) { goto finalize; }

		DropArea dropArea = hovered.GetComponent<DropArea>();
		if (dropArea && dropArea.OnDrop(state.picked, position))
		{
			state.pickedParent.Remove(state.picked.GetGO().transform.GetSiblingIndex());
			state.pickedParent.AdjustPositions();
		}

		finalize:
		foreach (Fitter it in suspendables)
		{
			it.SetElementsInteractable(state: true);
		}
	}

	// MonoBehaviour

	private void Update()
	{
		Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(inputRay, out RaycastHit hit);

		GameObject hovered = hit.transform ? hit.transform.gameObject : null;
		if (state.picked != null)
		{
			UpdateHover(hovered, hit.point);
		}

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
