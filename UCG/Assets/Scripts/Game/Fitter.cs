using UnityEngine;

public class Fitter : MonoBehaviour
{
	public Fittable elementPrefab;
	public Transform elementsRoot;
	public BoxCollider dimensions;

	public Vector3 rotation;
	[Range(0, 1)] public float separationFraction = 1;

	public void Init(int count)
	{
		if (elementsRoot.childCount == count) { return; }
		CalculateDimensions(count, out float separation, out float offset);
		for (int i = elementsRoot.childCount; i < count; i++)
		{
			Fittable instance = GameObject.Instantiate(elementPrefab, parent: elementsRoot, worldPositionStays: false);

			Card card = instance.GetComponent<Card>();
			if (card)
			{
				card.SetContent(i.ToString());
			}
		}
		foreach (Transform child in elementsRoot)
		{
			child.gameObject.SetActive(false);
		}
		for (int i = 0; i < elementsRoot.childCount; i++)
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

	public int GetCount()
	{
		int count = 0;
		foreach (Transform child in elementsRoot)
		{
			count += child.gameObject.activeSelf ? 1 : 0;
		}
		return count;
	}

	public void SetCount(int count)
	{
		// if (cardsRoot.childCount != CardsLimit) { return; }
		CalculateDimensions(count, out float separation, out float offset);
		for (int i = 0; i < elementsRoot.childCount; i++)
		{
			Transform childTransform = elementsRoot.GetChild(i);
			childTransform.localPosition = new Vector3(offset + i * separation, 0, 0);
			childTransform.gameObject.SetActive(i < count);
		}
	}

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
