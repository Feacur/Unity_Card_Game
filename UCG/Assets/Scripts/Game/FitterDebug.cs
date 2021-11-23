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
			for (int i = currentCount; i <= targetCount; i--)
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

	private void OnValidate()
	{
		Fitter fitter = GetComponent<Fitter>();
		inputCount = Mathf.Min(inputCount, targetLimit);
	}

	private void Start()
	{
		SetCount(CountLimit);

		Fitter fitter = GetComponent<Fitter>();
		for (int i = 0; i < CountLimit; i++)
		{
			Fittable fittable = fitter.Get(i);
			fittable.name = $"Fittable {(i + 1)}";

			Card card = fittable.GetComponent<Card>();
			if (card)
			{
				card.team = team;
				card.SetContent((i + 1).ToString());
			}
		}

		SetCount(inputCount);

		this.enabled = persistent;
	}

	private void Update()
	{
		SetCount(Mathf.Min(inputCount, targetLimit));
	}

	private void Destroy()
	{
		Fitter fitter = GetComponent<Fitter>();
		fitter.Free();
	}
}
