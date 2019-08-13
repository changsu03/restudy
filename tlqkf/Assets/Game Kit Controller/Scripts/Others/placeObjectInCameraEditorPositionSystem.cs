using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class placeObjectInCameraEditorPositionSystem : MonoBehaviour
{
	public List<Transform> objectsToMoveList = new List<Transform> ();

	public Vector3 positionOffset;

	public float rotationAmount = 90;

	public LayerMask layerToPlaceElements;
	RaycastHit hit;

	public void moveObjects ()
	{
		#if UNITY_EDITOR
		if (SceneView.lastActiveSceneView) {
			if (SceneView.lastActiveSceneView.camera) {
				Camera currentCameraEditor = SceneView.lastActiveSceneView.camera;
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;
				RaycastHit hit;
				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceElements)) {

					Vector3 positionToMove = hit.point + Vector3.right * positionOffset.x + Vector3.up * positionOffset.y + Vector3.forward * positionOffset.z;
					for (int i = 0; i < objectsToMoveList.Count; i++) { 
						objectsToMoveList [i].position = positionToMove;
					}
				}
			}
		}
		#endif
	}

	public void rotateObject (int direction)
	{
		for (int i = 0; i < objectsToMoveList.Count; i++) { 
			objectsToMoveList [i].Rotate (0, direction * rotationAmount, 0);
		}
	}

	public void resetObjectRotation ()
	{
		for (int i = 0; i < objectsToMoveList.Count; i++) { 
			objectsToMoveList [i].rotation = Quaternion.identity;
		}
	}
}
