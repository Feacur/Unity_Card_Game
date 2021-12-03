using UnityEngine;

public class FitterDebug : FitterController
	, IDragContainer
{
	[SerializeField] private int _team;
	[SerializeField] private bool _pickable;
	[SerializeField, Range(0, CountLimit)] private int _inputCount;
	[SerializeField] private bool _persistent;

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

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		base.OnValidate();
		if (_inputCount > _targetLimit) { _inputCount = _targetLimit; }
	}
#endif

	private void Start()
	{
		SetCount(CountLimit);

		for (int i = 0; i < CountLimit; i++)
		{
			IFittable fittable = _fitter.Get(i);
			fittable.SetTeam(_team);
			fittable.SetContent((i + 1).ToString());
		}

		_inputCount = Mathf.Min(_inputCount, _targetLimit);
		SetCount(_inputCount);

		this.enabled = _persistent;
	}

	private void Update()
	{
		_inputCount = Mathf.Min(_inputCount, _targetLimit);
		SetCount(_inputCount);
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
		if (!_pickable) { return null; }

		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), position.x);
		IFittable picked = _fitter.Get(index);
		if (picked == null) { return null; }

		_pickedFittable = picked;
		_pickedId = picked.GetGO().transform.GetSiblingIndex() + 1;

		return picked as IDraggable;
	}

	bool IDragContainer.OnDrop(IDraggable draggable, Vector3 position)
	{
		if (draggable == null) { return false; }
		if (_team != draggable.GetTeam()) { return false; }

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
