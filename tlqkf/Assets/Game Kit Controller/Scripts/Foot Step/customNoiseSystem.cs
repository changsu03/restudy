using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class customNoiseSystem : MonoBehaviour
{
	public List<noiseStateInfo> noiseStateInfoList = new List<noiseStateInfo> ();

	public Transform noiseOriginTransform;

	noiseMeshSystem noiseMeshManager;
	noiseStateInfo currentNoiseState;

	void Start ()
	{
		if (noiseOriginTransform == null) {
			noiseOriginTransform = transform;
		}
	}

	public void setNoiseState (string stateName)
	{
		for (int i = 0; i < noiseStateInfoList.Count; i++) {
			if (noiseStateInfoList [i].Name == stateName && noiseStateInfoList [i].stateEnabled) {
				currentNoiseState = noiseStateInfoList [i];
				activateNoise ();
			}
		}
	}

	public void activateNoise ()
	{
		if (currentNoiseState != null && currentNoiseState.stateEnabled && currentNoiseState.useNoise) {
			if (currentNoiseState.useNoiseDetection) {
				applyDamage.sendNoiseSignal (currentNoiseState.noiseRadius, noiseOriginTransform.position, currentNoiseState.noiseDetectionLayer, currentNoiseState.showNoiseDetectionGizmo);
			}

			if (!noiseMeshManager) {
				noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();
			}

			if (noiseMeshManager) {
				noiseMeshManager.addNoiseMesh (currentNoiseState.noiseRadius, noiseOriginTransform.position + Vector3.up, currentNoiseState.noiseExpandSpeed);
			}

			if (currentNoiseState.useNoiseEvent) {
				currentNoiseState.noiseEvent.Invoke ();
			}
		}
	}

	[System.Serializable]
	public class noiseStateInfo
	{
		public string Name;
		public bool stateEnabled = true;

		public bool useNoise;
		public float noiseRadius;
		public float noiseExpandSpeed;
		public bool useNoiseDetection;
		public LayerMask noiseDetectionLayer;
		public bool showNoiseDetectionGizmo;

		public bool useNoiseEvent;
		public UnityEvent noiseEvent;
	}
}
