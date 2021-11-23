using UnityEngine;

public class DropArea : FitterController
{
	public int team;

	public bool OnDrop(Card card, Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		int count = fitter.GetActiveCount();
		if (HaveSpace(count))
		{
			Fittable fittable = fitter.Add();
			fitter.AdjustPositions();

			Card newCard = fittable.GetComponent<Card>();
			if (newCard)
			{
				newCard.SetContent(card.GetContent());
				newCard.team = team;
			}
		}
		return false;
	}
}
