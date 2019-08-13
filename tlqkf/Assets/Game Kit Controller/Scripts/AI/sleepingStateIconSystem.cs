using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sleepingStateIconSystem : MonoBehaviour
{
	public GameObject sleepIcon;
	public float movementSpeed;
	public float lifeTime;
	public float increaseSizeSpeed;
	public int maxNumberOfIconsAtTime;
	public float spawnRate;

	public float verticalSpeed;
	public float horizontalMovementAmount;
	public float horizontalMovementSpeed;

	public bool sleepStateActive;
	public Transform spawnPosition;
	public float spawnPositionOffset;

	public bool changeTextColorToDuration;
	public Color startColor = Color.white;
	public Color endColor = Color.red;
	public float sleepDuration;

	public health healthManager;

	public Transform parentToAttach;
	public Vector3 localOffset = new Vector3 (0, 0, 0.13f);

	List<sleepIconInfo> sleepIconList = new List<sleepIconInfo> ();
	float lastTimeSpawn;
	float posTargetX;
	Vector3 posTarget;
	Vector3 directionToCamera;
	Transform mainCameraTransform;
	public float transitionColor;

	void Start ()
	{
		mainCameraTransform = FindObjectOfType<gameManager> ().getMainCamera ().transform;

		if (parentToAttach) {
			transform.SetParent (parentToAttach);
			transform.localPosition = Vector3.zero + localOffset;
		}
	}

	void Update ()
	{
		if (sleepStateActive) {
			if (Time.time > lastTimeSpawn + spawnRate && sleepIconList.Count < maxNumberOfIconsAtTime) {
				GameObject newIcon = Instantiate (sleepIcon);
				newIcon.transform.position = spawnPosition.position + Vector3.up * spawnPositionOffset;
				lastTimeSpawn = Time.time;
				newIcon.transform.SetParent (spawnPosition);
				sleepIconInfo newSleepIconInfo = new sleepIconInfo ();
				newSleepIconInfo.iconTransform = newIcon.transform;
				newSleepIconInfo.spawnTime = Time.time;
				newSleepIconInfo.meshText = newIcon.GetComponentInChildren<TextMesh> ();
				newSleepIconInfo.meshText.color = startColor; 
				sleepIconList.Add (newSleepIconInfo);
			}


			for (int i = 0; i < sleepIconList.Count; i++) {
				posTargetX = Mathf.Sin (Time.time * horizontalMovementSpeed) * horizontalMovementAmount;
				posTarget = sleepIconList [i].iconTransform.right * posTargetX;
				posTarget += sleepIconList [i].iconTransform.position;
				posTarget += Vector3.up * verticalSpeed;
				sleepIconList [i].iconTransform.position = Vector3.Lerp (sleepIconList [i].iconTransform.position, posTarget, Time.deltaTime * movementSpeed);
				if (Time.time > sleepIconList [i].spawnTime + lifeTime) {
					Destroy (sleepIconList [i].iconTransform.gameObject);
					sleepIconList.RemoveAt (i);
					i = 0;
				}

				sleepIconList [i].iconTransform.localScale += Vector3.one * increaseSizeSpeed * Time.deltaTime;

				directionToCamera = sleepIconList [i].iconTransform.position - mainCameraTransform.position;
				sleepIconList [i].iconTransform.rotation = Quaternion.LookRotation (directionToCamera);

				if (changeTextColorToDuration) {
					if (sleepDuration > 0) {
						sleepIconList [i].meshText.color = Color.Lerp (startColor, endColor, transitionColor);
					}
				}
			}

			if (changeTextColorToDuration) {
				if (sleepDuration > 0) {
					transitionColor += Time.deltaTime / sleepDuration;
				}
			}
		}
	}

	public void setSleepState (bool state)
	{
		sleepStateActive = state;

		if (sleepStateActive) {
			transitionColor = 0;
			sleepDuration = healthManager.getCurrentSedateDuration ();
		} else {
			for (int i = 0; i < sleepIconList.Count; i++) {
				Destroy (sleepIconList [i].iconTransform.gameObject);
			}
			sleepIconList.Clear ();
		}
	}

	[System.Serializable]
	public class sleepIconInfo
	{
		public Transform iconTransform;
		public float spawnTime;
		public TextMesh meshText;
	}
}
