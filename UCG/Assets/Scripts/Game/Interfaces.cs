using UnityEngine;

public interface IGameObject
{
	GameObject GetGO();
}

public interface ICompatible
{
	int GetTeam();
	void SetTeam(int value);
}

public interface IFittable : IGameObject
	, ICompatible
{
	string GetContent();
	void SetContent(string value);
}

public interface IDraggable : IGameObject
	, ICompatible
{
	void OnPick(Vector3 position);
	void OnUpdate(Vector3 position);
	void OnDrop(Vector3 position);
}

public interface IHoverable : IGameObject
{
	void OnEnter(IDraggable draggable, Vector3 position);
	void OnUpdate(IDraggable draggable, Vector3 position);
	void OnExit(IDraggable draggable, Vector3 position);
}

public interface IDragContainer : IGameObject
{
	IDraggable OnPick(Vector3 position);
	bool OnDrop(IDraggable draggable, Vector3 position);
	void OnPickEnd(Vector3 position, bool dropResult);
}
