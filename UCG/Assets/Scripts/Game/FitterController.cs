using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Fitter))]
public class FitterController : MonoBehaviour
{
	public const int CountLimit = 10;

	[Range(0, CountLimit)] public int targetLimit;

	protected bool HaveSpace(int count) => targetLimit == 0 || count < targetLimit;
}
