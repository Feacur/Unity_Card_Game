using UnityEngine;

public class GameInput : MonoBehaviour
{
	public new Camera camera;

	public GameObject[] suspendablesRoots;

	//

	private struct State
	{
		public GameObject hoveredObject;
		public Fittable hoveredFittable, selectedFittable;
		public DropArea dropArea;
		public Fitter sourceFitter;
		public int selectedIndex;
	};
	private State state;

	private void SetSuspendState(bool state)
	{
		if (suspendablesRoots == null) { return; }
		foreach (GameObject it in suspendablesRoots)
		{
			foreach (Collider collider in it.GetComponentsInChildren<Collider>(includeInactive: true))
			{
				collider.enabled = state;
			}
		}
	}

	private void UpdateHover(Vector3 position)
	{
		if (!state.selectedFittable) { return; }

		DropArea stateDropArea = state.dropArea;
		DropArea hoveredDropArea = state.hoveredObject.GetComponent<DropArea>();

		if (stateDropArea)
		{
			if (hoveredDropArea && hoveredDropArea == stateDropArea)
			{
				stateDropArea.OnHoverUpdate(position);
				return;
			}

			state.dropArea = null;
			stateDropArea.OnHoverExit(position);
		}

		if (!hoveredDropArea) { return; }

		Card stateSelectedCard = state.selectedFittable.GetComponent<Card>();
		if (hoveredDropArea.team != stateSelectedCard.team) { return; }

		state.dropArea = hoveredDropArea;
		hoveredDropArea.OnHoverEnter(position);
	}

	private void UpdatePick(Vector3 position)
	{
		SetSuspendState(false);
		if (!state.hoveredFittable) { return; }

		state.sourceFitter = state.hoveredFittable.GetComponentInParent<Fitter>();
		if (!state.sourceFitter) { return; }

		state.selectedFittable = state.hoveredFittable;
		state.selectedFittable.gameObject.SetActive(false);

		if (state.sourceFitter.yankOnSelect)
		{
			state.selectedIndex = state.selectedFittable.transform.GetSiblingIndex();
			state.selectedFittable.transform.parent = null;
		}
	}

	// private void UpdateDrag(Vector3 position)
	// {
	// 	if (!state.selectedCard) { return; }
	// 	state.selectedCard.transform.localPosition = position;
	// }

	private void UpdateDrop(Vector3 position)
	{
		SetSuspendState(true);

		Fitter stateSourceFitter = state.sourceFitter;
		state.sourceFitter = null;

		DropArea stateDropArea = state.dropArea;
		state.dropArea = null;
		stateDropArea?.OnHoverExit(position);

		Fittable stateSelectedFittable = state.selectedFittable;
		state.selectedFittable = null;

		if (!stateSelectedFittable) { return; }
		stateSelectedFittable.gameObject.SetActive(true);

		if (stateSourceFitter.yankOnSelect && (!stateDropArea || stateDropArea.gameObject == stateSourceFitter.gameObject))
		{
			foreach (Collider collider in stateSelectedFittable.GetComponentsInChildren<Collider>(includeInactive: true))
			{
				collider.enabled = true;
			}

			stateSourceFitter.EmplaceActive(
				stateSelectedFittable.GetComponent<Fittable>(),
				stateDropArea
					? stateSourceFitter.CalculateFittableIndex(stateSourceFitter.GetActiveCount() + 1, position.x)
					: state.selectedIndex
			);
			stateSourceFitter.AdjustPositions();
			return;
		}

		if (!stateDropArea) { return; }

		Card stateSelectedCard = stateSelectedFittable.GetComponent<Card>();
		if (stateDropArea.team != stateSelectedCard.team) { return; }

		if (stateDropArea.OnDrop(stateSelectedFittable, position))
		{
			stateSourceFitter.Remove(stateSelectedFittable.transform.GetSiblingIndex());
			stateSourceFitter.AdjustPositions();
		}
	}

	// MonoBehaviour

	private void Update()
	{
		Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(inputRay, out RaycastHit hit);
		state.hoveredObject = hit.transform ? hit.transform.gameObject : null;

		if (state.hoveredObject)
		{
			state.hoveredFittable = state.hoveredObject.GetComponent<Fittable>();
			UpdateHover(hit.point);
		}

		if (Input.GetMouseButtonDown(0))
		{
			UpdatePick(hit.point);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			UpdateDrop(hit.point);
		}
		// else
		// {
		// 	UpdateDrag(hit.point);
		// }
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(0, 0, 250, 180),     $"state:");
		GUI.Label(new Rect(10,  30, 250, 30), $"hovered object .. {(state.hoveredObject ? state.hoveredObject.name : "-")}");
		GUI.Label(new Rect(10,  60, 250, 30), $"hovered card .... {(state.hoveredFittable ? state.hoveredFittable.name : "-")}");
		GUI.Label(new Rect(10,  90, 250, 30), $"> its index ..... {(state.hoveredFittable ? state.hoveredFittable.transform.GetSiblingIndex().ToString() : "-")}");
		GUI.Label(new Rect(10, 120, 250, 30), $"selected card ... {(state.selectedFittable ? state.selectedFittable.name : "-")}");
		GUI.Label(new Rect(10, 150, 250, 30), $"> its index ..... {(state.selectedFittable ? state.selectedFittable.transform.GetSiblingIndex().ToString() : "-")}");
	}
}
