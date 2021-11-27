using UnityEngine;

public class DropArea : FitterController
	, IHoverable
{
	public int team;

	private IFittable hoveredFittable;

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     IHoverable
	// ----- ----- ----- ----- -----

	void IHoverable.OnEnter(Vector3 position)
	{
		Fitter fitter = GetComponent<Fitter>();
		if (!HaveSpace(fitter.GetActiveCount())) { return; }

		IFittable fittable = fitter.Add();
		hoveredFittable = fittable;

		foreach (Collider collider in fittable.GetGO().GetComponentsInChildren<Collider>(includeInactive: true))
		{
			collider.enabled = false;
		}

		fittable.GetGO().SetActive(false);

		int index = fitter.CalculateFittableIndex(fitter.GetActiveCount(), position.x);
		fittable.GetGO().transform.SetSiblingIndex(index);
		fitter.AdjustPositions();
	}

	void IHoverable.OnUpdate(Vector3 position)
	{
		if (hoveredFittable == null) { return; }
		Fitter fitter = GetComponent<Fitter>();

		int index = fitter.CalculateFittableIndex(fitter.GetActiveCount(), position.x);
		hoveredFittable.GetGO().transform.SetSiblingIndex(index);
		fitter.AdjustPositions();
	}

	void IHoverable.OnExit(Vector3 position)
	{
		if (hoveredFittable == null) { return; }
		Fitter fitter = GetComponent<Fitter>();

		IFittable fittable = hoveredFittable;
		hoveredFittable = null;

		foreach (Collider collider in fittable.GetGO().GetComponentsInChildren<Collider>(includeInactive: true))
		{
			collider.enabled = true;
		}

		fittable.GetGO().SetActive(true);

		int index = fittable.GetGO().transform.GetSiblingIndex();
		fitter.Remove(index);
		fitter.AdjustPositions();
	}

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	public bool OnDrop(IDraggable draggable, Vector3 position)
	{
		if (team != draggable.GetTeam()) { return false; }

		IFittable draggableFittable = draggable.GetGO().GetComponent<IFittable>();
		if (draggableFittable == null) { return false; }

		Fitter fitter = GetComponent<Fitter>();
		int count = fitter.GetActiveCount();
		if (HaveSpace(count))
		{

			IFittable newElement = fitter.Add();
			newElement.SetTeam(team);
			newElement.SetContent(draggableFittable.GetContent());

			int index = fitter.CalculateFittableIndex(fitter.GetActiveCount(), position.x);
			newElement.GetGO().transform.SetSiblingIndex(index);
			fitter.AdjustPositions();

			return true;
		}
		return false;
	}
}
