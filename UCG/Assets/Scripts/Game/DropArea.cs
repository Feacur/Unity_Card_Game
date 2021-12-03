using UnityEngine;

public class DropArea : FitterController
	, IDragContainer
	, IHoverable
{
	[SerializeField] private int _team;
	[SerializeField] private bool _pickable;

	// ----- ----- ----- ----- -----
	//     IGameObject
	// ----- ----- ----- ----- -----

	GameObject IGameObject.GetGO() => gameObject;

	// ----- ----- ----- ----- -----
	//     IDragContainer
	// ----- ----- ----- ----- -----

	private IFittable _pickedFittable;
	private int _pickedId;

	IDraggable IDragContainer.OnPick(GameInputData input)
	{
		if (!_pickable) { return null; }

		int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
		IFittable picked = _fitter.Get(index);
		if (picked == null) { return null; }

		_pickedFittable = picked;
		_pickedId = _pickedFittable.GetGO().transform.GetSiblingIndex() + 1;

		picked.GetGO().transform.parent = null;
		_fitter.AdjustPositions();

		return picked as IDraggable;
	}

	bool IDragContainer.OnDrop(IDraggable draggable, GameInputData input)
	{
		if (draggable == null) { return false; }
		if (_team != draggable.GetTeam()) { return false; }

		IFittable draggableFittable = draggable as IFittable;
		if (draggableFittable == null) { return false; }

		int count = _fitter.GetActiveCount();
		if (!HaveSpace(count)) { return false; }

		int index = _fitter.CalculateFittableIndex(count + 1, input.target.x);

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

	void IDragContainer.OnPickEnd(GameInputData input, bool dropResult)
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

	void IHoverable.OnEnter(IDraggable draggable, GameInputData input)
	{
		if (draggable != null && _team == draggable.GetTeam())
		{
			if (HaveSpace(_fitter.GetActiveCount()))
			{
				hoverablePlaceholder = _fitter.Add();

				hoverablePlaceholder.GetGO().SetActive(false);
				// (placeholder as IInteractable)?.SetState(false);

				int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
				hoverablePlaceholder.GetGO().transform.SetSiblingIndex(index);
				_fitter.AdjustPositions();
			}
		}
	}

	void IHoverable.OnUpdate(IDraggable draggable, GameInputData input)
	{
		if (hoverablePlaceholder != null)
		{
			int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
			hoverablePlaceholder.GetGO().transform.SetSiblingIndex(index);
			_fitter.AdjustPositions();
		}
	}

	void IHoverable.OnExit(IDraggable draggable, GameInputData input)
	{
		IFittable hoverablePlaceholder = this.hoverablePlaceholder;
		this.hoverablePlaceholder = null;

		if (hoverablePlaceholder != null)
		{
			hoverablePlaceholder.GetGO().SetActive(true);
			// (placeholder as IInteractable)?.SetState(true);

			int index = hoverablePlaceholder.GetGO().transform.GetSiblingIndex();
			_fitter.Remove(index);
			_fitter.AdjustPositions();
		}
	}
}
