using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class quickTravelStationSystem : MonoBehaviour
{
	public Transform quickTravelTransform;
	public LayerMask layer;
	public float maxDistanceToCheck = 4;

	public string animationName;
	public AudioClip enterAudioSound;
	public bool stationActivated;
	public bool activateAtStart;

	public bool setGravityDirection;

	public setGravity setGravityManager;

	public bool callEventOnTeleport;
	public UnityEvent eventOnTeleport;
	public bool callEventOnEveryTeleport;

	bool eventCalled;

	Animation stationAnimation;
	AudioSource audioSource;
	RaycastHit hit;
	mapObjectInformation mapObjectInformationManager;
	mapCreator mapCreatorManager;

	void Start ()
	{
		stationAnimation = GetComponent<Animation> ();
		audioSource = GetComponent<AudioSource> ();
		mapObjectInformationManager = GetComponent<mapObjectInformation> ();

		if (activateAtStart) {
			activateStation ();
		}

		mapCreatorManager = FindObjectOfType<mapCreator> ();
	}

	public void travelToThisStation (Transform currentPlayer)
	{
		Vector3 positionToTravel = quickTravelTransform.position;
		if (Physics.Raycast (quickTravelTransform.position, -transform.up, out hit, maxDistanceToCheck, layer)) {
			positionToTravel = hit.point + transform.up * 0.3f;
		}
		currentPlayer.position = positionToTravel;
		currentPlayer.rotation = quickTravelTransform.rotation;

		mapCreatorManager.changeCurrentBuilding (mapObjectInformationManager.getBuildingIndex (), currentPlayer.gameObject);

		if (setGravityDirection && setGravityManager) {
			Collider currentCollider = currentPlayer.GetComponent<Collider> ();
			if (currentCollider) {
				setGravityManager.checkTriggerType (currentCollider, true);
			}
		}

		if (callEventOnTeleport) {
			if (!eventCalled || callEventOnEveryTeleport) {
				eventOnTeleport.Invoke ();
				eventCalled = true;
			}
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		if (!stationActivated && col.tag == "Player") {
			audioSource.PlayOneShot (enterAudioSound);
			activateStation ();
		}
	}

	public void activateStation ()
	{
		if (stationAnimation != null && animationName != "") {
			stationAnimation [animationName].speed = 1;
			stationAnimation.Play (animationName);
		}

		mapObjectInformationManager.createMapIconInfo ();
		stationActivated = true;
	}
}