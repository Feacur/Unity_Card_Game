using UnityEngine;

public struct GameInputData
{
	public Vector3 origin;
	public Vector3 direction;
	public Vector3 target;
}

public interface IGameObject
{
	GameObject GetGO();
}

public interface ICompatible
{
	int GetTeam();
	void SetTeam(int value);
}

public interface IPreviewable
{
	void Show(GameInputData input);
	void Hide(GameInputData input);
}

public interface IFittable : IGameObject
	, ICompatible
	, IPreviewable
{
	int GetPosition();
	void SetPosition(int index);

	void SetContent(string value);
}

public interface IDraggable : IGameObject
	, ICompatible
{
	void OnPick(GameInputData input);
	void OnUpdate(GameInputData input);
	void OnDrop(GameInputData input);
}

public interface IHoverable : IGameObject
{
	void OnEnter(IDraggable draggable, GameInputData input);
	void OnUpdate(IDraggable draggable, GameInputData input);
	void OnExit(IDraggable draggable, GameInputData input);
}

public interface IDragSource : IGameObject
{
	IDraggable OnPick(GameInputData input);
	void OnDrop(GameInputData input, bool dropResult);
}

public interface IDragTarget : IGameObject
{
	bool OnDrop(IDraggable draggable, GameInputData input);
}
