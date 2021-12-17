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
		if (picked !is IDraggable) { return null; }

		_pickedFittable = picked;
		_pickedId = _pickedFittable.GetPosition() + 1;

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
			newFittable.SetPosition(index);
			
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

	private IFittable _hoveredPlaceholder;

	void IHoverable.OnEnter(IDraggable draggable, GameInputData input)
	{
		if (draggable != null && _team == draggable.GetTeam())
		{
			if (HaveSpace(_fitter.GetActiveCount()))
			{
				_hoveredPlaceholder = _fitter.Add();

				_hoveredPlaceholder.GetGO().SetActive(false);
				// (placeholder as IInteractable)?.SetState(false);

				int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
				_hoveredPlaceholder.SetPosition(index);
				_fitter.AdjustPositions();
			}
		}
	}

	void IHoverable.OnUpdate(IDraggable draggable, GameInputData input)
	{
		if (_hoveredPlaceholder != null)
		{
			int index = _fitter.CalculateFittableIndex(_fitter.GetActiveCount(), input.target.x);
			if (_hoveredPlaceholder.GetPosition() != index)
			{
				_hoveredPlaceholder.SetPosition(index);
				_fitter.AdjustPositions();
			}
		}
	}

	void IHoverable.OnExit(IDraggable draggable, GameInputData input)
	{
		IFittable hoverablePlaceholder = this._hoveredPlaceholder;
		this._hoveredPlaceholder = null;

		if (hoverablePlaceholder != null)
		{
			hoverablePlaceholder.GetGO().SetActive(true);
			// (placeholder as IInteractable)?.SetState(true);

			int index = hoverablePlaceholder.GetPosition();
			_fitter.Remove(index);
			_fitter.AdjustPositions();
		}
	}
}
