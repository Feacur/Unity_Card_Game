using UnityEngine;

public class InputController : MonoBehaviour
{
	private const float DragThreshold = 50;

	// ----- ----- ----- ----- -----
	//     Dependencies (external)
	// ----- ----- ----- ----- -----

	private InputTracker _inputTracker;
	[SerializeField] private Camera _camera;

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	[System.Serializable]
	private struct State
	{
		public IHoverable hoverable;
		public IDragSource dragSource;
		public IDraggable draggable;
		public PlatformInput.ActionType pickAction;
	};
	private State _state;

	private void UpdateHover(GameObject hovered, GameInputData input)
	{
		IHoverable currentHoverable = hovered?.GetComponent<IHoverable>();

		if (_state.hoverable != null)
		{
			if (ReferenceEquals(currentHoverable, _state.hoverable))
			{
				_state.hoverable.OnUpdate(_state.draggable, input);
				return;
			}

			_state.hoverable.OnExit(_state.draggable, input);
			_state.hoverable = null;
		}

		if (currentHoverable != null)
		{
			_state.hoverable = currentHoverable;
			currentHoverable.OnEnter(_state.draggable, input);
		}
	}

	private void UpdateKeep(GameObject hovered, GameInputData input)
	{
		if (!hovered) { return; }

		_state.dragSource = hovered.GetComponent<IDragSource>();
		_state.draggable = _state.dragSource?.OnPick(input);
	}

	private void UpdatePick(GameObject hovered, GameInputData input)
	{
		if (_state.draggable == null) { return; }

		_state.hoverable?.OnExit(_state.draggable, input);
		_state.hoverable = null;

		_state.draggable?.OnPick(input);
	}

	private void UpdateDrag(GameObject hovered, GameInputData input)
	{
		_state.draggable?.OnUpdate(input);
	}

	private void UpdateDrop(GameObject hovered, GameInputData input)
	{
		State state = this._state;
		this._state = default;

		state.hoverable?.OnExit(state.draggable, input);
		state.hoverable = null;

		bool dropResult = false;
		if (hovered)
		{
			IDragTarget dragTarget = hovered.GetComponent<IDragTarget>();
			dropResult = dragTarget?.OnDrop(state.draggable, input) ?? false;
		}

		state.draggable?.OnDrop(input);
		state.dragSource?.OnDrop(input, dropResult);
	}

	// ----- ----- ----- ----- -----
	//     MonoBehaviour
	// ----- ----- ----- ----- -----

	private void Awake()
	{
		_inputTracker = new InputTracker(
			dragThreshold: DragThreshold
		);
	}

	private void Update()
	{
		Vector3 position = PlatformInput.GetPrimaryPosition();
		PlatformInput.ActionType actionType = _inputTracker.Do(
			position, PlatformInput.GetAction(0), PlatformInput.GetAction(1)
		);

		Ray worldRay = _camera.ScreenPointToRay(position);
		Physics.Raycast(worldRay, out RaycastHit worldHit);

		float distance = Vector3.Magnitude(_camera.transform.position) / Vector3.Dot(worldRay.direction, Vector3.down);
		GameInputData input = new GameInputData {
			origin = _camera.transform.position,
			direction = worldRay.direction,
			target = _camera.transform.position + worldRay.direction * distance,
		};

		GameObject hovered = worldHit.transform?.gameObject;

		if (_state.pickAction == PlatformInput.ActionType.None)
		{
			switch (actionType)
			{
				case PlatformInput.ActionType.None:
				case PlatformInput.ActionType.Hold:
					if (_state.draggable == null)
					{
						UpdateHover(hovered, input);
					}
					break;

				case PlatformInput.ActionType.Down:
					UpdateKeep(hovered, input);
					break;

				case PlatformInput.ActionType.Up:
					UpdateDrop(null, input);
					break;

				case PlatformInput.ActionType.Tap:
				case PlatformInput.ActionType.Drag:
					_state.pickAction = actionType;
					UpdatePick(hovered, input);
					break;

				default:
					break;
			}
		}
		else
		{
			UpdateDrag(hovered, input);
			switch (actionType)
			{
				case PlatformInput.ActionType.None:
				case PlatformInput.ActionType.Hold:
					UpdateHover(hovered, input);
					break;

				case PlatformInput.ActionType.Tap:
				case PlatformInput.ActionType.Up:
					UpdateDrop(hovered, input);
					break;

				case PlatformInput.ActionType.Cancel:
				case PlatformInput.ActionType.Error:
					UpdateDrop(null, input);
				break;
			}
		}
	}
}
