using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Fitter))]
public abstract class FitterController : MonoBehaviour
{
	protected const int CountLimit = 10;

	[SerializeField, Range(0, CountLimit)] protected int _targetLimit;

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
}
