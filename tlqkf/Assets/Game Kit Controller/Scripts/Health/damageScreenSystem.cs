using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class damageScreenSystem : MonoBehaviour
{
	public bool damageScreenEnabled;
	public menuPause pauseManager;
	public Camera mainCamera;
	public RawImage damageImage;
	public health healtManager;

	public GameObject playerCameraGameObject;
	public GameObject damageScreen;
	public GameObject damageDirectionIcon;
	public GameObject damagePositionIcon;
	public Color damageColor;

	public float maxAlphaDamage = 0.6f;
	public float fadeToDamageColorSpeed;
	public float fadeToTransparentSpeed;
	public float timeToStartToHeal;
	public bool showDamageDirection;
	public bool showDamagePositionWhenEnemyVisible;
	public bool showAllDamageDirections;
	public bool usedByAI;

	public List<damageInfo> enemiesDamageList = new List<damageInfo> ();

	bool wounding;
	bool healWounds;

	int i, j;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;
	Vector3 screenPoint;
	float angle;
	Vector3 screenCenter;

	void Start ()
	{
		if (usedByAI) {
			return;
		}

		damageImage.color = damageColor;

		mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;
	}

	void FixedUpdate ()
	{
		if (usedByAI) {
			return;
		}

		if (damageScreenEnabled) {
			//if the player is wounded, then activate the icon that aims to the enemy position, so the player can see the origin of the damage
			//also, the screen color changes to red, setting the alpha value of a panel in the hud
			if (wounding) {
				Color alpha = damageImage.color;
				if (alpha.a < maxAlphaDamage) {
					float alphaValue = 1 - healtManager.getCurrentHealthAmount() / healtManager.getMaxHealthAmount();
					alpha.a = Mathf.Lerp (alpha.a, alphaValue, Time.deltaTime * fadeToDamageColorSpeed);
				} else {
					alpha.a = maxAlphaDamage;
				}

				damageImage.color = alpha;
				if (showDamageDirection) {
					for (i = 0; i < enemiesDamageList.Count; i++) {
						if (enemiesDamageList [i].enemy != null) {
							if (enemiesDamageList [i].enemy != gameObject) {
								//get the target position from global to local in the screen

								if (usingScreenSpaceCamera) {
									screenPoint = mainCamera.WorldToViewportPoint (enemiesDamageList [i].enemy.transform.position);
									targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
								} else {
									screenPoint = mainCamera.WorldToScreenPoint (enemiesDamageList [i].enemy.transform.position);
									targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
								}

								//if the target is visible in the screen, disable the arrow
								if (targetOnScreen) {
									if (enemiesDamageList [i].damageDirection.activeSelf) {
										enemiesDamageList [i].damageDirection.SetActive (false);
									}
									if (showDamagePositionWhenEnemyVisible) {
										if (!enemiesDamageList [i].damagePosition.activeSelf) {
											enemiesDamageList [i].damagePosition.SetActive (true);
										}
											
										if (usingScreenSpaceCamera) {
											iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x,
												(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

											enemiesDamageList [i].damagePositionRectTransform.anchoredPosition = iconPosition2d;
										} else {
											enemiesDamageList [i].damagePosition.transform.position = screenPoint;
										}
									}
								} else {
									//if the target is off screen, rotate the arrow to the target direction
									if (!enemiesDamageList [i].damageDirection.activeSelf) {
										enemiesDamageList [i].damageDirection.SetActive (true);
										enemiesDamageList [i].damagePosition.SetActive (false);
									}

									if (usingScreenSpaceCamera) {
										iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

										if (screenPoint.z < 0) {
											iconPosition2d *= -1;
										}

										angle = Mathf.Atan2 (iconPosition2d.y, iconPosition2d.x);
										angle -= 90 * Mathf.Deg2Rad;
									} else {
										if (screenPoint.z < 0) {
											screenPoint *= -1;
										}

										screenCenter = new Vector3 (Screen.width, Screen.height, 0) / 2;
										screenPoint -= screenCenter;
										angle = Mathf.Atan2 (screenPoint.y, screenPoint.x);
										angle -= 90 * Mathf.Deg2Rad;
									}

									enemiesDamageList [i].damageDirection.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);
								}
							}

							//if the player is not damaged for a while, disable the arrow
							if (Time.time > enemiesDamageList [i].woundTime + timeToStartToHeal) {
								Destroy (enemiesDamageList [i].damageDirection);
								Destroy (enemiesDamageList [i].damagePosition);
								enemiesDamageList.RemoveAt (i);
							}
						} else {
							enemiesDamageList.RemoveAt (i);
						}
					}
				}
			} 

			if (wounding && enemiesDamageList.Count == 0) {
				healWounds = true;
				wounding = false;
			}

			//if the player is not reciving damage for a while, then set alpha of the red color of the background to 0
			if (healWounds || (wounding && enemiesDamageList.Count == 0)) {
				Color alpha = damageImage.color;
				alpha.a -= Time.deltaTime * fadeToTransparentSpeed;
				damageImage.color = alpha;
				if (alpha.a <= 0) {
					damageScreen.SetActive (false);
					healWounds = false;
				}
			}
		}
	}

	//set the direction of the damage arrow to see the enemy that injured the player
	public void setDamageDir (GameObject enemy)
	{
		if (showAllDamageDirections) {
			bool enemyFound = false;
			int index = -1;
			for (j = 0; j < enemiesDamageList.Count; j++) {
				if (enemiesDamageList [j].enemy == enemy) {
					index = j;
					enemyFound = true;
				}
			}

			if (!enemyFound) {
				damageInfo newEnemy = new damageInfo ();
				newEnemy.enemy = enemy;
				GameObject newDirection = (GameObject)Instantiate (damageDirectionIcon, Vector3.zero, Quaternion.identity);
				newDirection.transform.SetParent (damageScreen.transform);
				newDirection.transform.localScale = Vector3.one;
				newDirection.transform.localPosition = Vector3.zero;
				newEnemy.damageDirection = newDirection;

				GameObject newPosition = (GameObject)Instantiate (damagePositionIcon, Vector3.zero, Quaternion.identity);
				newPosition.transform.SetParent (damageScreen.transform);
				newPosition.transform.localScale = Vector3.one;
				newPosition.transform.localPosition = Vector3.zero;
				newEnemy.damagePosition = newPosition;
				newEnemy.woundTime = Time.time;

				newEnemy.damagePositionRectTransform = newPosition.GetComponent<RectTransform> ();
				enemiesDamageList.Add (newEnemy);
			} else {
				if (index != -1) {
					enemiesDamageList [index].woundTime = Time.time;
				}
			}
		}
		wounding = true;
		damageScreen.SetActive (true);
	}

	[System.Serializable]
	public class damageInfo
	{
		public GameObject enemy;
		public GameObject damageDirection;
		public GameObject damagePosition;
		public float woundTime;
		public RectTransform damagePositionRectTransform;
	}
}