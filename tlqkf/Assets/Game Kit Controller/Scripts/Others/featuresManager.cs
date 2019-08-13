using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class featuresManager : MonoBehaviour
{
	//this script allows to enable and disable all the features in this asset, so you can configure which of them you need and which you don't
	[Header ("Player Controller Features")]
	public bool enabledRegularJump;
	public bool doubleJump;
	public bool fallDamage;
	public bool holdJumpToSlowDownFall;
	public bool lookAlwaysInCameraDirection;
	public bool lookInCameraDirectionIfLookingAtTarget;
	public bool lookOnlyIfMoving;
	public bool checkForStairAdherenceSystem;
	public bool canMoveWhileAimFirstPerson;
	public bool canMoveWhileAimThirdPerson;
	public bool canMoveWhileAimLockedCamera;
	public bool useLandMark;
	public bool canGetOnVehicles;
	public bool canDrive;
	public bool airDashEnabled;
	public bool sprintEnabled;

	[Header ("")]
	[Header ("Player Camera Features")]
	public bool zoomCamera;
	public bool moveAwayCamera;
	public bool shakeCamera;
	public bool moveAwayCameraInAir;
	public bool useAccelerometer;
	public bool resetCameraRotationAfterTime;
	public bool lookAtTargetEnabled;
	public bool canActivateLookAtTargetEnabled;
	public bool canActiveLookAtTargetOnLockedCamera;
	public bool changeCameraViewEnabled;

	[Header ("")]
	[Header ("Gravity Control Features")]
	public bool gravityPower;
	public bool liftToSearchEnabled;
	public bool randomRotationOnAirEnabled;
	public bool preserveVelocityWhenDisableGravityPower;
	public bool startWithZeroGravityMode;
	public bool canResetRotationOnZeroGravityMode;
	public bool canAdjustToForwardSurface;
	public bool canActivateFreeFloatingMode;
	public bool changeModelColor;


	[Header ("")]
	[Header ("Powers Features")]
	public bool runOnCrouchEnabled;
	public bool wallWalkEnabled;
	public bool aimModeEnabled;
	public bool shieldEnabled;
	public bool grabObjectsEnabled;
	public bool shootEnabled;
	public bool changePowersEnabled;
	public bool trailsActive;
	public bool canGrabObjectsUsingWeapons;
	public bool laserAbilityEnabled;
	public bool canFirePowersWithoutAiming;
	public bool useAimCameraOnFreeFireMode;
	public bool changeCameraSideActive;
	public bool teleportingEnabled;
	public bool headLookWhenAiming;
	public bool useAimAssistInThirdPerson;
	public bool infinitePower;


	[Header ("")]
	[Header ("Grab Object Features")]
	public bool grabObjectEnabled;
	public bool useCursor;
	public bool onlyEnableCursorIfLocatedObject;
	public bool grabInFixedPosition;
	public bool changeGravityObjectsEnabled;
	public bool grabbedObjectCanBeBroken;
	public bool grabObjectsPhysicallyEnabled;
	public bool useObjectToGrabFoundShader;
	public bool enableTransparency;
	public bool canUseZoomWhileGrabbed;


	[Header ("")]
	[Header ("Devices System Features")]
	public bool canUseDevices;
	public bool usePickUpAmountIfEqualToOne;
	public bool showUseDeviceIconEnabled;
	public bool useDeviceButtonEnabled;
	public bool useFixedDeviceIconPosition;
	public bool deviceOnScreenIfUseFixedIconPosition;
	public bool useDeviceFoundShader;
	public bool holdButtonToTakePickupsAround;


	[Header ("")]
	[Header ("Close Combat System Features")]
	public bool combatSystemEnabled;


	[Header ("")]
	[Header ("Foot Step System Features")]
	public bool soundsEnabled;
	public bool useFootPrints;
	public bool useFootParticles;


	[Header ("")]
	[Header ("Scanner System Features")]
	public bool scannerSystemEnabled;


	[Header ("")]
	[Header ("Pick Ups Info Features")]
	public bool pickUpScreenInfoEnabled;


	[Header ("")]
	[Header ("Player Weapons Features")]
	public bool setWeaponWhenPicked;
	public bool canGrabObjectsCarryingWeapons;
	public bool changeToNextWeaponIfAmmoEmpty;
	public bool drawKeepWeaponWhenModeChanged;
	public bool canFireWeaponsWithoutAiming;
	public bool drawWeaponIfFireButtonPressed;
	public bool keepWeaponAfterDelayThirdPerson;
	public bool keepWeaponAfterDelayFirstPerson;
	public bool useQuickDrawWeapon;
	public bool useAimCameraOnFreeFireModeWeapons;
	public bool storePickedWeaponsOnInventoryWeaponSystem;
	public bool drawWeaponWhenPicked;
	public bool changeToNextWeaponWhenUnequipped;
	public bool changeToNextWeaponWhenEquipped;
	public bool notActivateWeaponsAtStart;
	public bool openWeaponAttachmentsMenuEnabled;
	public bool setFirstPersonForAttachmentEditor;
	public bool useUniversalAttachments;
	public bool canDropWeapons;
	public bool changeToNextWeaponWhenDrop;
	public bool dropCurrentWeaponWhenDie;
	public bool dropAllWeaponsWhenDie;
	public bool dropWeaponsOnlyIfUsing;
	public bool drawWeaponWhenResurrect;
	public bool canMarkTargets;
	public bool useAimAssistInThirdPersonWeapons;
	public bool useAimAssistInFirstPerson;
	public bool useAimAssistInLockedCamera;


	[Header ("")]
	[Header ("Inventory Manager Features")]
	public bool inventoryEnabled;
	public bool combineElementsAtDrop;
	public bool useOnlyWhenNeededAmountToUseObject;
	public bool activeNumberOfObjectsToUseMenu;
	public bool setTotalAmountWhenDropObject;
	public bool examineObjectBeforeStoreEnabled;
	public bool storePickedWeaponsOnInventory;
	public bool useDragDropWeaponSlots;
	public bool equipWeaponsWhenPicked;


	[Header ("")]
	[Header ("Jetpack System Features")]
	public bool jetpackEnabled;


	[Header ("")]
	[Header ("Fly System Features")]
	public bool flyModeEnabled;


	[Header ("")]
	[Header ("Damage Screen System Features")]
	public bool damageScreenEnabled;
	public bool showDamageDirection;
	public bool showDamagePositionWhenEnemyVisible;
	public bool showAllDamageDirections;


	[Header ("")]
	[Header ("Damage In Screen System Features")]
	public bool showScreenInfoEnabled;
	public bool useUIDamageNumber;


	[Header ("")]
	[Header ("Friend List Manager Features")]
	public bool friendManagerEnabled;


	[Header ("")]
	[Header ("Player States Manager Features")]
	public bool openPlayerModeMenuEnabled;
	public bool changeModeEnabled;
	public bool closeMenuWhenModeSelected;


	[Header ("")]
	[Header ("Head Track Features")]
	public bool headTrackEnabled;
	public bool lookInCameraDirection;
	public bool lookInOppositeDirectionOutOfRange;
	public bool lookBehindIfMoving;


	[Header ("")]
	[Header ("Hand On Surface IK System Features")]
	public bool adjustHandsToSurfaces;


	[Header ("")]
	[Header ("IK Foot System Features")]
	public bool IKFootSystemEnabled;


	[Header ("")]
	[Header ("Climb Ledge System Features")]
	public bool climbLedgeActive;
	public bool useHangFromLedgeIcon;
	public bool useFixedDeviceIconPositionClimbSystem;
	public bool keepWeaponsOnLedgeDetected;
	public bool drawWeaponsAfterClimbLedgeIfPreviouslyCarried;
	public bool onlyGrabLedgeIfMovingForward;
	public bool canJumpWhenHoldLedge;


	[Header ("")]
	[Header ("Weapons List Manager Features")]
	public bool weaponListManagerEnabled;


	[Header ("")]
	[Header ("Powers Manager Features")]
	public bool powersActive;


	[Header ("")]
	[Header ("Map Features")]
	public bool mapActive;


	[Header ("")]
	[Header ("TimeBullet Features")]
	public bool timeBullet;


	[Header ("")]
	[Header ("")]

	//this script uses parameters inside the player, the camera, the map and the character (the parent of the player)
	public GameObject pController;
	public GameObject pCamera;

	[Header ("")]
	[Header ("")]

	[TextArea (10,10)] public string explanation;

	playerController playerControllerManager;
	playerCamera playerCameraManager;
	otherPowers powersManager;
	gravitySystem gravityManager;
	grabObjects grabObjectsManager;
	usingDevicesSystem usingDevicesManager;
	closeCombatSystem combatManager;
	scannerSystem scannerManager;
	pickUpsScreenInfo pickUpsScreenInfoManager;
	mapSystem mapManager;
	timeBullet timeBulletManager;
	powersListManager powerListManager;
	footStepManager footStepSystem;
	playerWeaponsManager weaponsManager;
	inventoryManager inventorySystem;
	jetpackSystem jetpackManager;
	damageInScreen damageInScreenManager;
	damageScreenSystem damageScreenManager;
	flySystem flyManager;
	friendListManager friendListSystem;
	playerStatesManager mainPlayerStatesManager;
	headTrack headTrackManager;
	handsOnSurfaceIKSystem handOnSurfaceIKManager;
	IKFootSystem IKFootManager;
	climbLedgeSystem climbLedgeManager;
	weaponListManager weaponListManager;

	public void udpateValues (bool settingConfiguration)
	{
		//search the component that has the values to enable or disable
		searchComponent ();

		//Player Controller
		if (playerControllerManager) {
			setBoolValue (ref playerControllerManager.enabledRegularJump, ref enabledRegularJump, !settingConfiguration);
			setBoolValue (ref playerControllerManager.enabledDoubleJump, ref doubleJump, !settingConfiguration);
			setBoolValue (ref playerControllerManager.damageFallEnabled, ref fallDamage, !settingConfiguration);
			setBoolValue (ref playerControllerManager.holdJumpSlowDownFall, ref holdJumpToSlowDownFall, !settingConfiguration);
			setBoolValue (ref playerControllerManager.lookAlwaysInCameraDirection, ref lookAlwaysInCameraDirection, !settingConfiguration);
			setBoolValue (ref playerControllerManager.lookInCameraDirectionIfLookingAtTarget, ref lookInCameraDirectionIfLookingAtTarget, !settingConfiguration);
			setBoolValue (ref playerControllerManager.lookOnlyIfMoving, ref lookOnlyIfMoving, !settingConfiguration);
			setBoolValue (ref playerControllerManager.checkForStairAdherenceSystem, ref checkForStairAdherenceSystem, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canMoveWhileAimFirstPerson, ref canMoveWhileAimFirstPerson, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canMoveWhileAimThirdPerson, ref canMoveWhileAimThirdPerson, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canMoveWhileAimLockedCamera, ref canMoveWhileAimLockedCamera, !settingConfiguration);
			setBoolValue (ref playerControllerManager.useLandMark, ref useLandMark, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canGetOnVehicles, ref canGetOnVehicles, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canDrive, ref canDrive, !settingConfiguration);
			setBoolValue (ref playerControllerManager.airDashEnabled, ref airDashEnabled, !settingConfiguration);
			setBoolValue (ref playerControllerManager.sprintEnabled, ref sprintEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Player Controller script hasn't been found");
		}

		//Player Camera
		if (playerCameraManager) {
			setBoolValue (ref playerCameraManager.settings.zoomEnabled, ref zoomCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.moveAwayCameraEnabled, ref moveAwayCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.enableShakeCamera, ref shakeCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.enableMoveAwayInAir, ref moveAwayCameraInAir, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.useAcelerometer, ref useAccelerometer, !settingConfiguration);
			setBoolValue (ref playerCameraManager.resetCameraRotationAfterTime, ref resetCameraRotationAfterTime, !settingConfiguration);
			setBoolValue (ref playerCameraManager.lookAtTargetEnabled, ref lookAtTargetEnabled, !settingConfiguration);
			setBoolValue (ref playerCameraManager.lookAtTargetEnabled, ref canActivateLookAtTargetEnabled, !settingConfiguration);
			setBoolValue (ref playerCameraManager.canActiveLookAtTargetOnLockedCamera, ref canActiveLookAtTargetOnLockedCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.changeCameraViewEnabled, ref changeCameraViewEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Player Camera script hasn't been found");
		}

		//Gravity System
		if (gravityManager) {
			setBoolValue (ref gravityManager.gravityPowerEnabled, ref gravityPower, !settingConfiguration);
			setBoolValue (ref gravityManager.liftToSearchEnabled, ref liftToSearchEnabled, !settingConfiguration);
			setBoolValue (ref gravityManager.randomRotationOnAirEnabled, ref randomRotationOnAirEnabled, !settingConfiguration);
			setBoolValue (ref gravityManager.preserveVelocityWhenDisableGravityPower, ref preserveVelocityWhenDisableGravityPower, !settingConfiguration);
			setBoolValue (ref gravityManager.startWithZeroGravityMode, ref startWithZeroGravityMode, !settingConfiguration);
			setBoolValue (ref gravityManager.canResetRotationOnZeroGravityMode, ref canResetRotationOnZeroGravityMode, !settingConfiguration);
			setBoolValue (ref gravityManager.canAdjustToForwardSurface, ref canAdjustToForwardSurface, !settingConfiguration);
			setBoolValue (ref gravityManager.canActivateFreeFloatingMode, ref canActivateFreeFloatingMode, !settingConfiguration);
			setBoolValue (ref gravityManager.changeModelColor, ref changeModelColor, !settingConfiguration);
		} else {
			print ("WARNING: Gravity System script hasn't been found");
		}

		//Powers
		if (powersManager) {
			setBoolValue (ref powersManager.settings.runOnCrouchEnabled, ref runOnCrouchEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.wallWalkEnabled, ref wallWalkEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.aimModeEnabled, ref aimModeEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.shieldEnabled, ref shieldEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.grabObjectsEnabled, ref grabObjectsEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.shootEnabled, ref shootEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.changePowersEnabled, ref changePowersEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.trailsActive, ref trailsActive, !settingConfiguration);
			setBoolValue (ref powersManager.canGrabObjectsUsingWeapons, ref canGrabObjectsUsingWeapons, !settingConfiguration);
			setBoolValue (ref powersManager.laserAbilityEnabled, ref laserAbilityEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.canFirePowersWithoutAiming, ref canFirePowersWithoutAiming, !settingConfiguration);
			setBoolValue (ref powersManager.useAimCameraOnFreeFireMode, ref useAimCameraOnFreeFireMode, !settingConfiguration);
			setBoolValue (ref powersManager.changeCameraSideActive, ref changeCameraSideActive, !settingConfiguration);
			setBoolValue (ref powersManager.teleportingEnabled, ref teleportingEnabled, !settingConfiguration);

			setBoolValue (ref powersManager.headLookWhenAiming, ref headLookWhenAiming, !settingConfiguration);
			setBoolValue (ref powersManager.useAimAssistInThirdPerson, ref useAimAssistInThirdPerson, !settingConfiguration);
			setBoolValue (ref powersManager.infinitePower, ref infinitePower, !settingConfiguration);
		} else {
			print ("WARNING: Other Powers script hasn't been found");
		}

		//Grab Objects
		if (grabObjectsManager) {
			setBoolValue (ref grabObjectsManager.grabObjectsEnabled, ref grabObjectEnabled, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.useCursor, ref useCursor, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.onlyEnableCursorIfLocatedObject, ref onlyEnableCursorIfLocatedObject, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.grabInFixedPosition, ref grabInFixedPosition, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.changeGravityObjectsEnabled, ref changeGravityObjectsEnabled, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.grabbedObjectCanBeBroken, ref grabbedObjectCanBeBroken, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.grabObjectsPhysicallyEnabled, ref grabObjectsPhysicallyEnabled, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.useObjectToGrabFoundShader, ref useObjectToGrabFoundShader, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.enableTransparency, ref enableTransparency, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.canUseZoomWhileGrabbed, ref canUseZoomWhileGrabbed, !settingConfiguration);
		} else {
			print ("WARNING: Grab Objects script hasn't been found");
		}

		//Using Devices System
		if (usingDevicesManager) {
			setBoolValue (ref usingDevicesManager.canUseDevices, ref canUseDevices, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.usePickUpAmountIfEqualToOne, ref usePickUpAmountIfEqualToOne, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.showUseDeviceIconEnabled, ref showUseDeviceIconEnabled, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.useDeviceButtonEnabled, ref useDeviceButtonEnabled, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.useFixedDeviceIconPosition, ref useFixedDeviceIconPosition, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.deviceOnScreenIfUseFixedIconPosition, ref deviceOnScreenIfUseFixedIconPosition, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.useDeviceFoundShader, ref useDeviceFoundShader, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.holdButtonToTakePickupsAround, ref holdButtonToTakePickupsAround, !settingConfiguration);
		} else {
			print ("WARNING: Using Devices System script hasn't been found");
		}

		//Close Combat System
		if (combatManager) {
			setBoolValue (ref combatManager.combatSystemEnabled, ref combatSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Close Combat System script hasn't been found");
		}

		//Foot step System
		if (footStepSystem) {
			setBoolValue (ref footStepSystem.soundsEnabled, ref soundsEnabled, !settingConfiguration);
			setBoolValue (ref footStepSystem.useFootPrints, ref useFootPrints, !settingConfiguration);
			setBoolValue (ref footStepSystem.useFootParticles, ref useFootParticles, !settingConfiguration);
		} else {
			print ("WARNING: Foot Step Manager script hasn't been found");
		}

		//Scanner System
		if (scannerManager) {
			setBoolValue (ref scannerManager.scannerSystemEnabled, ref scannerSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Scanner System script hasn't been found");
		}

		//Pick Ups Screen Info
		if (pickUpsScreenInfoManager) {
			setBoolValue (ref pickUpsScreenInfoManager.pickUpScreenInfoEnabled, ref pickUpScreenInfoEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Pickup Screen Info System script hasn't been found");
		}

		//Player Weapons System
		if (weaponsManager) {
			setBoolValue (ref weaponsManager.setWeaponWhenPicked, ref setWeaponWhenPicked, !settingConfiguration);
			setBoolValue (ref weaponsManager.canGrabObjectsCarryingWeapons, ref canGrabObjectsCarryingWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponIfAmmoEmpty, ref changeToNextWeaponIfAmmoEmpty, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawKeepWeaponWhenModeChanged, ref drawKeepWeaponWhenModeChanged, !settingConfiguration);
			setBoolValue (ref weaponsManager.canFireWeaponsWithoutAiming, ref canFireWeaponsWithoutAiming, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawWeaponIfFireButtonPressed, ref drawWeaponIfFireButtonPressed, !settingConfiguration);
			setBoolValue (ref weaponsManager.keepWeaponAfterDelayThirdPerson, ref keepWeaponAfterDelayThirdPerson, !settingConfiguration);
			setBoolValue (ref weaponsManager.keepWeaponAfterDelayFirstPerson, ref keepWeaponAfterDelayFirstPerson, !settingConfiguration);
			setBoolValue (ref weaponsManager.useQuickDrawWeapon, ref useQuickDrawWeapon, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimCameraOnFreeFireMode, ref useAimCameraOnFreeFireModeWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.storePickedWeaponsOnInventory, ref storePickedWeaponsOnInventoryWeaponSystem, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawWeaponWhenPicked, ref drawWeaponWhenPicked, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponWhenUnequipped, ref changeToNextWeaponWhenUnequipped, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponWhenEquipped, ref changeToNextWeaponWhenEquipped, !settingConfiguration);
			setBoolValue (ref weaponsManager.notActivateWeaponsAtStart, ref notActivateWeaponsAtStart, !settingConfiguration);
			setBoolValue (ref weaponsManager.openWeaponAttachmentsMenuEnabled, ref openWeaponAttachmentsMenuEnabled, !settingConfiguration);
			setBoolValue (ref weaponsManager.setFirstPersonForAttachmentEditor, ref setFirstPersonForAttachmentEditor, !settingConfiguration);
			setBoolValue (ref weaponsManager.useUniversalAttachments, ref useUniversalAttachments, !settingConfiguration);
			setBoolValue (ref weaponsManager.canDropWeapons, ref canDropWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponWhenDrop, ref changeToNextWeaponWhenDrop, !settingConfiguration);
			setBoolValue (ref weaponsManager.dropCurrentWeaponWhenDie, ref dropCurrentWeaponWhenDie, !settingConfiguration);
			setBoolValue (ref weaponsManager.dropAllWeaponsWhenDie, ref dropAllWeaponsWhenDie, !settingConfiguration);
			setBoolValue (ref weaponsManager.dropWeaponsOnlyIfUsing, ref dropWeaponsOnlyIfUsing, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawWeaponWhenResurrect, ref drawWeaponWhenResurrect, !settingConfiguration);
			setBoolValue (ref weaponsManager.canMarkTargets, ref canMarkTargets, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimAssistInThirdPerson, ref useAimAssistInThirdPersonWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimAssistInFirstPerson, ref useAimAssistInFirstPerson, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimAssistInLockedCamera, ref useAimAssistInLockedCamera, !settingConfiguration);
		} else {
			print ("WARNING: Player Weapons Manager script hasn't been found");
		}

		//Player Inventory settings
		if (inventorySystem) {
			setBoolValue (ref inventorySystem.inventoryEnabled, ref inventoryEnabled, !settingConfiguration);
			setBoolValue (ref inventorySystem.combineElementsAtDrop, ref combineElementsAtDrop, !settingConfiguration);
			setBoolValue (ref inventorySystem.useOnlyWhenNeededAmountToUseObject, ref useOnlyWhenNeededAmountToUseObject, !settingConfiguration);
			setBoolValue (ref inventorySystem.activeNumberOfObjectsToUseMenu, ref activeNumberOfObjectsToUseMenu, !settingConfiguration);
			setBoolValue (ref inventorySystem.setTotalAmountWhenDropObject, ref setTotalAmountWhenDropObject, !settingConfiguration);
			setBoolValue (ref inventorySystem.examineObjectBeforeStoreEnabled, ref examineObjectBeforeStoreEnabled, !settingConfiguration);
			setBoolValue (ref inventorySystem.storePickedWeaponsOnInventory, ref storePickedWeaponsOnInventory, !settingConfiguration);
			setBoolValue (ref inventorySystem.useDragDropWeaponSlots, ref useDragDropWeaponSlots, !settingConfiguration);
			setBoolValue (ref inventorySystem.equipWeaponsWhenPicked, ref equipWeaponsWhenPicked, !settingConfiguration);
		} else {
			print ("WARNING: Inventory Manager script hasn't been found");
		}

		//Jetpack System settings
		if (jetpackManager) {
			setBoolValue (ref jetpackManager.jetpackEnabled, ref jetpackEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Jetpack System script hasn't been found");
		}

		//Fly System settings
		if (flyManager) {
			setBoolValue (ref flyManager.flyModeEnabled, ref flyModeEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Fly System script hasn't been found");
		}

		//Damage Screen System settings
		if (damageScreenManager) {
			setBoolValue (ref damageScreenManager.damageScreenEnabled, ref damageScreenEnabled, !settingConfiguration);
			setBoolValue (ref damageScreenManager.showDamageDirection, ref showDamageDirection, !settingConfiguration);
			setBoolValue (ref damageScreenManager.showDamagePositionWhenEnemyVisible, ref showDamagePositionWhenEnemyVisible, !settingConfiguration);
			setBoolValue (ref damageScreenManager.showAllDamageDirections, ref showAllDamageDirections, !settingConfiguration);
		} else {
			print ("WARNING: Damage Screen System script hasn't been found");
		}
	
		//Damage In Screen Info settings
		if (damageInScreenManager) {
			setBoolValue (ref damageInScreenManager.showScreenInfoEnabled, ref showScreenInfoEnabled, !settingConfiguration);
			setBoolValue (ref damageInScreenManager.useUIDamageNumber, ref useUIDamageNumber, !settingConfiguration);
		} else {
			print ("WARNING: Damage In Screen script hasn't been found");
		}

		//Damage In Screen Info settings
		if (friendListSystem) {
			setBoolValue (ref friendListSystem.friendManagerEnabled, ref friendManagerEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Friend List Manager script hasn't been found");
		}

		//Player States Manage settings
		if (mainPlayerStatesManager) {
			setBoolValue (ref mainPlayerStatesManager.openPlayerModeMenuEnabled, ref openPlayerModeMenuEnabled, !settingConfiguration);
			setBoolValue (ref mainPlayerStatesManager.changeModeEnabled, ref changeModeEnabled, !settingConfiguration);
			setBoolValue (ref mainPlayerStatesManager.closeMenuWhenModeSelected, ref closeMenuWhenModeSelected, !settingConfiguration);
		} else {
			print ("WARNING: Player States Manager script hasn't been found");
		}

		//Head Track Manage settings
		if (headTrackManager) {
			setBoolValue (ref headTrackManager.headTrackEnabled, ref headTrackEnabled, !settingConfiguration);
			setBoolValue (ref headTrackManager.lookInCameraDirection, ref lookInCameraDirection, !settingConfiguration);
			setBoolValue (ref headTrackManager.lookInOppositeDirectionOutOfRange, ref lookInOppositeDirectionOutOfRange, !settingConfiguration);
			setBoolValue (ref headTrackManager.lookBehindIfMoving, ref lookBehindIfMoving, !settingConfiguration);
		} else {
			print ("WARNING: Head Track script hasn't been found");
		}

		//Hand On Surface IK System settings
		if (handOnSurfaceIKManager) {
			setBoolValue (ref handOnSurfaceIKManager.adjustHandsToSurfaces, ref adjustHandsToSurfaces, !settingConfiguration);
		} else {
			print ("WARNING: Hands On Surface IK System script hasn't been found");
		}

		//IK Foot System settings
		if (IKFootManager) {
			setBoolValue (ref IKFootManager.IKFootSystemEnabled, ref IKFootSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: IK Foot System script hasn't been found");
		}

		//Climb Ledge System settings
		if (climbLedgeManager) {
			setBoolValue (ref climbLedgeManager.climbLedgeActive, ref climbLedgeActive, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.useHangFromLedgeIcon, ref useHangFromLedgeIcon, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.useFixedDeviceIconPosition, ref useFixedDeviceIconPositionClimbSystem, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.keepWeaponsOnLedgeDetected, ref keepWeaponsOnLedgeDetected, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.drawWeaponsAfterClimbLedgeIfPreviouslyCarried, ref drawWeaponsAfterClimbLedgeIfPreviouslyCarried, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.onlyGrabLedgeIfMovingForward, ref onlyGrabLedgeIfMovingForward, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.canJumpWhenHoldLedge, ref canJumpWhenHoldLedge, !settingConfiguration);
		} else {
			print ("WARNING: Climb Ledge System script hasn't been found");
		}

		//Wepons List Manager settings
		if (weaponListManager) {
			setBoolValue (ref weaponListManager.weaponListManagerEnabled, ref weaponListManagerEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Weapons List Manager script hasn't been found");
		}

		//Map settings
		if (mapManager) {
			setBoolValue (ref mapManager.mapEnabled, ref mapActive, !settingConfiguration);
		} else {
			print ("WARNING: Map Manager script hasn't been found");
		}

		//Time Bullet settings
		if (timeBulletManager) {
			setBoolValue (ref timeBulletManager.timeBulletEnabled, ref timeBullet, !settingConfiguration);
		} else {
			print ("WARNING: Time Bullet script hasn't been found");
		}

		//Power List Manager settings
		if (powerListManager) {
			setBoolValue (ref powerListManager.powerListManagerEnabled, ref powersActive, !settingConfiguration);
		} else {
			print ("WARNING: Powers List Manager script hasn't been found");
		}

		//upload every change object in the editor
		updateComponents ();
	}

	public void setBoolValue (ref bool rightValue, ref bool leftValue, bool assignRightValueToLeftValue)
	{
		if (assignRightValueToLeftValue) {
			leftValue = rightValue;
		} else {
			rightValue = leftValue;
		}
	}

	//set the options that the user has configured in the inspector
	public void setConfiguration ()
	{
		udpateValues (true);
	}

	//get the current values of the features, to check the if the booleans fields are correct or not
	public void getConfiguration ()
	{
		udpateValues (false);
	}

	public void updateComponents ()
	{
		EditorUtility.SetDirty (pController.GetComponent<playerController> ());
		EditorUtility.SetDirty (pCamera.GetComponent<playerCamera> ());
		EditorUtility.SetDirty (pController.GetComponent<gravitySystem> ());
		EditorUtility.SetDirty (pController.GetComponent<otherPowers> ());
		EditorUtility.SetDirty (pController.GetComponent<grabObjects> ());
		EditorUtility.SetDirty (pController.GetComponent<usingDevicesSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<closeCombatSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<scannerSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<pickUpsScreenInfo> ());
		EditorUtility.SetDirty (GetComponent<timeBullet> ());
		EditorUtility.SetDirty (GetComponent<mapSystem> ());
		EditorUtility.SetDirty (GetComponent<powersListManager> ());

		EditorUtility.SetDirty (pController.GetComponent<footStepManager> ());
		EditorUtility.SetDirty (pController.GetComponent<playerWeaponsManager> ());
		EditorUtility.SetDirty (pController.GetComponent<inventoryManager> ());
		EditorUtility.SetDirty (pController.GetComponent<jetpackSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<damageInScreen> ());
		EditorUtility.SetDirty (pController.GetComponent<damageScreenSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<flySystem> ());
		EditorUtility.SetDirty (pController.GetComponent<friendListManager> ());
		EditorUtility.SetDirty (pController.GetComponent<playerStatesManager> ());
		EditorUtility.SetDirty (pController.GetComponent<headTrack> ());
		EditorUtility.SetDirty (pController.GetComponent<handsOnSurfaceIKSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<IKFootSystem> ());
		EditorUtility.SetDirty (pController.GetComponent<climbLedgeSystem> ());
		EditorUtility.SetDirty (GetComponent<weaponListManager> ());

		EditorUtility.SetDirty (this);
	}

	void searchComponent ()
	{
		playerControllerManager = pController.GetComponent<playerController> ();
		playerCameraManager = pCamera.GetComponent<playerCamera> ();
		gravityManager = pController.GetComponent<gravitySystem> ();
		powersManager = pController.GetComponent<otherPowers> ();
		grabObjectsManager = pController.GetComponent<grabObjects> ();
		usingDevicesManager = pController.GetComponent<usingDevicesSystem> ();
		combatManager = pController.GetComponent<closeCombatSystem> ();
		scannerManager = pController.GetComponent<scannerSystem> ();
		pickUpsScreenInfoManager = pController.GetComponent<pickUpsScreenInfo> ();
		timeBulletManager = GetComponent<timeBullet> ();
		mapManager = GetComponent<mapSystem> ();
		powerListManager = GetComponent<powersListManager> ();
		footStepSystem = pController.GetComponent<footStepManager> ();

		weaponsManager = pController.GetComponent<playerWeaponsManager> ();
		inventorySystem = pController.GetComponent<inventoryManager> ();
		jetpackManager = pController.GetComponent<jetpackSystem> ();
		damageInScreenManager = pController.GetComponent<damageInScreen> ();
		damageScreenManager = pController.GetComponent<damageScreenSystem> ();
		flyManager = pController.GetComponent<flySystem> ();
		friendListSystem = pController.GetComponent<friendListManager> ();
		mainPlayerStatesManager = pController.GetComponent<playerStatesManager> ();
		headTrackManager = pController.GetComponent<headTrack> ();
		handOnSurfaceIKManager = pController.GetComponent<handsOnSurfaceIKSystem> ();
		IKFootManager = pController.GetComponent<IKFootSystem> ();

		climbLedgeManager = pController.GetComponent<climbLedgeSystem> ();
		weaponListManager = GetComponent<weaponListManager> ();
	}
}
#endif