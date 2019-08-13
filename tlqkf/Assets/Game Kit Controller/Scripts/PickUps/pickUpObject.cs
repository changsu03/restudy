using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class pickUpObject : MonoBehaviour
{
	public pickUpType pickType;
	public int amount;
	public bool useAmountPerUnit;
	public int amountPerUnit;

	public bool useSecondaryString;
	public string secondaryString;
	public bool useTertiaryString;
	public string tertiaryString;

	public AudioClip pickUpSound;
	public bool staticPickUp;
	public bool moveToPlayerOnTrigger = true;
	public pickUpMode pickUpOption;

	public bool canBeExamined;

	public bool usableByAnything;
	public bool usableByPlayer = true;
	public bool usableByVehicles = true;
	public bool usableByCharacters;

	public bool showPickupInfoOnTaken = true;
	public bool usePickupIconOnTaken;
	public Texture pickupIcon;

	public enum pickUpType
	{
		health,
		energy,
		ammo,
		inventory,
		jetpackFuel,
		weapon,
		inventoryExtraSpace,
		map,
		vehicleFuel,
		attachment,
		power
	}

	public enum pickUpMode
	{
		trigger,
		button
	}

	public bool usePickupIconOnScreen = true;
	public string pickupIconGeneralName;
	public string pickupIconName;

	public bool useEventOnTaken;
	public UnityEvent eventOnTaken;
	public bool useEventOnRemainingAmount;
	public UnityEvent eventOnRemainingAmount;

	public bool sendPickupFinder;
	public eventParameters.eventToCallWithGameObject sendPickupFinderEvent;

	bool touched;
	GameObject player;
	GameObject vehicle;
	Rigidbody mainRigidbody;
	bool freeSpaceInInventorySlot;
	int inventoryAmountPicked;
	inventoryManager playerInventoryManager;
	inventoryObject inventoryObjectManager;
	pickUpsScreenInfo pickUpsScreenInfoManager;
	playerWeaponsManager weaponsManager;
	powersAndAbilitiesSystem powersAndAbilitiesManager;
	vehicleHUDManager vehicleHUD;
	Vector3 pickUpTargetPosition;
	GameObject npc;
	public GameObject finder;
	public bool finderIsPlayer;
	public bool finderIsVehicle;
	public bool finderIsCharacter;
	float targetPositionOffset = 1.5f;
	float distanceToByUsed = 1;
	pickUpManager mainPickupManager;
	grabbedObjectState currentGrabbedObject;
	SphereCollider mainSphereCollider;
	public bool examiningObject;

	bool storePickedWeaponsOnInventory;
	deviceStringAction deviceStringActionManager;
	int amountTaken;

	//if the pick up object has an icon in the inspector, instantiated in the hud
	void Start ()
	{
		getComponents ();
	}

	void Update ()
	{
		//if the player enters inside the object's trigger, translate the object's position to the player 
		if (touched) {
			if (finder) {
				pickUpTargetPosition = finder.transform.position + finder.transform.up * targetPositionOffset;
			}
			transform.position = Vector3.MoveTowards (transform.position, pickUpTargetPosition, Time.deltaTime * 15);
			//if the object is close enough, increase the finder's values, according to the type of object
			if (GKC_Utils.distance (transform.position, pickUpTargetPosition) < distanceToByUsed) {
				pickObject ();
			}
		}
	}

	public void getComponents ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();
		setUpIcon ();
		//if the pick up is static, set its rigibody to kinematic and reduce its radius, so the player has to come closer to get it
		mainSphereCollider = GetComponentInChildren<SphereCollider> ();
		if (staticPickUp) {
			mainRigidbody.isKinematic = true;
			mainSphereCollider.radius = 1;
		}
		getInventoryInfo ();

		deviceStringActionManager = GetComponentInChildren<deviceStringAction> ();

		if (deviceStringActionManager) {
			if (pickUpOption == pickUpMode.trigger) {
				deviceStringActionManager.setIconEnabledState (false);
				deviceStringActionManager.gameObject.tag = "Untagged";
			}
		}
	}

	public void playPickupSound ()
	{
		//play the pick up sound effect
		AudioSource finderAudioSource = applyDamage.getAudioSource (finder, "Pickup Object Audio Source");
		if (finderAudioSource) {
			finderAudioSource.PlayOneShot (pickUpSound);
		}

	}

	public void pickObject ()
	{
		//check if this object has been grabbed by the player, to drop it, before destroy it
		checkIfGrabbed ();
		float amountPicked = getAmountPicked ();

		checkEventOnTaken ();

		switch (pickType) {

		//the type of pickup is health
		case pickUpType.health:
			if (amountTaken > 0) {
				amount = (int)amountPicked - amountTaken;
				amountPicked = amountTaken;
			}

			//if the player is not driving then
			if (finderIsPlayer) {
				//increase its health
				applyDamage.setHeal (amountPicked, player);
			} 

			//the player is driving so the pickup will recover its health
			if (finderIsVehicle) {
				applyDamage.setHeal (amountPicked, vehicle);
			}

			if (finderIsCharacter) {
				applyDamage.setHeal (amountPicked, npc);
			}

			//set the info in the screen to show the type of object used and its amount
			showRecieveInfo ("Health x " + amountPicked.ToString ());

			playPickupSound ();

			if (amount > 0 && pickUpOption != pickUpMode.trigger) {
				checkEventOnRemainingAmount ();
				return;
			}

			break;

		//the other pickups works in the same way
		case pickUpType.energy:
			if (amountTaken > 0) {
				amount = (int)amountPicked - amountTaken;
				amountPicked = amountTaken;
			}
				
			if (finderIsPlayer) {
				applyDamage.setEnergy (amountPicked, player);
			}

			if (finderIsVehicle) {
				applyDamage.setEnergy (amountPicked, vehicle);
			}

			if (finderIsCharacter) {
				applyDamage.setEnergy (amountPicked, npc);
			}

			showRecieveInfo ("Energy x " + amountPicked.ToString ());

			playPickupSound ();

			if (amount > 0 && pickUpOption != pickUpMode.trigger) {
				checkEventOnRemainingAmount ();
				return;
			}

			break;

		case pickUpType.ammo:
			if (amountTaken > 0) {
				amount = (int)amountPicked - amountTaken;
				amountPicked = amountTaken;
			}
			if (finderIsPlayer) {
				weaponsManager.AddAmmo ((int)Mathf.Round (amountPicked), secondaryString);
			}

			if (finderIsVehicle) {
				vehicleHUD.getAmmo (secondaryString, (int)Mathf.Round (amountPicked));
			}

			if (finderIsCharacter) {
				weaponsManager.AddAmmo ((int)Mathf.Round (amountPicked), secondaryString);
			}

			showRecieveInfo ("Ammo " + secondaryString + " x " + Mathf.Round (amountPicked).ToString ());

			playPickupSound ();
		
			if (amount > 0 && pickUpOption != pickUpMode.trigger) {
				checkEventOnRemainingAmount ();
				return;
			}
		
			break;

		case pickUpType.inventory:
			if (finderIsPlayer) {
				string info = inventoryObjectManager.inventoryObjectInfo.Name + " Stored";
				if (inventoryAmountPicked > 1) {
					info = inventoryObjectManager.inventoryObjectInfo.Name + " x " + inventoryAmountPicked;
				}

				showRecieveInfo (info);

				playPickupSound ();

				if (inventoryObjectManager.inventoryObjectInfo.amount > 0 && pickUpOption != pickUpMode.trigger) {
					checkEventOnRemainingAmount ();
					inventoryAmountPicked = 0;
					return;
				}
			}
			break;

		case pickUpType.jetpackFuel:
			if (finderIsPlayer) {
				jetpackSystem jetpackManager = player.GetComponent<jetpackSystem> ();
				if (jetpackManager) {
					jetpackManager.getJetpackFuel (amountPicked);
				}
			} 

			showRecieveInfo ("Fuel x " + amountPicked.ToString ());

			playPickupSound ();
			break;

		case pickUpType.weapon:
			bool weaponPickedCorrectly = false;
			if (finderIsPlayer) {
				if (storePickedWeaponsOnInventory) {
					weaponPickedCorrectly = true;
				} else {
					weaponPickedCorrectly = weaponsManager.pickWeapon (secondaryString);
				}
			} 

			if (finderIsCharacter) {
				weaponPickedCorrectly = weaponsManager.pickWeapon (secondaryString);
			} 

			if (!weaponPickedCorrectly) {
				return;
			}

			showRecieveInfo (secondaryString + " Picked");

			playPickupSound ();
			break;

		case pickUpType.inventoryExtraSpace:
			if (finderIsPlayer) {
				int extraSpaceAmount = inventoryObjectManager.inventoryObjectInfo.amount;
				player.GetComponent<inventoryManager> ().addInventoryExtraSpace (extraSpaceAmount);

				showRecieveInfo ("+" + extraSpaceAmount + " slots added to inventory");

				playPickupSound ();
			}
			break;

		case pickUpType.map:
			if (finderIsPlayer) {
				GetComponent<mapZoneUnlocker> ().unlockMapZone ();
				showRecieveInfo ("Map Zone Picked");

				playPickupSound ();
			}
			break;

		case pickUpType.vehicleFuel:
			if (finderIsPlayer) {
				string info = inventoryObjectManager.inventoryObjectInfo.Name + " Stored";
				if (inventoryAmountPicked > 1) {
					info = inventoryObjectManager.inventoryObjectInfo.Name + " x " + inventoryAmountPicked;
				}

				showRecieveInfo (info);

				playPickupSound ();

				if (inventoryObjectManager.inventoryObjectInfo.amount > 0 && pickUpOption != pickUpMode.trigger) {
					checkEventOnRemainingAmount ();
					inventoryAmountPicked = 0;
					return;
				}
			}

			if (finderIsVehicle) {
				applyDamage.setFuel (amountPicked, vehicle);
				showRecieveInfo ("Fuel x " + amountPicked.ToString ());

				playPickupSound ();
			}
			break;

		case pickUpType.attachment:
			if (finderIsPlayer) {				
				showRecieveInfo ("Attachment " + tertiaryString + " Picked");

				playPickupSound ();
			}
			break;

		case pickUpType.power:

			if (finderIsPlayer) {
				powersAndAbilitiesManager.enableGeneralPower (secondaryString);

				showRecieveInfo (secondaryString + " Activated");
			} 

			if (finderIsCharacter) {
				powersAndAbilitiesManager.enableGeneralPower (secondaryString);
			} 

			playPickupSound ();
			break;
		}

		//remove the icon object
		if (mainPickupManager) {
			mainPickupManager.removeTarget (gameObject);
		}

		if (canBeExamined && examiningObject) {
			if (finderIsPlayer) {
				player.GetComponent<usingDevicesSystem> ().setExamineteDevicesCameraState (false);
			}
		}

		Destroy (gameObject);
	}

	public void checkEventOnTaken ()
	{
		if (useEventOnTaken) {
			eventOnTaken.Invoke ();
		}

		if (sendPickupFinder && finder) {
			sendPickupFinderEvent.Invoke (finder);
		}
	}

	public void checkEventOnRemainingAmount ()
	{
		if (useEventOnRemainingAmount) {
			eventOnRemainingAmount.Invoke ();
		}
	}

	public float getAmountPicked ()
	{
		if (useAmountPerUnit) {
			return (amount * amountPerUnit);
		}
		return amount;
	}

	//instantiate the icon object to show the type of pick up in the player's HUD
	public void setUpIcon ()
	{
		if (!usePickupIconOnScreen) {
			return;
		}

		mainPickupManager = FindObjectOfType<pickUpManager> (); 

		if (mainPickupManager) {
			if (pickupIconGeneralName == "") {
				pickupIconGeneralName = pickType.ToString ();
			}
			mainPickupManager.setPickUpIcon (gameObject, pickupIconGeneralName, pickupIconName);
		}
	}

	public void pickObjectByButton ()
	{
		if (canBeExamined && !examiningObject) {
			if (mainPickupManager) {
				mainPickupManager.setPauseState (true, gameObject);
			}
			examiningObject = true;
			return;
		}

		if (!checkIfCanBePicked ()) {
			return;
		}

		checkIfGrabbed ();
		pickObject ();
	}

	public void cancelPickObject ()
	{
		if (canBeExamined && examiningObject) {
			if (mainPickupManager) {
				mainPickupManager.setPauseState (false, gameObject);
			}
			examiningObject = false;
			GetComponentInChildren<electronicDevice> ().activateDevice ();
		}
	}

	//check if the player is inside the object trigger
	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col);
	}

	public void checkTriggerInfo (Collider col)
	{
		if (touched) {
			return;
		}

		if (col.tag == "Player" && !col.isTrigger) {
			player = col.GetComponent<Collider> ().gameObject;
			setFinderType (true, false, false);
			pickUpsScreenInfoManager = player.GetComponent<pickUpsScreenInfo> ();

			//check if the player needs this pickup
			finder = player;
			if (pickUpOption == pickUpMode.trigger) {
				if (!checkIfCanBePicked ()) {
					return;
				}

				Physics.IgnoreCollision (finder.GetComponent<Collider> (), transform.GetComponent<Collider> ());

				checkIfGrabbed ();
				mainRigidbody.isKinematic = true;
				if (moveToPlayerOnTrigger) {
					touched = true;
				} else {
					pickObject ();
				}
			}
		}

		//else check if the player is driving
		else if (col.GetComponent<vehicleHUDManager> ()) {
	
			vehicleHUD = col.GetComponent<vehicleHUDManager> ();

			if (vehicleHUD.isVehicleBeingDriven ()) {
				//then set the vehicle as the object which use the pickup
				vehicle = col.GetComponent<Collider> ().gameObject;
				setFinderType (false, true, false);
				player = vehicleHUD.IKDrivingManager.getcurrentDriver ();
				pickUpsScreenInfoManager = player.GetComponent<pickUpsScreenInfo> ();

				//check if the vehicle needs this pickup
				if (!checkIfCanBePicked ()) {
					return;
				}
				finder = vehicle;

				checkIfGrabbed ();

				if (pickUpOption == pickUpMode.trigger) {
					GetComponent<Collider> ().isTrigger = true;
					mainRigidbody.isKinematic = true;
					if (moveToPlayerOnTrigger) {
						touched = true;
					} else {
						pickObject ();
					}
				} else {
					pickObject ();
				}
			}
		}

		//else check if the finder is an ai
		else if (col.gameObject.GetComponent<AINavMesh> ()) {
			//then set the character as the object which use the pickup
			npc = col.GetComponent<Collider> ().gameObject;
			setFinderType (false, false, true);

			if (!checkIfCanBePicked ()) {
				return;
			}
			finder = npc;

			checkIfGrabbed ();

			if (pickUpOption == pickUpMode.trigger) {
				Physics.IgnoreCollision (finder.GetComponent<Collider> (), transform.GetComponent<Collider> ());

				GetComponent<Collider> ().isTrigger = true;
				mainRigidbody.isKinematic = true;
				if (moveToPlayerOnTrigger) {
					touched = true;
				} else {
					pickObject ();
				}
			} else {
				pickObject ();
			}
		}
	}

	public void setFinderType (bool isPlayer, bool isVehicle, bool isCharacter)
	{
		finderIsPlayer = isPlayer;
		finderIsVehicle = isVehicle;
		finderIsCharacter = isCharacter;
	}

	//check the values of health and energy according to the type of pickup, so the pickup will be used or not according to the values of health or energy
	//When the player/vehicle grabs a pickup, this will check if the amount of health, energy or ammo is filled or not,
	//so the player/vehicle only will get the neccessary objects to restore his state. In version 2.3, the player grabbed every pickup close to him.
	//for example, if the player has 90/100, he only will grab a health pickup
	bool checkIfCanBePicked ()
	{
		if (!usableByAnything) {
			if ((usableByPlayer && finderIsPlayer) || (usableByVehicles && finderIsVehicle) || (usableByCharacters && finderIsCharacter)) {
				//print ("usable");
			} else {
				return false;
			}
		}

		bool canPickCurrentObject = false;
		if (pickType == pickUpType.health) {
			GameObject character = gameObject;

			if (finderIsPlayer) {
				//if the player is not driving then increase an auxiliar value to check the amount of the same pickup that the player will use at once 
				//for example, when the player is close to more than one pickup, if he has 90/100 of health and he is close to two health pickups, 
				//he only will grab one of them.
				character = player;
			} 

			if (finderIsVehicle) {
				//check the same if the player is driving and works in the same way for any type of pickup
				character = vehicle;
			}

			if (finderIsCharacter) {
				character = npc;
			}

			amountTaken = (int)applyDamage.getHealthAmountToPick (character, amount);
			if (amountTaken > 0) {
				canPickCurrentObject = true;
			}
		}

		if (pickType == pickUpType.energy) {
			GameObject character = gameObject;

			if (finderIsPlayer) {
				character = player;
			} 

			if (finderIsVehicle) {
				character = vehicle;
			}

			if (finderIsCharacter) {
				character = npc;
			}

			amountTaken = (int)applyDamage.getEnergyAmountToPick (character, amount);
			//print (amountTaken);
			if (amountTaken > 0) {
				canPickCurrentObject = true;
			}
		}

		if (pickType == pickUpType.ammo) {
			if (finderIsPlayer || finderIsCharacter) {
				GameObject character = player;
				if (finderIsCharacter) {
					character = npc;
				}

				weaponsManager = character.GetComponent<playerWeaponsManager> ();
				bool weaponAvailable = weaponsManager.checkIfWeaponIsAvailable (secondaryString);
				bool weaponHasAmmoLimit = weaponsManager.hasAmmoLimit (secondaryString);

				if (weaponAvailable) {
					if (weaponHasAmmoLimit) {
						bool weaponHasMaximumAmmoAmount = weaponsManager.hasMaximumAmmoAmount (secondaryString);
						if (weaponHasMaximumAmmoAmount) {
							print ("maximum amount on " + secondaryString);
						}
						if (!weaponHasMaximumAmmoAmount) {
							amountTaken = applyDamage.getPlayerWeaponAmmoAmountToPick (weaponsManager, secondaryString, amount);
							print (amountTaken);
							if (amountTaken > 0) {
								canPickCurrentObject = true;
							}
						}
					} else {
						canPickCurrentObject = true;
						amountTaken = amount;
					}
				} 
			}

			if (finderIsVehicle) {
				vehicleWeaponSystem currentVehicleWeaponSystem = vehicle.GetComponentInChildren<vehicleWeaponSystem> ();
				if (currentVehicleWeaponSystem) {
					bool weaponAvailable = currentVehicleWeaponSystem.checkIfWeaponIsAvailable (secondaryString);
					bool weaponHasAmmoLimit = currentVehicleWeaponSystem.hasAmmoLimit (secondaryString);

					if (weaponAvailable) {
						if (weaponHasAmmoLimit) {
							bool weaponHasMaximumAmmoAmount = currentVehicleWeaponSystem.hasMaximumAmmoAmount (secondaryString);
							if (weaponHasMaximumAmmoAmount) {
								print ("maximum amount on " + secondaryString);
							}
							if (!weaponHasMaximumAmmoAmount) {
								amountTaken = applyDamage.getVehicleWeaponAmmoAmountToPick (currentVehicleWeaponSystem, secondaryString, amount);
								if (amountTaken > 0) {
									canPickCurrentObject = true;
								}
							}
						} else {
							canPickCurrentObject = true;
							amountTaken = amount;
						}
					} 
				}
			}
		}

		if (pickType == pickUpType.inventory) {
			if (finderIsPlayer) {
				canPickCurrentObject = tryToPickUpObject ();
			}

			if (finderIsCharacter) {

			}
		}

		if (pickType == pickUpType.jetpackFuel) {
			if (finderIsPlayer) {
				canPickCurrentObject = true;
			}
		}

		if (pickType == pickUpType.weapon) {
			if (finderIsPlayer) {
				weaponsManager = player.GetComponent<playerWeaponsManager> ();
				if (!weaponsManager.isAimingWeapons ()) {
					bool weaponCanBePicked = weaponsManager.checkIfWeaponCanBePicked (secondaryString);
					bool weaponsAreMoving = weaponsManager.weaponsAreMoving ();
					//print ("already picked " + alreadyPicked);
					if (weaponCanBePicked && !weaponsAreMoving && !weaponsManager.currentWeaponIsMoving ()) {
						//check if the weapon can be stored in the inventory too
						bool canBeStoredOnInventory = false;
						bool hasInventoryObjectComponent = false;
						if (weaponsManager.storePickedWeaponsOnInventory) {
							if (inventoryObjectManager) {
								hasInventoryObjectComponent = true;
								canBeStoredOnInventory = tryToPickUpObject ();
								storePickedWeaponsOnInventory = true;
							}
						}

						if ((weaponsManager.storePickedWeaponsOnInventory && canBeStoredOnInventory) ||
						    !weaponsManager.storePickedWeaponsOnInventory || !hasInventoryObjectComponent) {
							canPickCurrentObject = true;
						}
					}
				}
			}

			if (finderIsCharacter) {
				findObjectivesSystem currentfindObjectivesSystem = npc.GetComponent<findObjectivesSystem> ();  

				if (currentfindObjectivesSystem) {
					if (currentfindObjectivesSystem.isSearchingWeapon ()) {
						weaponsManager = npc.GetComponent<playerWeaponsManager> ();
						if (!weaponsManager.isAimingWeapons ()) {
							bool weaponCanBePicked = weaponsManager.checkIfWeaponCanBePicked (secondaryString);
							bool weaponsAreMoving = weaponsManager.weaponsAreMoving ();
							//print ("already picked " + alreadyPicked);
							if (weaponCanBePicked && !weaponsAreMoving) {
								canPickCurrentObject = true;
							}
						}
					}
				}
			}
		}

		if (pickType == pickUpType.inventoryExtraSpace) {
			if (finderIsPlayer) {
				canPickCurrentObject = true;
			}
		}

		if (pickType == pickUpType.map) {
			if (finderIsPlayer) {
				canPickCurrentObject = true;
			}
		}

		if (pickType == pickUpType.vehicleFuel) {
			if (finderIsPlayer) {
				canPickCurrentObject = tryToPickUpObject ();
			}

			if (finderIsVehicle) {
				//check the same if the player is driving and works in the same way for any type of pickup
				amountTaken = (int)applyDamage.getFuelAmountToPick (vehicle, amount);
				if (amountTaken > 0) {
					canPickCurrentObject = true;
				}
			}
		}

		if (pickType == pickUpType.attachment) {
			if (finderIsPlayer) {
				weaponsManager = player.GetComponent<playerWeaponsManager> ();

				if (weaponsManager.pickupAttachment (secondaryString, tertiaryString)) {
					canPickCurrentObject = true;
				} else {
					weaponsManager.showCantPickAttacmentMessage (tertiaryString);
				}
			}
		}

		if (pickType == pickUpType.power) {
			GameObject character = gameObject;

			if (finderIsPlayer) {
				character = player;
			} 
	
			if (finderIsCharacter) {
				character = npc;
			}

			powersAndAbilitiesManager = character.GetComponentInChildren<powersAndAbilitiesSystem> ();

			if (powersAndAbilitiesManager) {
				canPickCurrentObject = true;
			}
		}

		if (deviceStringActionManager != null) {
			deviceStringActionManager.disableIconOnPress = false;
		}

		return canPickCurrentObject;
	}

	public bool tryToPickUpObject ()
	{
		playerInventoryManager = player.GetComponent<inventoryManager> ();
		if (!playerInventoryManager.isInventoryFull ()) {

			if (!playerInventoryManager.playerIsExaminingInventoryObject () && !player.GetComponent<usingDevicesSystem> ().isExaminingObject ()) {
				if (playerInventoryManager.examineObjectBeforeStoreEnabled) {
					playerInventoryManager.examineCurrentPickupObject (inventoryObjectManager.inventoryObjectInfo);

					playerInventoryManager.setCurrentPickupObject (this);
					return false;
				} 
			}

			inventoryAmountPicked = playerInventoryManager.tryToPickUpObject (inventoryObjectManager.inventoryObjectInfo);
			if (inventoryAmountPicked > 0) {
				inventoryObjectManager.inventoryObjectInfo.amount -= inventoryAmountPicked;
				amount = inventoryObjectManager.inventoryObjectInfo.amount;
				return true;
			} else {
				playerInventoryManager.showInventoryFullMessage ();
			}
		} else {
			playerInventoryManager.showInventoryFullMessage ();
		}
		return false;
	}

	//just to ignore the collisions with a turret when it explodes
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer ("Ignore Raycast")) {
			if (col.gameObject.GetComponent<Collider> ()) {
				Physics.IgnoreCollision (col.gameObject.GetComponent<Collider> (), transform.GetComponent<Collider> ());
			}
		}
	}

	//drop this object just in case the object has grabbed it to use it
	void checkIfGrabbed ()
	{
		if (finderIsPlayer && player) {
			//if the object is being carried by the player, make him drop it
			currentGrabbedObject = GetComponent<grabbedObjectState> ();
			if (currentGrabbedObject) {
				GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gameObject);
			}
		}
	}

	//enable the trigger of the pickup, so the player can use it
	public void activateObjectTrigger ()
	{
		if (mainSphereCollider && !mainSphereCollider.enabled) {
			mainSphereCollider.enabled = true;
		}
	}

	public void showRecieveInfo (string message)
	{
		if (!showPickupInfoOnTaken) {
			return;
		}

		if (finderIsPlayer || finderIsVehicle) {
			if (usePickupIconOnTaken) {
				pickUpsScreenInfoManager.recieveInfo (message, pickupIcon);
			} else {
				pickUpsScreenInfoManager.recieveInfo (message);
			}
		}
	}

	public void setPickUpAmount (int amountToSet)
	{
		amount = amountToSet;
		getInventoryInfo ();
	}

	public void getInventoryInfo ()
	{
		if (inventoryObjectManager == null) {
			if (pickType == pickUpType.inventory || pickType == pickUpType.inventoryExtraSpace || pickType == pickUpType.vehicleFuel || pickType == pickUpType.weapon) {
				inventoryObjectManager = GetComponentInChildren<inventoryObject> ();
				if (inventoryObjectManager) {
					setAmount (amount);
				}
			}
		}
	}

	public void setAmount (int newAmount)
	{
		inventoryObjectManager.inventoryObjectInfo.amount = amount;
		if (useAmountPerUnit) {
			inventoryObjectManager.inventoryObjectInfo.amountPerUnit = amountPerUnit;
		}
	}

	public void setNewAmount (int newAmount)
	{
		amount = newAmount;
		if (inventoryObjectManager == null) {
			getInventoryInfo ();
		} else {
			setAmount (amount);
		}
	}

	public SphereCollider getPickupTrigger ()
	{
		return mainSphereCollider;
	}
}