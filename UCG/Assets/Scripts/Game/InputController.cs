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
		public IHoverable hovered;
		public IDragSource pickedFrom;
		public IDraggable picked;
		public bool readyToDrop;
	};
	private State _state;

	private void UpdateHover(GameObject hovered, GameInputData input)
	{
		IHoverable currentHoverable = hovered?.GetComponent<IHoverable>();

		if (_state.hovered != null)
		{
			if (ReferenceEquals(currentHoverable, _state.hovered))
			{
				_state.hovered.OnUpdate(_state.picked, input);
				return;
			}

			_state.hovered.OnExit(_state.picked, input);
			_state.hovered = null;
		}

		if (currentHoverable != null)
		{
			_state.hovered = currentHoverable;
			currentHoverable.OnEnter(_state.picked, input);
		}
	}

	private void UpdatePrepare(GameObject hovered, GameInputData input)
	{
		// stop hovering the picked object
		_state.hovered?.OnExit(_state.picked, input);
		_state.hovered = null;

		_state.picked?.OnPick(input);
		_state.readyToDrop = true;
	}

	private void UpdateDrop(GameObject hovered, GameInputData input)
	{
		State state = this._state;
		this._state = default;

		state.hovered?.OnExit(state.picked, input);
		state.hovered = null;

		bool dropResult = false;
		if (hovered)
		{
			IDragTarget dragTarget = hovered.GetComponent<IDragTarget>();
			dropResult = dragTarget?.OnDrop(state.picked, input) ?? false;
		}

		state.picked?.OnDrop(input);
		state.pickedFrom?.OnDrop(input, dropResult);
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
		// read and interpret input
		Vector3 position = PlatformInput.GetPrimaryPosition();
		PlatformInput.ActionType trackedAction = _inputTracker.Do(
			position, PlatformInput.GetAction(0)
		);

			if (_state.picked != null && PlatformInput.GetAction(1) == PlatformInput.ActionType.Up)
			{
				trackedAction = PlatformInput.ActionType.Cancel;
			}

		// construct input data, always target the ground
		Ray worldRay = _camera.ScreenPointToRay(position);
		Physics.Raycast(worldRay, out RaycastHit worldHit);
		GameObject hovered = worldHit.transform?.gameObject;

		float distance = Vector3.Magnitude(_camera.transform.position) / Vector3.Dot(worldRay.direction, Vector3.down);
		GameInputData input = new GameInputData {
			origin = _camera.transform.position,
			direction = worldRay.direction,
			// _camera.transform.position + worldRay.direction * distance,
			target = new Vector3(
				_camera.transform.position.x + worldRay.direction.x * distance,
				0,
				_camera.transform.position.z + worldRay.direction.z * distance
			),
		};

		// process drag and drop
		if (_state.picked == null)
		{
			if (hovered && trackedAction == PlatformInput.ActionType.Down)
			{
				_state.pickedFrom = hovered.GetComponent<IDragSource>();
				_state.picked = _state.pickedFrom?.OnPick(input);
			}
		}
		else
		{
			if (!_state.readyToDrop)
			{
				switch (trackedAction)
				{
					case PlatformInput.ActionType.Tap:
					case PlatformInput.ActionType.Drag:
						UpdatePrepare(hovered, input);
						break;
				}
			}
			else
			{
				_state.picked?.OnUpdate(input);
				switch (trackedAction)
				{
					case PlatformInput.ActionType.Tap:
					case PlatformInput.ActionType.Up:
						UpdateDrop(hovered, input);
						break;
				}
			}
		}

		// process errors
		switch (trackedAction)
		{
			case PlatformInput.ActionType.Cancel:
			case PlatformInput.ActionType.Error:
				UpdateDrop(null, input);
				break;
		}

		// process hover
		if (_state.picked == null || _state.readyToDrop)
		{
			UpdateHover(hovered, input);
		}
	}
}
