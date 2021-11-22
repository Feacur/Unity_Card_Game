using UnityEngine;

public class DropArea : FitterController
{
	public bool OnDrop(Card card, Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		int count = fitter.GetActiveCount();
		if (HaveSpace(count))
		{
			Fittable fittable = fitter.Add();
			fitter.AdjustPositions();
		}
		return false;
	}
}
