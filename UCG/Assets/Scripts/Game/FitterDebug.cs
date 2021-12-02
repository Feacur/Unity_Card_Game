using UnityEngine;

public class FitterDebug : FitterController
	, IDragContainer
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

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     IDragContainer
	// ----- ----- ----- ----- -----

	private IFittable _pickedFittable;
	private int _pickedId;

	IDraggable IDragContainer.OnPick(Vector3 position)
	{
		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), position.x);
		IFittable picked = _fitter.Get(index);
		if (picked == null) { return null; }

		_pickedFittable = picked;
		_pickedId = picked.GetGO().transform.GetSiblingIndex() + 1;

		return picked as IDraggable;
	}

	bool IDragContainer.OnDrop(IDraggable draggable, Vector3 position)
	{
		IFittable draggableFittable = draggable as IFittable;
		if (_pickedFittable == draggableFittable)
		{
			_pickedFittable = null;
			_pickedId = 0;
		}
		return false;
	}

	void IDragContainer.OnPickEnd(Vector3 position, bool dropResult)
	{
		if (dropResult && _pickedId > 0)
		{
			_fitter.Remove(_pickedId - 1);
			_fitter.AdjustPositions();
		}
		_pickedFittable = null;
		_pickedId = 0;
	}
}
