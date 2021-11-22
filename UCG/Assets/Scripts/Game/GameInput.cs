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
	};
	private State state;

	private void SetSuspendState(bool state)
	{
		if (suspendablesRoots == null) { return; }
		foreach (GameObject it in suspendablesRoots)
		{
			foreach (Collider collider in it.GetComponentsInChildren<Collider>())
			{
				collider.enabled = state;
			}
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
			state.hoveredCard = state.hoveredObject.GetComponent<Card>();
		}

		if (Input.GetMouseButtonDown(0))
		{
			state.selectedCard = state.hoveredCard;
			if (state.selectedCard)
			{
				Debug.Log("picked a card");
				state.selectedCard.gameObject.SetActive(false);
				SetSuspendState(false);
			}
			else
			{
				Debug.Log("picked nothing");
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (state.selectedCard)
			{
				if (state.hoveredObject)
				{
					DropArea dropArea = state.hoveredObject.GetComponent<DropArea>();
					if (dropArea)
					{
						Debug.Log("dropped a card onto a drop area");
						Fitter cardFitter = state.selectedCard.GetComponentInParent<Fitter>();
						if (cardFitter)
						{
							state.selectedCard.gameObject.SetActive(true);
							cardFitter.SetCount(cardFitter.GetCount() - 1);
						}
						dropArea.OnDrop();
					}
					else
					{
						Debug.Log("dropped a card onto an object");
						state.selectedCard.gameObject.SetActive(true);
					}
				}
				else
				{
					Debug.Log("dropped a card onto nothing");
					state.selectedCard.gameObject.SetActive(true);
				}
				state.selectedCard = null;
				SetSuspendState(true);
			}
			else
			{
				Debug.Log("dropeed nothing");
			}
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

/*
- hand cards [temporary] dissapear upon drag
- hero power, target: arrow from hero power
- spell cards, no targets: drag
- spell cards, target: arrow from hero card
- minion cards: push other minions
*/
