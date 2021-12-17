using UnityEngine;

public static class Easing
{
	public static float SmoothStep(float t) => t * t * (3 - 2 * t);
}
