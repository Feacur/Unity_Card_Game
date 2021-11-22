using UnityEngine;

public class FitterDebug : FitterController
{
	[Range(0, CountLimit)] public int targetCount;

	private void UpdateCount()
	{
		Fitter fitter = GetComponent<Fitter>();

		int currentCount = fitter.GetActiveCount();
		targetCount = Mathf.Min(targetCount, targetLimit);

		if (currentCount > targetCount)
		{
			for (int i = currentCount; i < targetCount; i--)
			{
				fitter.Remove(i);
			}
			fitter.AdjustPositions();
		}
		else if (currentCount < targetCount)
		{
			for (int i = currentCount; i < targetCount; i++)
			{
				fitter.Add();
			}
			fitter.AdjustPositions();
		}
	}

	// MonoBehaviour

	private void OnValidate()
	{
		Fitter fitter = GetComponent<Fitter>();
		targetCount = Mathf.Min(targetCount, targetLimit);
	}

	private void Update()
	{
		UpdateCount();
	}

	private void Destroy()
	{
		Fitter fitter = GetComponent<Fitter>();
		fitter.Free();
	}
}
