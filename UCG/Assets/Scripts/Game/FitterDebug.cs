using UnityEngine;

public class FitterDebug : FitterController
{
	public int team;
	[Range(0, CountLimit)] public int inputCount;
	public bool persistent;

	// ----- ----- ----- ----- -----
	//     Dependencies
	// ----- ----- ----- ----- -----

	private Fitter _fitter;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	private void SetCount(int targetCount)
	{
		int currentCount = _fitter.GetActiveCount();

		if (currentCount > targetCount)
		{
			for (int i = currentCount; i > targetCount; i--)
			{
				_fitter.Remove(i - 1);
			}
			_fitter.AdjustPositions();
		}
		else if (currentCount < targetCount)
		{
			for (int i = currentCount; i < targetCount; i++)
			{
				_fitter.Add();
			}
			_fitter.AdjustPositions();
		}
	}

	// ----- ----- ----- ----- -----
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

	private void Awake()
	{
		_fitter = GetComponent<Fitter>();
	}

	private void Start()
	{
		SetCount(CountLimit);

		for (int i = 0; i < CountLimit; i++)
		{
			IFittable fittable = _fitter.Get(i);
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
		_fitter.Free();
	}
}
