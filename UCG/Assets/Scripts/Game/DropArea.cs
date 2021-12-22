using UnityEngine;

public class DropArea : FitterController
	, IDragTarget
	, IHoverable
{
	[SerializeField] private int _team;
	[SerializeField] private bool _isDragTarget;

	// ----- ----- ----- ----- -----
	//     IComponent
	// ----- ----- ----- ----- -----

	GameObject IComponent.GetGO() => gameObject;
	T IComponent.GetComponent<T>() => GetComponent<T>();

	// ----- ----- ----- ----- -----
	//     IDropContainer
	// ----- ----- ----- ----- -----

	bool IDragTarget.OnDrop(IDraggable draggable, GameInputData input)
	{
		if (!_isDragTarget) { return false; }

		if (draggable == null) { return false; }
		if (_team != draggable.GetTeam()) { return false; }

		// IFittable draggableFittable = draggable.GetComponent<IFittable>();
		IFittable draggableFittable = draggable as IFittable;
		if (draggableFittable == null) { return false; }

		int count = _fitter.GetActiveCount();
		if (!HaveSpace(count)) { return false; }

		int index = _fitter.CalculateFittableIndex(count + 1, input.target.x);
		index = Mathf.Clamp(index, 0, count);

		_fitter.EmplaceActive(draggableFittable, index);
		Animate();

		return true;
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

				int count = _fitter.GetActiveCount();
				int index = _fitter.CalculateFittableIndex(count, input.target.x);
				index = Mathf.Clamp(index, 0, count - 1);

				_hoveredPlaceholder.SetPosition(index);
				Animate();
			}
		}
	}

	void IHoverable.OnUpdate(IDraggable draggable, GameInputData input)
	{
		if (_hoveredPlaceholder != null)
		{
			int count = _fitter.GetActiveCount();
			int index = _fitter.CalculateFittableIndex(count, input.target.x);
			index = Mathf.Clamp(index, 0, count - 1);

			if (_hoveredPlaceholder.GetPosition() != index)
			{
				_hoveredPlaceholder.SetPosition(index);
				Animate();
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

			int index = hoverablePlaceholder.GetPosition();
			_fitter.Remove(index);
			Animate();
		}
	}
}
