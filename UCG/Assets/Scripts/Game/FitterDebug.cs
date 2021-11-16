using UnityEngine;

[RequireComponent(typeof(Fitter))]
public class FitterDebug : MonoBehaviour
{
	private const int CountLimit = 10;

	[Range(0, CountLimit)] public int targetLimit;
	[Range(0, CountLimit)] public int targetCount;

	private int currentCount;

	private void UpdateCount()
	{
		if (currentCount == targetCount) { return; }
		targetCount = Mathf.Min(targetCount, targetLimit);
		currentCount = targetCount;

		Fitter fitter = GetComponent<Fitter>();
		fitter.SetCount(targetCount);
	}

	// MonoBehaviour

	private void OnValidate()
	{
		targetCount = Mathf.Min(targetCount, targetLimit);
	}

	private void Start()
	{
		Fitter fitter = GetComponent<Fitter>();
		fitter.Init(CountLimit);
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
