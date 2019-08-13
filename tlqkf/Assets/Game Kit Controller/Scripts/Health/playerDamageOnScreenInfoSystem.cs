using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerDamageOnScreenInfoSystem : MonoBehaviour
{
	public int damageOnScreenId;

	public GameObject player;

	public Transform damageNumberParent;
	public GameObject damageNumberTargetParent;
	public GameObject damageNumberText;

	public Camera mainCamera;

	public bool showObjectivesActive = true;
	public bool showObjectivesPaused;

	public bool placeMarkAboveDamagedTargets;
	public RawImage markAboveDamagedTargets;

	public bool useRandomDirection;
	public bool useProjectileDirection;
	public float movementSpeed;
	public float movementAmount;
	public float maxRadiusToInstantiate;
	public float downMovementAmount = 2;

	public bool checkDistanceToTarget;
	public float distanceMultiplierAmount;

	public bool useRandomColor;
	public float randomColorAlpha;
	public Color damageColor = Color.red;
	public Color healColor = Color.green;

	public menuPause pauseManager;

	public damageOnScreenInfoSystem damageOnScreenInfoManager;

	public List<damageOnScreenInfoSystem.targetInfo> targetInfoList = new List<damageOnScreenInfoSystem.targetInfo> ();

	Vector3 currenMapObjectPosition;
	Vector3 screenPoint;

	bool targetOnScreen;

	damageOnScreenInfoSystem.targetInfo currentTargetInfo;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	void Awake ()
	{
		damageOnScreenInfoManager.addNewPlayer (this);
	}

	void Start ()
	{
		mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;

		markAboveDamagedTargets.enabled = placeMarkAboveDamagedTargets;
	}

	void FixedUpdate ()
	{
		if (!showObjectivesActive || showObjectivesPaused || targetInfoList.Count == 0) {
			return;
		}

		for (int i = 0; i < targetInfoList.Count; i++) {
			currentTargetInfo = targetInfoList [i];

			if (currentTargetInfo.containsNumberToShow) {
				if (currentTargetInfo.target && currentTargetInfo.targetRectTransform) {
					currenMapObjectPosition = currentTargetInfo.target.position;

					if (usingScreenSpaceCamera) {
						screenPoint = mainCamera.WorldToViewportPoint (currenMapObjectPosition);
					} else {
						screenPoint = mainCamera.WorldToScreenPoint (currenMapObjectPosition);
					}

					targetOnScreen = screenPoint.z > 0;
						
					if (targetOnScreen) {
						if (!currentTargetInfo.targetRectTransformGameObject.activeSelf) {
							currentTargetInfo.targetRectTransformGameObject.SetActive (true);
						}

						if (usingScreenSpaceCamera) {
							iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
							currentTargetInfo.targetRectTransform.anchoredPosition = iconPosition2d;
						} else {
							currentTargetInfo.targetRectTransform.position = screenPoint;
						}
					} else {
						if (currentTargetInfo.targetRectTransformGameObject.activeSelf) {
							currentTargetInfo.targetRectTransformGameObject.SetActive (false);
						}
					}

					for (int j = 0; j < currentTargetInfo.damageNumberInfoList.Count; j++) {
						if (currentTargetInfo.damageNumberInfoList [j].damageNumberRectTransform == null) {
							currentTargetInfo.damageNumberInfoList.RemoveAt (j);
							j--;
						}
					}

					if (currentTargetInfo.damageNumberInfoList.Count == 0) {
						currentTargetInfo.containsNumberToShow = false;
					}
				} else {
					removeElementFromListByPlayer (currentTargetInfo.ID);
					i--;
				}
			} else {
				if (currentTargetInfo.targetRectTransformGameObject.activeSelf) {
					currentTargetInfo.targetRectTransformGameObject.SetActive (false);
				}

				if(currentTargetInfo.isDead && !currentTargetInfo.containsNumberToShow){
					removeElementFromListByPlayer (currentTargetInfo.ID);
					i--;
				}
			} 
		}
	}

	public void addNewTarget (damageOnScreenInfoSystem.targetInfo newTarget)
	{
		damageOnScreenInfoSystem.targetInfo newTargetInfo = new damageOnScreenInfoSystem.targetInfo ();
		newTargetInfo.Name = newTarget.Name;
		newTargetInfo.target = newTarget.target;
		newTargetInfo.ID = newTarget.ID;

		GameObject newDamageNumberTargetParent = (GameObject)Instantiate (damageNumberTargetParent, Vector3.zero, Quaternion.identity);

		newDamageNumberTargetParent.SetActive (true);
		newDamageNumberTargetParent.transform.SetParent (damageNumberParent);
		newDamageNumberTargetParent.transform.localScale = Vector3.one;
		newDamageNumberTargetParent.transform.localPosition = Vector3.zero;

		newTargetInfo.targetRectTransformGameObject = newDamageNumberTargetParent;
		newTargetInfo.targetRectTransform = newDamageNumberTargetParent.GetComponent<RectTransform> ();
		targetInfoList.Add (newTargetInfo);
	}

	public void setDamageInfo (int targetIndex, float amount, bool isDamage, Vector3 direction, float healthAmount)
	{
		damageOnScreenInfoSystem.targetInfo currentTargetInfoToCheck = targetInfoList [targetIndex];

		if (currentTargetInfoToCheck != null) {

			if(currentTargetInfoToCheck.isDead){
				return;
			}

			GameObject newDamageNumberText = (GameObject)Instantiate (damageNumberText, Vector3.zero, Quaternion.identity);
			newDamageNumberText.SetActive (true);

			newDamageNumberText.transform.SetParent (currentTargetInfoToCheck.targetRectTransform);
			newDamageNumberText.transform.localScale = Vector3.one;
			newDamageNumberText.transform.localPosition = Vector3.zero;

			damageOnScreenInfoSystem.damageNumberInfo newDamageNumberInfo = new damageOnScreenInfoSystem.damageNumberInfo ();

			newDamageNumberInfo.damageNumberText = newDamageNumberText.GetComponent<Text> ();
			newDamageNumberInfo.damageNumberRectTransform = newDamageNumberText.GetComponent<RectTransform> ();

			currentTargetInfoToCheck.damageNumberInfoList.Add (newDamageNumberInfo);

			currentTargetInfoToCheck.containsNumberToShow = true;

			string text = "";
			if (useRandomColor) {
				if (isDamage) {
					text = "-";
				} else {
					text = "+";
				}
				newDamageNumberInfo.damageNumberText.color = new Vector4 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), randomColorAlpha);
			} else {
				if (isDamage) {
					newDamageNumberInfo.damageNumberText.color = damageColor;
				} else {
					newDamageNumberInfo.damageNumberText.color = healColor;
				}
			}

			if (amount >= 1) {
				text += amount.ToString ("0");
			} else {
				if (amount < 0.1 && amount > 0) {
					amount = 0.1f;
				}
				text += amount.ToString ("F1");
			}

			newDamageNumberInfo.damageNumberText.text = text;

			newDamageNumberInfo.movementCoroutine = StartCoroutine (moveNumber (currentTargetInfoToCheck.target, newDamageNumberInfo.damageNumberRectTransform, isDamage, direction));

			if(healthAmount <= 0){
				currentTargetInfoToCheck.isDead = true;
			}
		}
	}

	//if the target is reached, disable all the parameters and clear the list, so a new objective can be added in any moment
	public void removeElementFromList (int objectID)
	{
		for (int i = 0; i < targetInfoList.Count; i++) {
			if (targetInfoList [i].ID == objectID) {
				if (targetInfoList [i].targetRectTransformGameObject) {
					Destroy (targetInfoList [i].targetRectTransformGameObject);
				}

				targetInfoList.RemoveAt (i);
				return;
			}
		}
	}

	public void removeElementFromListByPlayer (int objectID)
	{
		damageOnScreenInfoManager.removeElementFromObjectiveListCalledByPlayer(objectID, player);
		removeElementFromList(objectID);
	}

	IEnumerator moveNumber (Transform targetTransform, RectTransform damageNumberRectTransform, bool damage, Vector2 direction)
	{
		float newMovementAmount = movementAmount;
		float newMaxRadiusToInstantiate = maxRadiusToInstantiate;
		if (checkDistanceToTarget) {
			
			float currentDistance = GKC_Utils.distance (transform.position, targetTransform.position);

			newMovementAmount = newMovementAmount - currentDistance * distanceMultiplierAmount;

			newMovementAmount = Mathf.Abs (newMovementAmount);

			newMaxRadiusToInstantiate = newMaxRadiusToInstantiate - currentDistance * distanceMultiplierAmount;

			newMaxRadiusToInstantiate = Mathf.Abs (newMaxRadiusToInstantiate);
		}

		if (!useRandomDirection) {
			damageNumberRectTransform.anchoredPosition += Random.insideUnitCircle * newMaxRadiusToInstantiate;
		}

		Vector2 currentPosition = damageNumberRectTransform.anchoredPosition;
		Vector2 targetPosition = currentPosition + Vector2.up * newMovementAmount;
		if (useRandomDirection) {
			targetPosition = currentPosition + getRandomDirection () * newMovementAmount;
		}

		if (useProjectileDirection && damage) {
			targetPosition = currentPosition + direction * newMovementAmount;
		}

		while (GKC_Utils.distance (damageNumberRectTransform.anchoredPosition, targetPosition) > 0.1f) {
			damageNumberRectTransform.anchoredPosition = Vector2.MoveTowards (damageNumberRectTransform.anchoredPosition, targetPosition, Time.deltaTime * movementSpeed);
			yield return null;
		}

		if (!useRandomDirection) {
			currentPosition = damageNumberRectTransform.anchoredPosition;
			targetPosition = currentPosition - Vector2.up * newMovementAmount * downMovementAmount;
			while (GKC_Utils.distance (damageNumberRectTransform.anchoredPosition, targetPosition) > 0.1f) {
				damageNumberRectTransform.anchoredPosition = Vector2.MoveTowards (damageNumberRectTransform.anchoredPosition, targetPosition, Time.deltaTime * movementSpeed);
				yield return null;
			}
		}

		Destroy (damageNumberRectTransform.gameObject);
	}

	public Vector2 getRandomDirection ()
	{
		return new Vector2 (Random.Range (-1f, 1f), Random.Range (-1f, 1f));
	}
}
