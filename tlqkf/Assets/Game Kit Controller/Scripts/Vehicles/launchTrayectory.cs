﻿using UnityEngine;
using System.Collections;

public class launchTrayectory : MonoBehaviour
{
	public LayerMask layer;
	public Transform shootPosition;
	public float width;
	public float numberOfPoints;
	public float animationSpeed;
	public float tillingOffset;
	public bool animateTexture;
	public Color textureColor;
	public GameObject character;
	public GameObject characterCamera;
	public bool showGizmo;

	float currentAnimationOffset;
	Vector3 a, b;
	Vector3 rayPoint;
	RaycastHit hit;
	bool rayColliding;
	bool parableEnabled = false;
	LineRenderer lineRenderer;
	float hitDistance;
	Transform mainCameraTransform;
	vehicleCameraController vehicleCameraManager;
	playerCamera playerCameraManager;

	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer> ();
		changeParableState (false);
		vehicleCameraManager = characterCamera.GetComponent<vehicleCameraController> ();
		playerCameraManager = characterCamera.GetComponent<playerCamera> ();
	}

	void Update ()
	{
		//if the player is using the barrel launcher
		if (parableEnabled) {
			//get the start position of the parable
			a = shootPosition.position;
			if (vehicleCameraManager) {
				mainCameraTransform = vehicleCameraManager.getCurrentCameraTransform ();
			}
			if (playerCameraManager) {
				mainCameraTransform = playerCameraManager.getCameraTransform ();
			}
			//check where the camera is looking and 
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
				//enable the linerender
				hitDistance = hit.distance;
				rayPoint = hit.point;
				rayColliding = true;
				lineRenderer.enabled = true;
			} else {
				//disable it
				rayColliding = false;
				lineRenderer.enabled = false;
			}
			if (rayColliding) {
				//if the ray detects a surface, set the linerenderer positions and animated it
				b = rayPoint;
				lineRenderer.positionCount = (int)numberOfPoints + 1;
				//get every linerendere position according to the number of points
				for (float i = 0; i < numberOfPoints + 1; i++) {
					Vector3 p = getParablePoint (a, b, i / numberOfPoints);
					int index = (int)i;
					lineRenderer.SetPosition (index, p);
				}
				//animate the texture of the line renderer by changing its offset texture
				lineRenderer.startWidth = width;
				lineRenderer.endWidth = width;
				if (animateTexture) {
					currentAnimationOffset -= animationSpeed * Time.deltaTime * hitDistance * 0.05f;
					lineRenderer.material.mainTextureScale = new Vector2 (tillingOffset * hitDistance * 0.2f, 1);
					lineRenderer.material.mainTextureOffset = new Vector2 (currentAnimationOffset, lineRenderer.material.mainTextureOffset.y);
					if (lineRenderer.material.HasProperty ("_Color")) {
						lineRenderer.material.color = textureColor;
					}
				}
			}
		}
	}

	public void changeParableState (bool state)
	{
		//enable or disable the barrel launcher parable
		parableEnabled = state;
		if (lineRenderer) {
			if (parableEnabled) {
				lineRenderer.enabled = true;
			} else {
				lineRenderer.enabled = false;
			}
		}
	}

	Vector3 getParablePoint (Vector3 start, Vector3 end, float t)
	{
		//set the height of the parable according to the final position 
		float value = GKC_Utils.distance (start, end) / 65;
		float v0y = Physics.gravity.magnitude * value;
		float height = v0y;
		//translate to local position, to work correctly with the gravity control in the character
		float heightY = Mathf.Abs (transform.InverseTransformDirection (start).y - transform.InverseTransformDirection (end).y);
		if (heightY < 0.1f) {
			//start and end are roughly level
			Vector3 travelDirection = end - start;
			Vector3 result = start + t * travelDirection;
			result += Mathf.Sin (t * Mathf.PI) * height * character.transform.up;
			return result;
		} else {
			//start and end are not level
			Vector3 travelDirection = end - start;
			Vector3 startNew = start - transform.InverseTransformDirection (start).y * character.transform.up;
			startNew += transform.InverseTransformDirection (end).y * character.transform.up;
			Vector3 levelDirection = end - startNew;
			Vector3 right = Vector3.Cross (travelDirection, levelDirection);
			Vector3 up = Vector3.Cross (right, levelDirection);
			if (transform.InverseTransformDirection (end).y > transform.InverseTransformDirection (start).y) {
				up = -up;
			}
			Vector3 result = start + t * travelDirection;
			result += (Mathf.Sin (t * Mathf.PI) * height) * up.normalized;
			return result;
		}
	}

	void OnDrawGizmos ()
	{
		//draw the parable in the editor
		if (showGizmo && Application.isPlaying) {
			GUI.skin.box.fontSize = 16;
			Gizmos.color = Color.red;
			Gizmos.DrawLine (a, b);
			Vector3 lastP = a;
			for (float i = 0; i < numberOfPoints + 1; i++) {
				Vector3 p = getParablePoint (a, b, i / numberOfPoints);
				Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
				Gizmos.DrawLine (lastP, p);
				lastP = p;
			}
		}
	}
}