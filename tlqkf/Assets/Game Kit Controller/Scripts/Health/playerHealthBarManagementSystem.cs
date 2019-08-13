using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealthBarManagementSystem : MonoBehaviour
{
	public bool showSlidersActive = true;

	public bool disableHealthBarsOnLockedCamera;

	public bool showSlidersPaused;

	public GameObject playerGameObject;
	public playerController playerControllerManager;

	public playerCamera playerCameraManager;

	public Transform healtSlidersParent;

	public menuPause pauseManager;
	public healthBarManagementSystem healthBarManager;

	public LayerMask layer;
	public LayerMask layerForFirstPerson;

	public float distanceToShowSlider = 200;

	public Camera mainCamera;
	public Transform mainCameraTransform;

	public checkBarTypes checkBarType;

	public enum checkBarTypes
	{
		raycast,
		distance,
		always_visible
	}

	public showSliderModeFirstTime showSliderFirstTimeMode;

	public List<healthSliderInfo> healthSliderInfoList = new List<healthSliderInfo> ();

	public enum showSliderModeFirstTime
	{
		raycast,
		distance,
		inScreen
	}

	Vector3 screenPoint;
	Vector3 currentPosition;
	Vector3 mainCameraPosition;
	Vector3 upDirection;
	Vector3 positionUpDirection;
	Vector3 direction;
	float distanceToMainCamera;

	RaycastHit hit;

	healthSliderInfo currentHealthSliderInfo;

	bool layerForThirdPersonAssigned;
	bool layerForFirstPersonAssigned;
	LayerMask currentLayer;
	bool activeIcon;

	bool checkDisableHealthBars;

	bool isLockedCameraActive;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;
	Vector2 iconPosition2d;
	Vector3 currentSliderPosition;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	void Awake ()
	{
		healthBarManager.addNewPlayer (this);
	}

	void Start ()
	{
		mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;
	}

	void FixedUpdate ()
	{
		if (!showSlidersActive || showSlidersPaused) {
			return;
		}

		isLockedCameraActive = !playerCameraManager.isCameraTypeFree ();

		if (disableHealthBarsOnLockedCamera) {
			if (isLockedCameraActive) {
				if (!checkDisableHealthBars) {
					disableHealhtBars ();
					checkDisableHealthBars = true;
				}
			} else {
				checkDisableHealthBars = false;
			}

			if (isLockedCameraActive) {
				return;
			}
		} else {
			checkDisableHealthBars = false;
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

		//if the health slider has been created, set its position in the screen, so the slider follows the object position
		//to make the slider visible, the player has to see directly the object
		//also, the slider is disabled, when the object is not visible in the screen
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			currentHealthSliderInfo = healthSliderInfoList [i];
			if (currentHealthSliderInfo.sliderGameObject) {
				if (currentHealthSliderInfo.sliderOwner) {
					if (currentHealthSliderInfo.sliderCanBeShown) {
						currentPosition = currentHealthSliderInfo.sliderOwner.position;
						mainCameraPosition = mainCameraTransform.position;

						if (isLockedCameraActive) {
							mainCameraPosition = playerCameraManager.getCurrentLockedCameraTransform ().position;
						}

						upDirection = currentHealthSliderInfo.sliderOwner.up;
						positionUpDirection = currentPosition + upDirection;

						currentSliderPosition = currentPosition + currentHealthSliderInfo.sliderOffset;

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (currentSliderPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (currentSliderPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
						}

						if (targetOnScreen && currentHealthSliderInfo.sliderOwnerLocated) {
							if (usingScreenSpaceCamera) {
								iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

								currentHealthSliderInfo.sliderRectTransform.anchoredPosition = iconPosition2d;
							} else {
								currentHealthSliderInfo.sliderGameObject.transform.position = screenPoint;
							}

//							screenPoint = mainCamera.WorldToViewportPoint (currentSliderPosition);
//							currentHealthSliderInfo.sliderRectTransform.anchorMax = screenPoint;
//							currentHealthSliderInfo.sliderRectTransform.anchorMin = screenPoint;

							if (checkBarType == checkBarTypes.raycast) {

								//set the direction of the raycast
								direction = positionUpDirection - mainCameraPosition;
								direction = direction / direction.magnitude;
								distanceToMainCamera = GKC_Utils.distance (currentPosition, mainCameraPosition);
								activeIcon = false;

								//if the raycast find an obstacle between the enemy and the camera, disable the icon
								//if the distance from the camera to the enemy is higher than 100, disable the raycast and the icon
								if (distanceToMainCamera < distanceToShowSlider) {
									if (Physics.Raycast (positionUpDirection, -direction, out hit, distanceToMainCamera, currentLayer)) {
										Debug.DrawRay (positionUpDirection, -direction * hit.distance, Color.red);
										if (hit.collider.gameObject == currentHealthSliderInfo.sliderOwner) {
											activeIcon = true;
										}
									} else {
										//else, the raycast reachs the camera, so enable the pick up icon
										Debug.DrawRay (positionUpDirection, -direction * distanceToMainCamera, Color.green);
										activeIcon = true;
									}
								}

								if (currentHealthSliderInfo.iconCurrentlyEnabled != activeIcon) {
									setCurrentHealthSliderState (activeIcon);
								}
							} else if (checkBarType == checkBarTypes.distance) {
								//if the icon uses the distance, then check it
								distanceToMainCamera = GKC_Utils.distance (currentPosition, mainCameraPosition);
								if (distanceToMainCamera < distanceToShowSlider) {
									if (!currentHealthSliderInfo.iconCurrentlyEnabled) {
										setCurrentHealthSliderState (true);
									}
								} else {
									if (currentHealthSliderInfo.iconCurrentlyEnabled) {
										setCurrentHealthSliderState (false);
									}
								}
							} else {
								if (!currentHealthSliderInfo.iconCurrentlyEnabled) {
									setCurrentHealthSliderState (true);
								}
							}
						} else {
							if (currentHealthSliderInfo.iconCurrentlyEnabled) {
								setCurrentHealthSliderState (false);
							}
						}

						//if the slider is not visible yet, check the camera position
						if (!currentHealthSliderInfo.sliderOwnerLocated) {
							if (isLockedCameraActive) {
								if (targetOnScreen) {
									currentHealthSliderInfo.sliderOwnerLocated = true;
								}
							} else {
								if (showSliderFirstTimeMode == showSliderModeFirstTime.raycast) {
									//when the player looks at the enemy position, enable his slider health bar
									if (Physics.Raycast (mainCameraPosition, mainCameraTransform.forward, out hit, distanceToShowSlider, currentLayer)) {
										if (hit.collider.gameObject == currentHealthSliderInfo.sliderOwner.gameObject ||
										    hit.collider.gameObject.transform.IsChildOf (currentHealthSliderInfo.sliderOwner)) {
											currentHealthSliderInfo.sliderOwnerLocated = true;
										}
									}
								} else if (showSliderFirstTimeMode == showSliderModeFirstTime.inScreen) {
									if (targetOnScreen) {
										currentHealthSliderInfo.sliderOwnerLocated = true;
									}
								} else if (showSliderFirstTimeMode == showSliderModeFirstTime.distance) {
									float distance = GKC_Utils.distance (currentPosition, mainCameraPosition);
									if (distance < distanceToShowSlider) {
										currentHealthSliderInfo.sliderOwnerLocated = true;
									}
								}
							}
						}
					} else {
						if (currentHealthSliderInfo.iconCurrentlyEnabled) {
							setCurrentHealthSliderState (false);
						}
					}
				} else {
					removeElementFromListByPlayer (currentHealthSliderInfo.ID);
					i--;
				}
			} 
		}
	}

	public void setCurrentHealthSliderState (bool state)
	{
		currentHealthSliderInfo.sliderGameObject.SetActive (state);
		currentHealthSliderInfo.iconCurrentlyEnabled = state;
	}

	public void disableHealhtBars ()
	{
		enableOrDisableHealhtBars (false);
	}

	public void enableHealhtBars ()
	{
		enableOrDisableHealhtBars (true);
	}

	public void enableOrDisableHealhtBars (bool state)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			currentHealthSliderInfo = healthSliderInfoList [i];
			if (currentHealthSliderInfo.sliderGameObject && currentHealthSliderInfo.sliderOwner) {
				currentHealthSliderInfo.sliderGameObject.SetActive (state);
				currentHealthSliderInfo.iconCurrentlyEnabled = state;
			}
		}
	}

	public void addNewTargetSlider (GameObject sliderOwner, GameObject sliderPrefab, Vector3 sliderOffset, float healthAmount, string ownerName, Color textColor, Color sliderColor, int objectID)
	{
		healthSliderInfo newHealthSliderInfo = new healthSliderInfo ();
		newHealthSliderInfo.Name = ownerName;
		newHealthSliderInfo.sliderOwner = sliderOwner.transform;
		newHealthSliderInfo.ID = objectID;

		GameObject sliderGameObject = Instantiate (sliderPrefab);
		sliderGameObject.transform.SetParent (healtSlidersParent);
		sliderGameObject.transform.localScale = Vector3.one;
		sliderGameObject.transform.localPosition = Vector3.zero;
		sliderGameObject.transform.localRotation = Quaternion.identity;

		newHealthSliderInfo.sliderGameObject = sliderGameObject;
		newHealthSliderInfo.sliderInfo = sliderGameObject.GetComponent<AIHealtSliderInfo> ();

		newHealthSliderInfo.healthSlider = newHealthSliderInfo.sliderInfo.healthSlider;
		newHealthSliderInfo.sliderOffset = sliderOffset;

		newHealthSliderInfo.healthSlider.maxValue = healthAmount;
		newHealthSliderInfo.healthSlider.value = healthAmount;
		newHealthSliderInfo.iconCurrentlyEnabled = true;

		newHealthSliderInfo.sliderRectTransform = sliderGameObject.AddComponent<RectTransform> ();

		healthSliderInfoList.Add (newHealthSliderInfo);

		udpateSliderInfo (objectID, ownerName, textColor, sliderColor);
	}

	public void removeTargetSlider (int objectID)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				Destroy (healthSliderInfoList [i].sliderGameObject);
				healthSliderInfoList.RemoveAt (i);

				return;
			}
		}
	}

	public void removeElementFromListByPlayer (int objectID)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				Destroy (healthSliderInfoList [i].sliderGameObject);

				healthBarManager.removeElementFromObjectiveListCalledByPlayer (objectID, playerGameObject);
				healthSliderInfoList.RemoveAt (i);

				return;
			}
		}
	}

	public void udpateSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				healthSliderInfoList [i].sliderInfo.nameText.text = newName;
				healthSliderInfoList [i].sliderInfo.nameText.color = textColor;
				healthSliderInfoList [i].sliderInfo.sliderBackground.color = backgroundColor;

				return;
			}
		}
	}

	public void updateSliderAmount (int objectID, float value)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				if (healthSliderInfoList [i].sliderInfo.healthSlider) {
					healthSliderInfoList [i].sliderInfo.healthSlider.value = value;

					return;
				}
			}
		}
	}

	public void setSliderAmount (int objectID, float sliderValue)
	{
		updateSliderAmount (objectID, sliderValue);
	}

	public void setSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		udpateSliderInfo (objectID, newName, textColor, backgroundColor);
	}

	public void setSliderVisibleState (int objectID, bool state)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				healthSliderInfoList [i].sliderCanBeShown = state;

				return;
			}
		}
	}

	public void pauseOrResumeShowHealthSliders (bool state)
	{
		showSlidersPaused = state;
		healtSlidersParent.gameObject.SetActive (!showSlidersPaused);
	}

	public GameObject getPlayerGameObject ()
	{
		return playerGameObject;
	}
}
