using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitter : MonoBehaviour
{
	[SerializeField] private BoxCollider _dimensions;

	[SerializeField] private Fittable _elementPrefab;
	[SerializeField] private Transform _activeRoot;
	[SerializeField] private Transform _pooledRoot;
	[SerializeField] private float _animationDuration = 0.25f;

	public Vector3 rotation;
	[Range(0, 2)] public float _separationFraction = 1;

	// ----- ----- ----- ----- -----
	//     Dimensions
	// ----- ----- ----- ----- -----

	public Vector3 GetSize() => _dimensions.size;

	public int CalculateFittableIndex(int count, float positionX)
	{
		if (count == 0) { return -1; }

		float elementHalfSizeX = _elementPrefab.GetSize().x / 2;
		if (count == 1)
		{
			if (positionX < -elementHalfSizeX) { return -1; }
			if (positionX >  elementHalfSizeX) { return  1; }
			return 0;
		}

		CalculateMetrics(count, out float separation, out float offset);
		return Mathf.FloorToInt((positionX - offset + elementHalfSizeX) / separation);
	}

	public void CalculateMetrics(int count, out float separation, out float offset)
	{
		if (count <= 1) { separation = 0; offset = 0; return; }

		float elementSizeX = _elementPrefab.GetSize().x;
		float elementCenterBoundsX = GetSize().x - elementSizeX;
		int spacesCount = count - 1;

		separation = Mathf.Min(elementSizeX * _separationFraction, elementCenterBoundsX / spacesCount);
		offset = - separation * spacesCount / 2;
	}

	// ----- ----- ----- ----- -----
	//     Content
	// ----- ----- ----- ----- -----

	public int GetActiveCount() => _activeRoot.childCount;
	public int GetPooledCount() => _pooledRoot.childCount;

	public void EmplaceActive(IFittable fittable, int index)
	{
		fittable.GetGO().transform.SetParent(_activeRoot, worldPositionStays: true);
		fittable.SetPosition(index);
	}

	public void EmplacePooled(IFittable fittable)
	{
		Transform fittableTransform = fittable.GetGO().transform;
		fittableTransform.SetParent(_pooledRoot, worldPositionStays: false);
	}

	public IFittable Add()
	{
		foreach (Transform child in _pooledRoot)
		{
			child.SetParent(_activeRoot, worldPositionStays: false);
			return child.GetComponent<IFittable>();
		}

		return GameObject.Instantiate(_elementPrefab, parent: _activeRoot, worldPositionStays: false);
	}

	public IFittable Get(int index)
	{
		if (index < 0) { return null; }
		if (index >= _activeRoot.childCount) { return null; }

		Transform child = _activeRoot.GetChild(index);
		return child.GetComponent<IFittable>();
	}

	public bool Remove(int index)
	{
		if (index < 0) { return false; }
		if (index >= _activeRoot.childCount) { return false; }

		Transform child = _activeRoot.GetChild(index);
		child.SetParent(_pooledRoot, worldPositionStays: false);

		return true;
	}

	public void Reset()
	{
		while (_activeRoot.childCount > 0)
		{
			Transform child = _activeRoot.GetChild(0);
			child.SetParent(_pooledRoot, worldPositionStays: false);
		}
	}

	public void Free()
	{
		if (Application.isPlaying)
		{
			foreach (Transform child in _activeRoot)
			{
				GameObject.Destroy(child.gameObject);
			}
			foreach (Transform child in _pooledRoot)
			{
				GameObject.Destroy(child.gameObject);
			}
		}
		else
		{
			while (_activeRoot.childCount > 0)
			{
				Transform child = _activeRoot.GetChild(0);
				GameObject.DestroyImmediate(child.gameObject);
			}
			while (_pooledRoot.childCount > 0)
			{
				Transform child = _pooledRoot.GetChild(0);
				GameObject.DestroyImmediate(child.gameObject);
			}
		}
	}

	// ----- ----- ----- ----- -----
	//     Animation
	// ----- ----- ----- ----- -----

	private Coroutine _animationCoroutine;
	private List<(Transform transform, Vector3 from, Vector3 to)> _animation = new List<(Transform, Vector3, Vector3)>();

	public void Animate()
	{
		if (_animationCoroutine != null)
		{
			StopCoroutine(_animationCoroutine);
		}
		_animationCoroutine = StartCoroutine(AnimationCoroutine());
	}
	
	private IEnumerator AnimationCoroutine()
	{
		yield return null;

		int initialCount = GetActiveCount();
		CalculateMetrics(initialCount, out float separation, out float offset);

		_animation.Clear();
		for (int i = 0; i < initialCount; i++)
		{
			Transform childTransform = _activeRoot.GetChild(i);
			_animation.Add((
				childTransform,
				childTransform.localPosition,
				new Vector3(offset + i * separation, 0, 0)
			));
		}

		for (int i = 0; i < _animation.Count; i++)
		{
			Transform childTransform = _animation[i].transform;
			childTransform.localRotation = Quaternion.Euler(rotation);
		}

		for (float time = 0; time < _animationDuration; time += Time.deltaTime)
		{
			foreach (var (childTransform, from, to) in _animation)
			{
				childTransform.localPosition = Vector3.Lerp(
					from, to,
					Easing.SmoothStep(Mathf.Min(time / _animationDuration, 1))
				);
			}
			yield return null;
		}

		foreach (var (childTransform, from, to) in _animation)
		{
			childTransform.localPosition = to;
		}
		_animation.Clear();

		_animationCoroutine = null;
	}
}
