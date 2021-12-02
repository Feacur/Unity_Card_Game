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
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

	private void Awake()
	{
		_fitter = GetComponent<Fitter>();
	}

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     IDragContainer
	// ----- ----- ----- ----- -----

	private IFittable _pickedFittable;
	private int _pickedId;

	IDraggable IDragContainer.OnPick(Vector3 position)
	{
		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), position.x);
		IFittable picked = _fitter.Get(index);
		if (picked == null) { return null; }

		_pickedFittable = picked;
		_pickedId = _pickedFittable.GetGO().transform.GetSiblingIndex() + 1;

		picked.GetGO().transform.parent = null;
		// _fitter.AdjustPositions();

		return picked as IDraggable;
	}

	bool IDragContainer.OnDrop(IDraggable draggable, Vector3 position)
	{
		if (_team != draggable.GetTeam()) { return false; }

		IFittable draggableFittable = draggable as IFittable;
		if (draggableFittable == null) { return false; }

		int count = _fitter.GetActiveCount();
		if (!HaveSpace(count)) { return false; }

		int index = _fitter.CalculateFittableIndex(count + 1, position.x);

		if (_pickedFittable != draggableFittable)
		{

			var newFittable = _fitter.Add();
			newFittable.SetTeam(_team);
			newFittable.SetContent(draggableFittable.GetContent());
			newFittable.GetGO().transform.SetSiblingIndex(index);
			
			_pickedFittable = null;
			_pickedId = 0;
		}
		else
		{
			_fitter.EmplaceActive(_pickedFittable, index);
		}

		_fitter.AdjustPositions();

		return true;
	}

	void IDragContainer.OnPickEnd(Vector3 position, bool dropResult)
	{
		if (!dropResult && _pickedFittable != null && _pickedId > 0)
		{
			_fitter.EmplaceActive(_pickedFittable, _pickedId - 1);
			_fitter.AdjustPositions();
		}
		_pickedFittable = null;
		_pickedId = 0;
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
}
