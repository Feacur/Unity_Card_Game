using UnityEngine;

public class DropArea : FitterController
{
	public int team;

	private Fittable hoveredFittable;

	public bool OnDrop(Card card, Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		int count = fitter.GetActiveCount();
		if (HaveSpace(count))
		{
			Fittable fittable = fitter.Add();

			int index = fitter.CalculateFittableIndex(position);
			fittable.transform.SetSiblingIndex(index);
			fitter.AdjustPositions();

			Card newCard = fittable.GetComponent<Card>();
			if (newCard)
			{
				newCard.SetContent(card.GetContent());
				newCard.team = team;
			}
			return true;
		}
		return false;
	}

	public void OnHoverEnter(Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		if (!HaveSpace(fitter.GetActiveCount())) { return; }

		Fittable fittable = fitter.Add();
		hoveredFittable = fittable;

		foreach (Collider collider in fittable.GetComponentsInChildren<Collider>(includeInactive: true))
		{
			collider.enabled = false;
		}

		Card card = fittable.GetComponent<Card>();
		card?.SetVisible(false);

		int index = fitter.CalculateFittableIndex(position);
		fittable.transform.SetSiblingIndex(index);
		fitter.AdjustPositions();
	}

	public void OnHoverUpdate(Vector3 position)
	{
		if (!hoveredFittable) { return; }
		Fitter fitter = GetComponent<Fitter>();

		int index = fitter.CalculateFittableIndex(position);
		hoveredFittable.transform.SetSiblingIndex(index);
		fitter.AdjustPositions();
	}

	public void OnHoverExit(Vector3 position)
	{
		if (!hoveredFittable) { return; }
		Fitter fitter = GetComponent<Fitter>();

		Fittable fittable = hoveredFittable;
		hoveredFittable = null;

		foreach (Collider collider in fittable.GetComponentsInChildren<Collider>(includeInactive: true))
		{
			collider.enabled = true;
		}

		Card card = fittable.GetComponent<Card>();
		card?.SetVisible(true);

		int index = fittable.transform.GetSiblingIndex();
		fitter.Remove(index);
		fitter.AdjustPositions();
	}
}
