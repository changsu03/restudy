using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class playerPickupIconManager : MonoBehaviour
{
	public bool showIconsActive = true;
	public bool showIconsPaused;

	public List<pickUpIconInfo> pickUpIconList = new List<pickUpIconInfo> ();
	public GameObject pickUpIconObject;
	public Transform pickupObjectIconParent;
	public LayerMask layer;
	public LayerMask layerForFirstPerson;
	public checkIconType checkIcontype;
	public float maxDistanceIconEnabled;
	public Camera mainCamera;
	public pickUpManager mainPickupManager;
	public menuPause pauseManager;
	public playerController playerControllerManager;

	Transform mainCameraTransform;
	pickUpManager manager;
	Vector3 targetPosition;
	Vector3 cameraPosition;
	GameObject currentIconObject;
	Vector3 screenPoint;
	Vector3 direction;
	float distance;

	LayerMask currentLayer;

	public bool layerForThirdPersonAssigned;
	public bool layerForFirstPersonAssigned;

	//how to check if the icon is visible,
	//		-using a raycast from the object to the camera
	//		-using distance from the object to the player position
	//		-visible always that the player is looking at the object position



	public enum checkIconType
	{
		raycast,
		distance,
		always_visible,
		nothing
	}

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	void Awake ()
	{
		mainPickupManager.addNewPlayer (this);
	}

	void Start ()
	{
		mainCameraTransform = mainCamera.transform;

		mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;
	}

	void FixedUpdate ()
	{
		if (!showIconsActive || showIconsPaused || pickUpIconList.Count == 0) {
			return;
		}

		if (playerControllerManager.isPlayerOnFirstPerson ()) {
			if (!layerForThirdPersonAssigned) {
				currentLayer = layerForFirstPerson;
				layerForThirdPersonAssigned = true;
				layerForFirstPersonAssigned = false;
			}
		} else {
			if (!layerForFirstPersonAssigned) {
				currentLayer = layer;
				layerForFirstPersonAssigned = true;
				layerForThirdPersonAssigned = false;
			}
		}
			
		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].target) {

				currentIconObject = pickUpIconList [i].iconObject;

				if (pickUpIconList [i].paused) {
					if (currentIconObject.activeSelf) {
						enableOrDisableIcon (false, i);
					}
					return;
				}

				//get the target position from global to local in the screen
				targetPosition = pickUpIconList [i].target.transform.position;
				cameraPosition = mainCameraTransform.position;

				if (usingScreenSpaceCamera) {
					screenPoint = mainCamera.WorldToViewportPoint (targetPosition);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
				} else {
					screenPoint = mainCamera.WorldToScreenPoint (targetPosition);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
				}

				//if the target is visible in the screen, enable the icon
				if (targetOnScreen) {
					if (usingScreenSpaceCamera) {
						iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

						pickUpIconList [i].iconRectTransform.anchoredPosition = iconPosition2d;
					} else {
						currentIconObject.transform.position = screenPoint;
					}
						
					//use a raycast to check if the icon is visible
					if (checkIcontype == checkIconType.raycast) {
						distance = GKC_Utils.distance (targetPosition, cameraPosition);
						if (distance <= maxDistanceIconEnabled) {
							//set the direction of the raycast
							direction = targetPosition - cameraPosition;
							direction = direction / direction.magnitude;
							//Debug.DrawRay(target.transform.position,-direction*distance,Color.red);
							//if the raycast find an obstacle between the pick up and the camera, disable the icon
							if (Physics.Raycast (targetPosition, -direction, distance, currentLayer)) {
								if (currentIconObject.activeSelf) {
									enableOrDisableIcon (false, i);
								}
							} else {
								//else, the raycast reachs the camera, so enable the pick up icon
								if (!currentIconObject.activeSelf) {
									enableOrDisableIcon (true, i);
								}
							}
						} else {
							if (currentIconObject.activeSelf) {
								enableOrDisableIcon (false, i);
							}
						}
					} else if (checkIcontype == checkIconType.distance) {
						//if the icon uses the distance, then check it
						distance = GKC_Utils.distance (targetPosition, cameraPosition);
						if (distance <= maxDistanceIconEnabled) {
							if (!currentIconObject.activeSelf) {
								enableOrDisableIcon (true, i);
							}
						} else {
							if (currentIconObject.activeSelf) {
								enableOrDisableIcon (false, i);
							}
						}
					} else {
						//else, always visible when the player is looking at its direction
						if (!currentIconObject.activeSelf) {
							enableOrDisableIcon (true, i);
						}
					}
				} else {
					//else the icon is only disabled, when the player is not looking at its direction
					if (currentIconObject.activeSelf) {
						enableOrDisableIcon (false, i);
					}
				}
			} else {
				removeAtTarget (i);
				i = 0;
				return;
			}
		}
	}

	public void enableOrDisableIcon (bool state, int index)
	{
		pickUpIconList [index].iconObject.SetActive (state);
	}

	//set what type of pick up is this object, and the object that the icon has to follow
	public void setPickUpIcon (GameObject target, Texture targetTexture, int objectID)
	{
		if (checkIcontype == checkIconType.nothing) {
			return;
		}
		GameObject newIconElement = (GameObject)Instantiate (pickUpIconObject, Vector3.zero, Quaternion.identity);

		pickUpIconInfo newIcon = newIconElement.GetComponent<pickUpIcon> ().pickUpElementInfo;
		newIcon.iconObject.transform.SetParent (pickupObjectIconParent);
		newIconElement.transform.localScale = Vector3.one;
		newIconElement.transform.localPosition = Vector3.zero;

		newIcon.target = target;
		newIconElement.gameObject.SetActive (true);
		newIcon.ID = objectID;

		if (targetTexture) {
			newIcon.texture.AddComponent<RawImage> ().texture = targetTexture;
		}

		pickUpIconList.Add (newIcon);

		if (!showIconsActive) {
			newIcon.iconObject.SetActive (false);
		}
	}

	//destroy the icon
	public void removeAtTarget (int index)
	{
		print ("remover " + index);
		if (index < pickUpIconList.Count) {
			if (pickUpIconList [index].iconObject) {
				Destroy (pickUpIconList [index].iconObject);
			}

			mainPickupManager.removeElementFromPickupListCalledByPlayer (pickUpIconList [index].ID);
			pickUpIconList.RemoveAt (index);

		} else {
			print ("WARNING: the index to remove in player pickup icon manager is not correct, check the object picked to see if the icon is configured correctly");
		}
	}

	public void removeAtTargetByID (int objectID)
	{
		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].ID == objectID) {
				if (pickUpIconList [i].iconObject) {
					Destroy (pickUpIconList [i].iconObject);
				}
				pickUpIconList.RemoveAt (i);
				return;
			}
		}
	}

	public void setPauseState (bool state, int index)
	{
		pickUpIconList [index].paused = state;
	}

	public void pauseOrResumeShowIcons (bool state)
	{
		showIconsPaused = state;
		pickupObjectIconParent.gameObject.SetActive (!showIconsPaused);
	}
}