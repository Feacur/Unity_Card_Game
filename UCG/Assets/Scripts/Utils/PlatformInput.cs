using UnityEngine;

public static class PlatformInput
{
	public enum ActionType
	{
		None,
		Hold,
		Down,
		Up,
		Cancel,
		Tap,
		Drag,
		Error,
	}

	// ----- ----- ----- ----- -----
	//     API
	// ----- ----- ----- ----- -----

	public static Vector2 GetPrimaryPosition()
	{
	#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isRemoteConnected)
		{
			return GetTouchPosition(0);
		}
		return GetMousePosition();
	#elif UNITY_IOS || UNITY_ANDROID
		return GetTouchPosition(0);
	#else
		return GetMousePosition();
	#endif
	}

	public static ActionType GetAction(int id)
	{
	#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isRemoteConnected)
		{
			return GetTouchAction(id);
		}
		return GetMouseAction(id);
	#elif UNITY_IOS || UNITY_ANDROID
		return GetTouchAction(id);
	#else
		return GetMouseAction(id);
	#endif
	}

	// ----- ----- ----- ----- -----
	//     Implementation (mouse)
	// ----- ----- ----- ----- -----

	private static ActionType GetMouseAction(int id)
	{
		if (Input.GetMouseButtonDown(id)) { return ActionType.Down; }
		if (Input.GetMouseButtonUp(id)) { return ActionType.Up; }
		return Input.GetMouseButton(id) ? ActionType.Hold : ActionType.None;
	}

	private static Vector2 GetMousePosition() => Input.mousePosition;

	// ----- ----- ----- ----- -----
	//     Implementation (touch)
	// ----- ----- ----- ----- -----

	private static ActionType GetTouchAction(int id)
	{
		foreach (Touch touch in Input.touches)
		{
			if (touch.fingerId != id) { continue; }
			switch (touch.phase)
			{
				case TouchPhase.Began:      return ActionType.Down;
				case TouchPhase.Moved:      return ActionType.Hold;
				case TouchPhase.Stationary: return ActionType.Hold;
				case TouchPhase.Ended:      return ActionType.Up;
			}
			return ActionType.Error;

		}
		return ActionType.None;
	}

	private static Vector2 GetTouchPosition(int id)
	{
		foreach (Touch touch in Input.touches)
		{
			if (touch.fingerId != id) { continue; }
			return touch.position;
		}
		return Vector2.zero;
	}
}
