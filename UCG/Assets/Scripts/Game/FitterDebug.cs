using UnityEngine;

public class FitterDebug : FitterController
{
	public int team;
	[Range(0, CountLimit)] public int inputCount;
	public bool persistent;

	private void SetCount(int targetCount)
	{
		Fitter fitter = GetComponent<Fitter>();

		int currentCount = fitter.GetActiveCount();

		if (currentCount > targetCount)
		{
			for (int i = currentCount; i > targetCount; i--)
			{
				fitter.Remove(i - 1);
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

	private void Start()
	{
		SetCount(CountLimit);

		Fitter fitter = GetComponent<Fitter>();
		for (int i = 0; i < CountLimit; i++)
		{
			IFittable fittable = fitter.Get(i);
			fittable.SetTeam(team);
			fittable.SetContent((i + 1).ToString());
		}

		inputCount = Mathf.Min(inputCount, targetLimit);
		SetCount(inputCount);

		this.enabled = persistent;
	}

	private void Update()
	{
		inputCount = Mathf.Min(inputCount, targetLimit);
		SetCount(inputCount);
	}

	private void Destroy()
	{
		Fitter fitter = GetComponent<Fitter>();
		fitter.Free();
	}
}
