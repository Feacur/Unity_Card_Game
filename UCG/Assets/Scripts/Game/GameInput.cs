using UnityEngine;

public class GameInput : MonoBehaviour
{
	public new Camera camera;

	//

	private struct State
	{
		public GameObject hoveredObject;
		public Card hoveredCard, selectedCard;
	};
	private State state;

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
				Debug.Log($"picked a card");
			}
			else
			{
				Debug.Log($"picked nothing");
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (state.selectedCard)
			{
				if (!state.hoveredCard)
				{
					Debug.Log($"dropped a card");
				}
				else if (state.hoveredCard == state.selectedCard)
				{
					Debug.Log($"dropped a card onto itself");
				}
				else
				{
					Debug.Log($"dropped a card onto another one");
				}
				state.selectedCard = null;
			}
		}
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(0, 0, 200, 180),     $"state:");
		GUI.Label(new Rect(10,  30, 200, 30), $"hovered object .... {(state.hoveredObject ? state.hoveredObject.name : "-")}");
		GUI.Label(new Rect(10,  60, 200, 30), $"hovered card ...... {(state.hoveredCard ? state.hoveredCard.name : "-")}");
		GUI.Label(new Rect(10,  90, 200, 30), $"> its index ....... {(state.hoveredCard ? state.hoveredCard.transform.GetSiblingIndex().ToString() : "-")}");
		GUI.Label(new Rect(10, 120, 200, 30), $"selected card ..... {(state.selectedCard ? state.selectedCard.name : "-")}");
		GUI.Label(new Rect(10, 150, 200, 30), $"> its index ....... {(state.selectedCard ? state.selectedCard.transform.GetSiblingIndex().ToString() : "-")}");
	}
}

/*
- hand cards [temporary] dissapear upon drag
- hero power, target: arrow from hero power
- spell cards, no targets: drag
- spell cards, target: arrow from hero card
- minion cards: push other minions
*/
