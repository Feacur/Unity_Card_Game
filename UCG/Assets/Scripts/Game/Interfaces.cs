using UnityEngine;

public struct GameInputData
{
	public Vector3 origin;
	public Vector3 direction;
	public Vector3 target;
}

public interface IComponent
{
	GameObject GetGO();
	T GetComponent<T>() where T : IComponent;
}

public interface ICompatible
{
	int GetTeam();
}

public interface IPreviewable
{
	void Show(GameInputData input);
	void Hide(GameInputData input);
}

public interface IContent
{
	void Set(int team, string value);
}

public interface IFittable : IComponent
	, IPreviewable
{
	int GetPosition();
	void SetPosition(int index);
}

public interface IDraggable : IComponent
	, ICompatible
{
	void OnPick(GameInputData input);
	void OnUpdate(GameInputData input);
	void OnDrop(GameInputData input);
}

public interface IHoverable : IComponent
{
	void OnEnter(IDraggable draggable, GameInputData input);
	void OnUpdate(IDraggable draggable, GameInputData input);
	void OnExit(IDraggable draggable, GameInputData input);
}

public interface IDragSource : IComponent
{
	IDraggable OnPick(GameInputData input);
	void OnDrop(GameInputData input, bool dropResult);
}

public interface IDragTarget : IComponent
{
	bool OnDrop(IDraggable draggable, GameInputData input);
}
