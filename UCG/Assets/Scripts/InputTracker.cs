using UnityEngine;

[System.Serializable]
public class InputTracker
{
	private float _dragThreshold;

	public InputTracker(float dragThreshold)
	{
		_dragThreshold = dragThreshold;
	}

	// ----- ----- ----- ----- -----
	//     API
	// ----- ----- ----- ----- -----

	public PlatformInput.ActionType Do(Vector2 position, PlatformInput.ActionType confirm, PlatformInput.ActionType decline)
	{
		if (decline == PlatformInput.ActionType.Up)
		{
			_down = default; _drag = default;
			return PlatformInput.ActionType.Cancel;
		}

		Vector3 from = _down.position;

		bool isDown = _down.frame != 0;
		bool isDrag = _drag.frame != 0;

		switch (confirm)
		{
			case PlatformInput.ActionType.Down: {
				_down = ConstructState(position);
			} break;

			case PlatformInput.ActionType.Up: {
				if (!isDown) { return PlatformInput.ActionType.None; }
				if (!isDrag)
				{
					if (HasPassedDragThreshold(position))
					{
						_down = default;
						return PlatformInput.ActionType.Cancel;
					}
					_down = default;
					return PlatformInput.ActionType.Tap;
				}
				_down = default; _drag = default;
			} break;

			case PlatformInput.ActionType.Hold: {
				if (!isDown) { return PlatformInput.ActionType.None; }
				if (!isDrag && HasPassedDragThreshold(position))
				{
					_drag = ConstructState(position);
					return PlatformInput.ActionType.Drag;
				}
			} break;
		}

		return confirm;
	}

	// ----- ----- ----- ----- -----
	//     Implementation
	// ----- ----- ----- ----- -----

	[System.Serializable]
	private struct State
	{
		public int frame;
		public double time;
		public Vector2 position;
	}
	private State _down, _drag;

	private bool HasPassedDragThreshold(Vector2 position) =>
		Vector2.SqrMagnitude(position - _down.position) >= _dragThreshold * _dragThreshold;

	private static State ConstructState(Vector2 position) => new State
	{
		frame    = Time.frameCount,
		time     = Time.realtimeSinceStartupAsDouble,
		position = position,
	};
}
