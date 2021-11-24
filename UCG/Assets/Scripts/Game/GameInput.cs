using UnityEngine;

public class GameInput : MonoBehaviour
{
	public new Camera camera;

	public GameObject[] suspendablesRoots;

	//

	private struct State
	{
		public GameObject hoveredObject;
		public Card hoveredCard, selectedCard;
		public DropArea dropArea;
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
		if (!state.selectedCard) { return; }

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

		if (hoveredDropArea && hoveredDropArea.team == state.selectedCard.team)
		{
			state.dropArea = hoveredDropArea;
			hoveredDropArea.OnHoverEnter(position);
		}
	}

	private void UpdatePick(Vector3 position)
	{
		SetSuspendState(false);
		if (!state.hoveredCard) { return; }
		state.selectedCard = state.hoveredCard;
		state.selectedCard.SetVisible(false);
	}

	private void UpdateDrop(Vector3 position)
	{
		SetSuspendState(true);

		DropArea stateDropArea = state.dropArea;
		state.dropArea = null;
		stateDropArea?.OnHoverExit(position);

		Card stateSelectedCard = state.selectedCard;
		state.selectedCard = null;
		stateSelectedCard?.SetVisible(true);

		if (!stateSelectedCard) { return; }
		if (!stateDropArea) { return; }
		if (stateDropArea.team != stateSelectedCard.team) { return; }

		Fitter cardFitter = stateSelectedCard.GetComponentInParent<Fitter>();
		if (cardFitter)
		{
			cardFitter.Remove(stateSelectedCard.transform.GetSiblingIndex());
			cardFitter.AdjustPositions();
		}
		stateDropArea.OnDrop(stateSelectedCard, position);
	}

	// MonoBehaviour

	private void Update()
	{
		Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(inputRay, out RaycastHit hit);
		state.hoveredObject = hit.transform ? hit.transform.gameObject : null;

		if (state.hoveredObject)
		{
			state.hoveredCard = state.hoveredObject.GetComponent<Card>();
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
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(0, 0, 250, 180),     $"state:");
		GUI.Label(new Rect(10,  30, 250, 30), $"hovered object .... {(state.hoveredObject ? state.hoveredObject.name : "-")}");
		GUI.Label(new Rect(10,  60, 250, 30), $"hovered card ...... {(state.hoveredCard ? state.hoveredCard.name : "-")}");
		GUI.Label(new Rect(10,  90, 250, 30), $"> its index ....... {(state.hoveredCard ? state.hoveredCard.transform.GetSiblingIndex().ToString() : "-")}");
		GUI.Label(new Rect(10, 120, 250, 30), $"selected card ..... {(state.selectedCard ? state.selectedCard.name : "-")}");
		GUI.Label(new Rect(10, 150, 250, 30), $"> its index ....... {(state.selectedCard ? state.selectedCard.transform.GetSiblingIndex().ToString() : "-")}");
	}
}
