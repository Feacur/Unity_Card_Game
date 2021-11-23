using System;
using UnityEngine;

public class MBTrace : MonoBehaviour
{
	// MonoBehaviour

	private void Awake() => Debug.Log(@$"MBTrace, Awake: {Environment.StackTrace}");
	private void Start() => Debug.Log(@$"MBTrace, Start: {Environment.StackTrace}");
	private void OnEnable() => Debug.Log(@$"MBTrace, OnEnable: {Environment.StackTrace}");
	private void OnDisable() => Debug.Log(@$"MBTrace, OnDisable: {Environment.StackTrace}");
	private void OnDestroy() => Debug.Log(@$"MBTrace, OnDestroy: {Environment.StackTrace}");
}
