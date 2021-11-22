using UnityEngine;

public class DropArea : FitterController
{
	public bool OnDrop(Card card, Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		int count = fitter.GetCount();
		if (HaveSpace(count))
		{
			fitter.SetCount(count + 1);
		}
		return false;
	}

	// MonoBehaviour

	private void Start()
	{
		Fitter fitter = GetComponent<Fitter>();
		fitter.Init(CountLimit);
	}
}
