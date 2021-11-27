using UnityEngine;

public class DropArea : FitterController
{
	public int team;

	private Fittable hoveredFittable;

	public bool OnDrop(Fittable fittable, Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		int count = fitter.GetActiveCount();
		if (HaveSpace(count))
		{
			Fittable newFittable = fitter.Add();

			int index = fitter.CalculateFittableIndex(fitter.GetActiveCount(), position.x);
			newFittable.transform.SetSiblingIndex(index);
			fitter.AdjustPositions();

			Card card = fittable.GetComponent<Card>();
			Card newCard = newFittable.GetComponent<Card>();
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

		fittable.gameObject.SetActive(false);

		int index = fitter.CalculateFittableIndex(fitter.GetActiveCount(), position.x);
		fittable.transform.SetSiblingIndex(index);
		fitter.AdjustPositions();
	}

	public void OnHoverUpdate(Vector3 position)
	{
		if (!hoveredFittable) { return; }
		Fitter fitter = GetComponent<Fitter>();

		int index = fitter.CalculateFittableIndex(fitter.GetActiveCount(), position.x);
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

		fittable.gameObject.SetActive(true);

		int index = fittable.transform.GetSiblingIndex();
		fitter.Remove(index);
		fitter.AdjustPositions();
	}
}
