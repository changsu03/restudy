using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class damageInScreen : MonoBehaviour
{
	public bool showScreenInfoEnabled;

	public int damageOnScreenId;

	public bool useUIDamageNumber;
	public Transform targetTransform;

	public damageOnScreenInfoSystem damageOnScreenInfoManager;
	public Transform mainCameraTransform;
	public GameObject playerCameraGameObject;
	public GameObject damageNumberPrefab;
	public Transform numbersParent;

	public Color damageColor;
	public Color healColor;
	public bool useRandomColor;
	[Range (0, 1)] public float randomColorAlpha;
	public float fadeSpeed;
	public float maxRadiusToInstantiate;
	public int textSize;
	public bool followCameraRotation;
	public bool useProjectileDirection;
	public bool useRandomDirection;
	public bool removeWhenFade;
	public float movementSpeed;

	bool pauseDamageInScreen;
	List<healthNumber> numbersList = new List<healthNumber> ();
	int i;

	Camera mainCamera;
	Vector3 mainCameraPosition;

	void Start ()
	{
		if (targetTransform == null) {
			targetTransform = transform;
		}

		if (useUIDamageNumber) {
			if (damageOnScreenInfoManager == null) {
				damageOnScreenInfoManager = FindObjectOfType<damageOnScreenInfoSystem> ();
			}

			damageOnScreenId = damageOnScreenInfoManager.addNewTarget (targetTransform);
		} else {
			if (!numbersParent) {
				GameObject newNumbersParent = new GameObject ();
				newNumbersParent.transform.SetParent (transform);
				newNumbersParent.transform.localPosition = Vector3.zero;
				newNumbersParent.transform.localRotation = Quaternion.identity;
				newNumbersParent.name = "Numbers Parent";
				numbersParent = newNumbersParent.transform;
			}

			if (playerCameraGameObject == null) {
				mainCamera = FindObjectOfType<gameManager> ().getMainCamera ();
				mainCameraTransform = mainCamera.transform;
			}
		}
	}

	void Update ()
	{
		if (!useUIDamageNumber && showScreenInfoEnabled) {
			if (numbersList.Count <= 0) {
				return;
			}

			mainCameraPosition = mainCameraTransform.position;

			if (followCameraRotation) {
				Vector3 dir = mainCameraPosition - numbersParent.position;
				numbersParent.rotation = Quaternion.LookRotation (dir);
			}

			for (i = 0; i < numbersList.Count; i++) {
				if (numbersList [i].numberTransform) {
					if (followCameraRotation) {
						Vector3 dir = mainCameraPosition - numbersList [i].numberTransform.transform.position;
						numbersList [i].numberTransform.transform.rotation = Quaternion.LookRotation (dir);
					}
					Color alpha = numbersList [i].meshNumber.color;
					alpha.a -= Time.deltaTime * fadeSpeed;
					numbersList [i].meshNumber.color = alpha;
					if (removeWhenFade) {
						if (alpha.a <= 0) {
							if (numbersList [i].movementCoroutine != null) {
								StopCoroutine (numbersList [i].movementCoroutine);
							}
							Destroy (numbersList [i].numberTransform);
							numbersList.RemoveAt (i);
						}
					}
				} else {
					if (numbersList [i].movementCoroutine != null) {
						StopCoroutine (numbersList [i].movementCoroutine);
					}
					numbersList.RemoveAt (i);
				}
			}
		}
	}

	public void showScreenInfo (float amount, bool damage, Vector3 direction, float healthAmount)
	{
		if (showScreenInfoEnabled && !pauseDamageInScreen) {

			if (useUIDamageNumber) {
				if (showScreenInfoEnabled && !pauseDamageInScreen) {
					damageOnScreenInfoManager.setDamageInfo (damageOnScreenId, amount, damage, direction, healthAmount);
				}
			} else {
				GameObject newNumber = (GameObject)Instantiate (damageNumberPrefab, numbersParent.position, Quaternion.identity);
				newNumber.transform.SetParent (numbersParent);
				if (!useRandomDirection) {
					newNumber.transform.position += Random.insideUnitSphere * maxRadiusToInstantiate;
				}

				Vector3 dir = mainCameraTransform.position - newNumber.transform.position;
				newNumber.transform.rotation = Quaternion.LookRotation (dir);
				string text = "";

				TextMesh currentTextMesh = newNumber.GetComponentInChildren<TextMesh> ();

				if (useRandomColor) {
					if (damage) {
						text = "-";
					} else {
						text = "+";
					}
					currentTextMesh.color = new Vector4 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), randomColorAlpha);
				} else {
					if (damage) {
						currentTextMesh.color = damageColor;
					} else {
						currentTextMesh.color = healColor;
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
				currentTextMesh.text = text;
				currentTextMesh.fontSize = textSize;

				healthNumber newHealthNumber = new healthNumber ();
				newHealthNumber.numberTransform = newNumber;
				newHealthNumber.meshNumber = currentTextMesh;
				newHealthNumber.movementCoroutine = StartCoroutine (moveNumber (newNumber, damage, direction));
				numbersList.Add (newHealthNumber);
			}
		}
	}

	IEnumerator moveNumber (GameObject number, bool damage, Vector3 direction)
	{
		Vector3 currentPosition = number.transform.localPosition;
		Vector3 targetPosition = currentPosition + transform.up;
		if (useRandomDirection) {
			targetPosition = currentPosition + getRandomDirection ();
		}
		if (useProjectileDirection && damage) {
			targetPosition += direction;
		}
		if (removeWhenFade) {
			if (useRandomDirection) {
				while (1 > 0) {
					number.transform.Translate (targetPosition * Time.deltaTime * movementSpeed);
					yield return null;
				}
			} else {
				while (GKC_Utils.distance (number.transform.localPosition, targetPosition) > 0.1f) {
					number.transform.localPosition = Vector3.MoveTowards (number.transform.localPosition, targetPosition, Time.deltaTime * movementSpeed);
					yield return null;
				}
				currentPosition = number.transform.localPosition;
				targetPosition = currentPosition - transform.up * 3;
				while (GKC_Utils.distance (number.transform.localPosition, targetPosition) > 0.1f) {
					number.transform.localPosition = Vector3.Lerp (number.transform.localPosition, targetPosition, Time.deltaTime * movementSpeed);
					yield return null;
				}
			}
		} else {
			while (GKC_Utils.distance (number.transform.localPosition, targetPosition) > 0.1f) {
				number.transform.localPosition = Vector3.MoveTowards (number.transform.localPosition, targetPosition, Time.deltaTime * movementSpeed);
				yield return null;
			}
			if (!useRandomDirection) {
				currentPosition = number.transform.localPosition;
				targetPosition = currentPosition - transform.up * 3;
				while (GKC_Utils.distance (number.transform.localPosition, targetPosition) > 0.1f) {
					number.transform.localPosition = Vector3.Lerp (number.transform.localPosition, targetPosition, Time.deltaTime * movementSpeed);
					yield return null;
				}
			}
			Destroy (number);
		}
	}

	public Vector3 getRandomDirection ()
	{
		Vector3 newDirection = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0);
		return newDirection;
	}

	public void pauseOrPlayDamageInScreen (bool state)
	{
		pauseDamageInScreen = state;
	}

	[System.Serializable]
	public class healthNumber
	{
		public TextMesh meshNumber;
		public GameObject numberTransform;
		public Coroutine movementCoroutine;
	}
}