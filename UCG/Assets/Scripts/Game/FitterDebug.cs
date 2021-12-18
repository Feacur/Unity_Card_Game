using UnityEngine;

public class FitterDebug : FitterController
	, IDragContainer
	, IHoverable
{
	[SerializeField] private int _team;
	[SerializeField] private bool _pickable;
	[SerializeField] private bool _previewable;
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
			_fitter.AnimatePositions();
		}
		else if (currentCount < targetCount)
		{
			for (int i = currentCount; i < targetCount; i++)
			{
				_fitter.Add();
			}
			_fitter.AnimatePositions();
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

	IDraggable IDragContainer.OnPick(GameInputData input)
	{
		if (!_pickable) { return null; }

		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
		IFittable picked = _fitter.Get(index);
		if (picked == null) { return null; }
		if (!(picked is IDraggable)) { return null; }

		_pickedFittable = picked;
		_pickedId = picked.GetPosition() + 1;

		picked.GetGO().transform.SetParent(null, worldPositionStays: true);

		return picked as IDraggable;
	}

	bool IDragContainer.OnDrop(IDraggable draggable, GameInputData input)
	{
		if (draggable == null) { return false; }
		if (_team != draggable.GetTeam()) { return false; }

		IFittable draggableFittable = draggable as IFittable;
		if (draggableFittable == null) { return false; }

		return false;
	}

	void IDragContainer.OnPickEnd(GameInputData input, bool dropResult)
	{
		if (!dropResult && _pickedFittable != null && _pickedId > 0)
		{
			_fitter.EmplaceActive(_pickedFittable, _pickedId - 1);
		}
		_pickedFittable = null;
		_pickedId = 0;

		_fitter.AnimatePositions();
	}

	// ----- ----- ----- ----- -----
	//     IHoverable
	// ----- ----- ----- ----- -----

	private IFittable hoveredFittable;

	void IHoverable.OnEnter(IDraggable draggable, GameInputData input)
	{
		if (_previewable && draggable == null)
		{
			int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
			hoveredFittable = _fitter.Get(index);

			hoveredFittable?.Show(input);
		}
	}

	void IHoverable.OnUpdate(IDraggable draggable, GameInputData input)
	{
		if (_previewable && draggable == null)
		{
			hoveredFittable?.Hide(input);

			int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
			hoveredFittable = _fitter.Get(index);

			hoveredFittable?.Show(input);

		}
	}

	void IHoverable.OnExit(IDraggable draggable, GameInputData input)
	{
		if (_previewable)
		{
			IFittable hoveredFittable = this.hoveredFittable;
			this.hoveredFittable = null;

			hoveredFittable?.Hide(input);
		}
	}
}
