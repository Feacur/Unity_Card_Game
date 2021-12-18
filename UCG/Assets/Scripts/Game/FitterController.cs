using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Fitter))]
public abstract class FitterController : MonoBehaviour
	, IDragSource
{
	protected const int CountLimit = 10;

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
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     IPickContainer
	// ----- ----- ----- ----- -----

	private IFittable _pickedFittable;
	private int _pickedId;

	IDraggable IDragSource.OnPick(GameInputData input)
	{
		if (!_isDragSource) { return null; }

		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
		IFittable picked = _fitter.Get(index);
		if (picked == null) { return null; }
		if (!(picked is IDraggable)) { return null; }

		_pickedFittable = picked;
		_pickedId = _pickedFittable.GetPosition() + 1;

		picked.GetGO().transform.SetParent(null, worldPositionStays: true);

		return picked as IDraggable;
	}

	void IDragSource.OnDrop(GameInputData input, bool dropResult)
	{
		if (!dropResult && _pickedFittable != null && _pickedId > 0)
		{
			_fitter.EmplaceActive(_pickedFittable, _pickedId - 1);
		}
		_pickedFittable = null;
		_pickedId = 0;

		_fitter.Animate();
	}
}
