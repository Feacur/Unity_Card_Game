using UnityEngine;

public class DropArea : FitterController
	, IDragContainer
	, IHoverable
{
	[SerializeField] private int _team;

	// ----- ----- ----- ----- -----
	//     Dependencies
	// ----- ----- ----- ----- -----

	private Fitter _fitter;

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     IDragContainer
	// ----- ----- ----- ----- -----

	IDraggable IDragContainer.OnPick(Ray ray)
	{
		if (!Physics.Raycast(ray, out RaycastHit hit)) { return null; }
		return hit.transform.GetComponent<IDraggable>();
	}

	bool IDragContainer.OnDrop(IDraggable draggable, Vector3 position)
	{
		if (_team != draggable.GetTeam()) { return false; }

		IFittable draggableFittable = draggable.GetGO().GetComponent<IFittable>();
		if (draggableFittable == null) { return false; }

		int count = _fitter.GetActiveCount();
		if (HaveSpace(count))
		{

			IFittable newElement = _fitter.Add();
			newElement.SetTeam(_team);
			newElement.SetContent(draggableFittable.GetContent());

			int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), position.x);
			newElement.GetGO().transform.SetSiblingIndex(index);
			_fitter.AdjustPositions();

			return true;
		}
		return false;
	}

	// ----- ----- ----- ----- -----
	//     IHoverable
	// ----- ----- ----- ----- -----

	private IFittable hoverablePlaceholder;

	void IHoverable.OnEnter(IDraggable draggable, Vector3 position)
	{
		if (draggable != null)
		{
			if (HaveSpace(_fitter.GetActiveCount()))
			{
				hoverablePlaceholder = _fitter.Add();

				hoverablePlaceholder.GetGO().SetActive(false);
				// (placeholder as IInteractable)?.SetState(false);

				int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), position.x);
				hoverablePlaceholder.GetGO().transform.SetSiblingIndex(index);
				_fitter.AdjustPositions();
			}
		}
	}

	void IHoverable.OnUpdate(IDraggable draggable, Vector3 position)
	{
		if (hoverablePlaceholder != null)
		{
			int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), position.x);
			hoverablePlaceholder.GetGO().transform.SetSiblingIndex(index);
			_fitter.AdjustPositions();
		}
	}

	void IHoverable.OnExit(IDraggable draggable, Vector3 position)
	{
		IFittable placeholder = this.hoverablePlaceholder;
		this.hoverablePlaceholder = null;

		if (placeholder != null)
		{
			placeholder.GetGO().SetActive(true);
			// (placeholder as IInteractable)?.SetState(true);

			int index = placeholder.GetGO().transform.GetSiblingIndex();
			_fitter.Remove(index);
			_fitter.AdjustPositions();
		}
	}

	// ----- ----- ----- ----- -----
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

	private void Awake()
	{
		_fitter = GetComponent<Fitter>();
	}
}
