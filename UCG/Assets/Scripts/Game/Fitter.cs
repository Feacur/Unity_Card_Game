using System.Collections.Generic;
using UnityEngine;

public class Fitter : MonoBehaviour
{
	[SerializeField] private BoxCollider dimensions;

	[SerializeField] private Fittable elementPrefab;
	[SerializeField] private Transform activeRoot;
	[SerializeField] private Transform pooledRoot;

	public Vector3 rotation;
	[Range(0, 1)] public float separationFraction = 1;
	public bool yankOnSelect;

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

	public void EmplaceActive(Fittable fittable, int index)
	{
		fittable.transform.SetParent(activeRoot, worldPositionStays: false);
		fittable.transform.SetSiblingIndex(index);
	}

	public void EmplacePooled(Fittable fittable)
	{
		fittable.transform.SetParent(pooledRoot, worldPositionStays: false);
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

	public void AdjustPositions()
	{
		int count = GetActiveCount();
		CalculateMetrics(count, out float separation, out float offset);
		for (int i = 0; i < count; i++)
		{
			Transform childTransform = activeRoot.GetChild(i);
			childTransform.localPosition = new Vector3(offset + i * separation, 0, 0);
			childTransform.localRotation = Quaternion.Euler(rotation);
		}
	}

	// ----- ----- ----- ----- -----
	//     Animation
	// ----- ----- ----- ----- -----

	private List<IInteractable> interactables = new List<IInteractable>();
	public void SetElementsInteractable(bool state)
	{
		activeRoot.GetComponentsInChildren<IInteractable>(includeInactive: true, result: interactables);
		foreach (IInteractable interactable in interactables)
		{
			interactable.SetState(state);
		}
		interactables.Clear();
	}
}
