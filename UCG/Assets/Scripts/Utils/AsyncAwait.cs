using System;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;

public struct UnityWebRequestAwaiter : INotifyCompletion
{
	private UnityWebRequestAsyncOperation _operation;
	public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation operation) => _operation = operation;
	public bool IsCompleted => _operation.isDone;
	void INotifyCompletion.OnCompleted(Action continuation) => _operation.completed += _ => continuation();
	public UnityWebRequest GetResult() => _operation.webRequest;
}

public static class AsyncAwait
{
	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation operation)
	{
		return new UnityWebRequestAwaiter(operation);
	}
}
