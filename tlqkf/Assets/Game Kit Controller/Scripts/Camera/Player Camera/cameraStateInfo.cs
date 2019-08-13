using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class cameraStateInfo
{
	public string Name;
	public Vector3 camPositionOffset;
	public Vector3 pivotPositionOffset;
	public Vector3 pivotParentPossitionOffset;
	public Vector2 yLimits;

	public float initialFovValue;
	public float fovTransitionSpeed = 10;

	public float maxFovValue;
	public float minFovValue = 17;

	public bool showGizmo;
	public Color gizmoColor;

	public Vector3 originalCamPositionOffset;

	public cameraStateInfo (cameraStateInfo newState)
	{
		Name = newState.Name;
		camPositionOffset = newState.camPositionOffset;
		pivotPositionOffset = newState.pivotPositionOffset;
		pivotParentPossitionOffset = newState.pivotParentPossitionOffset;
		yLimits = newState.yLimits;       
		initialFovValue = newState.initialFovValue;
		fovTransitionSpeed = newState.fovTransitionSpeed;
		maxFovValue = newState.maxFovValue;
		minFovValue = newState.minFovValue;
		originalCamPositionOffset = newState.originalCamPositionOffset;
	}
}