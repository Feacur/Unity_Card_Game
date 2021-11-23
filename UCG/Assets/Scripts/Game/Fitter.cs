using UnityEngine;

public class Fitter : MonoBehaviour
{
	public Fittable elementPrefab;
	public Transform elementsRoot;
	public BoxCollider dimensions;

	public Vector3 rotation;
	[Range(0, 1)] public float separationFraction = 1;

	public Fittable Add()
	{
		foreach (Transform child in elementsRoot)
		{
			if (child.gameObject.activeSelf) { continue; }
			Fittable fittable = child.GetComponent<Fittable>();
			fittable.gameObject.SetActive(true);
			return fittable;
		}

		Fittable instance = GameObject.Instantiate(elementPrefab, parent: elementsRoot, worldPositionStays: false);
		instance.gameObject.SetActive(true);
		return instance;
	}

	public Fittable Get(int index)
	{
		if (index < 0) { return null; }
		if (index >= elementsRoot.childCount) { return null; }

		Transform child = elementsRoot.GetChild(index);
		return child.GetComponent<Fittable>();
	}

	public bool Remove(int index)
	{
		if (index < 0) { return false; }
		if (index >= elementsRoot.childCount) { return false; }

		Transform child = elementsRoot.GetChild(index);
		child.gameObject.SetActive(false);
		child.SetAsLastSibling();

		return true;
	}

	public void AdjustPositions()
	{
		int count = GetActiveCount();
		CalculateDimensions(count, out float separation, out float offset);
		for (int i = 0; i < count; i++)
		{
			Transform childTransform = elementsRoot.GetChild(i);
			childTransform.localPosition = new Vector3(offset + i * separation, 0, 0);
			childTransform.localRotation = Quaternion.Euler(rotation);
		}
	}

	public void Free()
	{
		if (Application.isPlaying)
		{
			foreach (Transform child in elementsRoot)
			{
				GameObject.Destroy(child.gameObject);
			}
		}
		else
		{
			while (elementsRoot.childCount > 0)
			{
				GameObject.DestroyImmediate(elementsRoot.GetChild(0).gameObject);
			}
		}
	}

	public int GetActiveCount()
	{
		int count = 0;
		foreach (Transform child in elementsRoot)
		{
			count += child.gameObject.activeSelf ? 1 : 0;
		}
		return count;
	}

	public int GetPoolSize() => elementsRoot.childCount;

	private void CalculateDimensions(int count, out float separation, out float offset)
	{
		if (count > 1)
		{
			float elementSizeX = elementPrefab.dimensions.size.x;
			float extentsX = dimensions.size.x - elementSizeX;

			int spacesCount = count - 1;
			separation = Mathf.Min(elementSizeX * separationFraction, extentsX / spacesCount);
			offset = - separation * spacesCount / 2;
			return;
		}

		separation = 0;
		offset = 0;
	}
}
