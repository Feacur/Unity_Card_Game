using UnityEngine;

public class FitterDebug : FitterController
	, IHoverable
{
	[SerializeField] private int _team;
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
			_fitter.Animate();
		}
		else if (currentCount < targetCount)
		{
			for (int i = currentCount; i < targetCount; i++)
			{
				_fitter.Add();
			}
			_fitter.Animate();
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
			IContent fittableContent = fittable as IContent;
			fittableContent.Set(_team, (i + 1).ToString());
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
	//     IComponent
	// ----- ----- ----- ----- -----

	GameObject IComponent.GetGO() => gameObject;
	T IComponent.GetComponent<T>() => GetComponent<T>();

	// ----- ----- ----- ----- -----
	//     IHoverable
	// ----- ----- ----- ----- -----

	private IFittable _hoveredFittable;

	void IHoverable.OnEnter(IDraggable draggable, GameInputData input)
	{
		if (_previewable && draggable == null)
		{
			int count = _fitter.GetActiveCount();
			int index = _fitter.CalculateFittableIndex(count, input.target.x);
			index = Mathf.Clamp(index, 0, count - 1);

			IFittable fittable = _fitter.Get(index);

			_hoveredFittable = fittable;
			_hoveredFittable?.Show(input);
		}
	}

	void IHoverable.OnUpdate(IDraggable draggable, GameInputData input)
	{
		if (_previewable && draggable == null)
		{
			int count = _fitter.GetActiveCount();
			int index = _fitter.CalculateFittableIndex(count, input.target.x);
			index = Mathf.Clamp(index, 0, count - 1);

			IFittable fittable = _fitter.Get(index);
			if (fittable != _hoveredFittable)
			{
				_hoveredFittable?.Hide(input);
				_hoveredFittable = fittable;
				_hoveredFittable?.Show(input);
			}
		}
	}

	void IHoverable.OnExit(IDraggable draggable, GameInputData input)
	{
		if (_previewable)
		{
			IFittable hoveredFittable = this._hoveredFittable;
			this._hoveredFittable = null;

			hoveredFittable?.Hide(input);
		}
	}
}
