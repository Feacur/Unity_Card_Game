using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Fitter))]
public abstract class FitterController : MonoBehaviour
	, IDragSource
{
	protected const int CountLimit = 10;
	protected const float AnimationDuration = 0.25f;

	[SerializeField, Range(0, CountLimit)] protected int _targetLimit;
	[SerializeField] private bool _isDragSource;

	// ----- ----- ----- ----- -----
	//     Dependencies (local)
	// ----- ----- ----- ----- -----

	protected Fitter _fitter;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	protected bool HaveSpace(int count) => _targetLimit == 0 || count < _targetLimit;

	protected void Animate() => _fitter.Animate(duration: AnimationDuration);

	// ----- ----- ----- ----- -----
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		if (_targetLimit > CountLimit) { _targetLimit = CountLimit; }
	}
#endif

	protected virtual void Awake()
	{
		_fitter = GetComponent<Fitter>();
	}

	// ----- ----- ----- ----- -----
	//     IComponent
	// ----- ----- ----- ----- -----

	GameObject IComponent.GetGO() => gameObject;
	T IComponent.GetComponent<T>() => GetComponent<T>();

	// ----- ----- ----- ----- -----
	//     IPickContainer
	// ----- ----- ----- ----- -----

	private IFittable _pickedFittable;
	private int _pickedId;

	IDraggable IDragSource.OnPick(GameInputData input)
	{
		if (!_isDragSource) { return null; }

		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
		IFittable pickedFittable = _fitter.Get(index);
		if (pickedFittable == null) { return null; }

		// IDraggable pickedDraggable = pickedFittable.GetComponent<IDraggable>();
		IDraggable pickedDraggable = pickedFittable as IDraggable;
		if (pickedDraggable == null) { return null; }

		_pickedFittable = pickedFittable;
		_pickedId = _pickedFittable.GetPosition() + 1;

		_fitter.SkipAnimation(pickedFittable);
		pickedFittable.GetGO().transform.SetParent(null, worldPositionStays: true);

		return pickedDraggable;
	}

	void IDragSource.OnDrop(GameInputData input, bool dropResult)
	{
		if (!dropResult && _pickedFittable != null && _pickedId > 0)
		{
			_fitter.EmplaceActive(_pickedFittable, _pickedId - 1);
		}
		_pickedFittable = null;
		_pickedId = 0;

		Animate();
	}
}
