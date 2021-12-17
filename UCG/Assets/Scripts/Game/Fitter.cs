using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitter : MonoBehaviour
{
	[SerializeField] private BoxCollider dimensions;

	[SerializeField] private Fittable elementPrefab;
	[SerializeField] private Transform activeRoot;
	[SerializeField] private Transform pooledRoot;
	[SerializeField] private float _animationSpeed = 10;

	public Vector3 rotation;
	[Range(0, 1)] public float separationFraction = 1;

	// ----- ----- ----- ----- -----
	//     Dimensions
	// ----- ----- ----- ----- -----

	public Vector3 GetSize() => dimensions.size;

	public int CalculateFittableIndex(int count, float positionX)
	{
		if (count <= 1) { return 0; }

		CalculateMetrics(count, out float separation, out float offset);

		float localPositionOffsetX = positionX - offset;
		float elementHalfSizeX = elementPrefab.GetSize().x / 2;
		int index = Mathf.FloorToInt((localPositionOffsetX + elementHalfSizeX) / separation);

		return Mathf.Clamp(index, 0, count - 1);
	}

	public void CalculateMetrics(int count, out float separation, out float offset)
	{
		if (count <= 1) { separation = 0; offset = 0; return; }

		float elementSizeX = elementPrefab.GetSize().x;
		float elementCenterBoundsX = GetSize().x - elementSizeX;
		int spacesCount = count - 1;

		separation = Mathf.Min(elementSizeX * separationFraction, elementCenterBoundsX / spacesCount);
		offset = - separation * spacesCount / 2;
	}

	// ----- ----- ----- ----- -----
	//     Content
	// ----- ----- ----- ----- -----

	public int GetActiveCount() => activeRoot.childCount;
	public int GetPooledCount() => pooledRoot.childCount;

	public void EmplaceActive(IFittable fittable, int index)
	{
		fittable.GetGO().transform.SetParent(activeRoot, worldPositionStays: false);
		fittable.SetPosition(index);
	}

	public void EmplacePooled(IFittable fittable)
	{
		Transform fittableTransform = fittable.GetGO().transform;
		fittableTransform.SetParent(pooledRoot, worldPositionStays: false);
	}

	public IFittable Add()
	{
		foreach (Transform child in pooledRoot)
		{
			child.SetParent(activeRoot, worldPositionStays: false);
			return child.GetComponent<IFittable>();
		}

		return GameObject.Instantiate(elementPrefab, parent: activeRoot, worldPositionStays: false);
	}

	public IFittable Get(int index)
	{
		if (index < 0) { return null; }
		if (index >= activeRoot.childCount) { return null; }

		Transform child = activeRoot.GetChild(index);
		return child.GetComponent<IFittable>();
	}

	public bool Remove(int index)
	{
		if (index < 0) { return false; }
		if (index >= activeRoot.childCount) { return false; }

		Transform child = activeRoot.GetChild(index);
		child.SetParent(pooledRoot, worldPositionStays: false);

		return true;
	}

	public void Reset()
	{
		while (activeRoot.childCount > 0)
		{
			Transform child = activeRoot.GetChild(0);
			child.SetParent(pooledRoot, worldPositionStays: false);
		}
	}

	public void Free()
	{
		if (Application.isPlaying)
		{
			foreach (Transform child in activeRoot)
			{
				GameObject.Destroy(child.gameObject);
			}
			foreach (Transform child in pooledRoot)
			{
				GameObject.Destroy(child.gameObject);
			}
		}
		else
		{
			while (activeRoot.childCount > 0)
			{
				Transform child = activeRoot.GetChild(0);
				GameObject.DestroyImmediate(child.gameObject);
			}
			while (pooledRoot.childCount > 0)
			{
				Transform child = pooledRoot.GetChild(0);
				GameObject.DestroyImmediate(child.gameObject);
			}
		}
	}

	// ----- ----- ----- ----- -----
	//     Animation
	// ----- ----- ----- ----- -----

	private Coroutine _animatePositionsCoroutine;
	private List<Vector3> _animationFrom = new List<Vector3>();

	public void AnimatePositions()
	{
		if (_animatePositionsCoroutine != null)
		{
			StopCoroutine(_animatePositionsCoroutine);
		}
		_animatePositionsCoroutine = StartCoroutine(AnimatePositionsCoroutine());
	}
	
	private IEnumerator AnimatePositionsCoroutine()
	{
		yield return null;

		int count = GetActiveCount();

		_animationFrom.Clear();
		for (int i = 0; i < count; i++)
		{
			Transform childTransform = activeRoot.GetChild(i);
			_animationFrom.Add(childTransform.localPosition);
		}

		CalculateMetrics(count, out float separation, out float offset);

		for (int i = 0; i < count; i++)
		{
			Transform childTransform = activeRoot.GetChild(i);
			childTransform.localRotation = Quaternion.Euler(rotation);
		}

		float animationDuration = 0.25f;
		for (float time = 0; time < animationDuration; time += Time.deltaTime)
		{
			for (int i = 0; i < count; i++)
			{
				Transform childTransform = activeRoot.GetChild(i);
				childTransform.localPosition = Vector3.Lerp(
					_animationFrom[i],
					new Vector3(offset + i * separation, 0, 0),
					Easing.SmoothStep(Mathf.Min(time / animationDuration, 1))
				);
			}
			yield return null;
		}

		_animationFrom.Clear();
		for (int i = 0; i < count; i++)
		{
			Transform childTransform = activeRoot.GetChild(i);
			childTransform.localPosition = new Vector3(offset + i * separation, 0, 0);
		}

		_animatePositionsCoroutine = null;
	}
}
