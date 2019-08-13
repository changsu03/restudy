using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(IKWeaponSystem))]
//[CanEditMultipleObjects]
public class IKWeaponSystemEditor : Editor
{
	IKWeaponSystem IKWeaponManager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	bool settings;
	bool elementSettings;
	bool showThirdPerson;
	bool showFirstPerson;
	bool showWeaponIdleSettings;
	bool showShotShakeSettings;
	bool showSwaySettings;
	bool showWalkSwaySettings;
	bool showRunSwaySettings;
	bool showElbowInfo;

	bool showSingleWeaponSettings;
	bool showDualWeaponSettings;

	Color buttonColor;
	Vector3 oldPoint;
	Vector3 newPoint;

	Color listButtonBackgroundColor;

	string isCurrentWeapon;
	string carringWeapon;
	string aimingWeapon;
	string surfaceDetected;
	string weaponInRunPosition;
	string inputListOpenedText;
	string handsInPosition;
	string handInPosition;
	string jumpingOnProcess;
	bool expanded;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	Vector3 currentFreeHandlePosition;
	Vector3 newFreeHandlePosition;

	Vector3 snapValue = new Vector3 (.25f, .25f, .25f);

	float thirdPersonHandleRadius;
	float firstPersonHandleRadius;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		IKWeaponManager = (IKWeaponSystem)target;
	}

	void OnSceneGUI ()
	{
		if (!Application.isPlaying) {
			if (IKWeaponManager.showThirdPersonGizmo) {
				style.normal.textColor = IKWeaponManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				if (IKWeaponManager.showPositionGizmo) {
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.aimPosition.position, "Aim \n Position", style);
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.walkPosition.position, "Walk \n Position", style);
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.keepPosition.position, "Keep \n Position", style);
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.aimRecoilPosition.position, "Aim \n Recoil \n Position", style);

					if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionPosition.position, "Surface \n Collision \n Position", style);
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition.position, "Surface \n Collision \n Ray \n Position", style);
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.useRunPosition && IKWeaponManager.thirdPersonWeaponInfo.runPosition) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.runPosition.position, "Run \n Position", style);
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.hasAttachments) {
						if (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition) {
							Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition.position, "Edit \n Attachment \n Position", style);
						}
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
						style.normal.textColor = IKWeaponManager.gizmoLabelColor;
						style.alignment = TextAnchor.MiddleCenter;
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition.position +
						IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition.forward * IKWeaponManager.thirdPersonWeaponInfo.collisionRayDistance,
							"Collision \n Ray \n Distance", style);
					}
				}

				thirdPersonHandleRadius = IKWeaponManager.thirdPersonWeaponInfo.handleRadius;

				if (IKWeaponManager.showHandsWaypointGizmo) {
					for (int i = 0; i < IKWeaponManager.thirdPersonWeaponInfo.handsInfo.Count; i++) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].position.position, IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].Name, style);

						if (IKWeaponManager.thirdPersonWeaponInfo.useHandleForVertex) {
							for (int j = 0; j < IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints.Count; j++) {

								Handles.color = IKWeaponManager.thirdPersonWeaponInfo.handleGizmoColor;

								showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints [j],
									"move Third IKWeapon Waypoint Info Handle" + i.ToString (), thirdPersonHandleRadius);
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.usePositionHandle) {
							for (int j = 0; j < IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints.Count; j++) {

								showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints [j], "move handle waypoint" + i.ToString ());
							}
						}
					}
				}

				if (IKWeaponManager.showWeaponWaypointGizmo) {
					for (int i = 0; i < IKWeaponManager.thirdPersonWeaponInfo.keepPath.Count; i++) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.keepPath [i].position, (1 + i).ToString (), style);

						if (IKWeaponManager.thirdPersonWeaponInfo.useHandleForVertex) {
							for (int j = 0; j < IKWeaponManager.thirdPersonWeaponInfo.keepPath.Count; j++) {

								Handles.color = IKWeaponManager.thirdPersonWeaponInfo.handleGizmoColor;

								showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPath [i], "move Third IKWeapon Weapon Waypoint Info Handle" + i.ToString (), thirdPersonHandleRadius);
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.usePositionHandle) {
							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPath [i], "move weapon handle waypoint" + i.ToString ());
						}
					}
				}

				if (IKWeaponManager.showPositionGizmo) {
					if (IKWeaponManager.thirdPersonWeaponInfo.useHandleForVertex) {
						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.aimPosition, "move Third Aim Position Info Handle", thirdPersonHandleRadius);

						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.walkPosition, "move Third Walk Position Info Handle", thirdPersonHandleRadius);
						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPosition, "move Third Keep Position Info Handle", thirdPersonHandleRadius);
						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.aimRecoilPosition, "move Third Aim Recoil Position Info Handle", thirdPersonHandleRadius);


						if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
							showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionPosition, "move Third Aim Surface Collision Position Info Handle", thirdPersonHandleRadius);

							showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition, "move Third Aim Surface Collision Ray Position Info Handle", thirdPersonHandleRadius);
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition) {
								showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition, "move Third Edit Attachment Position Info Handle", thirdPersonHandleRadius);
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.useRunPosition && IKWeaponManager.thirdPersonWeaponInfo.runPosition) {
							showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.runPosition, "move Third Run Position Info Handle", thirdPersonHandleRadius);
						}
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.usePositionHandle) {
						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.aimPosition, "move Third Aim Position Info Handle");

						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.walkPosition, "move Third Walk Position Info Handle");
						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPosition, "move Third Keep Position Info Handle");
						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.aimRecoilPosition, "move Third Aim Recoil Position Info Handle");


						if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionPosition, "move Third Aim Surface Collision Position Info Handle");

							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition, "move Third Aim Surface Collision Ray Position Info Handle");
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition) {
								showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition, "move Third Edit Attachment Position Info Handle");
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.useRunPosition && IKWeaponManager.thirdPersonWeaponInfo.runPosition) {
							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.runPosition, "move Third Run Position Info Handle");
						}
					}
				}
			}

			if (IKWeaponManager.showFirstPersonGizmo) {
				style.normal.textColor = IKWeaponManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				if (IKWeaponManager.showPositionGizmo) {
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.aimPosition.position, "Aim \n Position", style);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.walkPosition.position, "Walk \n Position", style);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.keepPosition.position, "Keep \n Position", style);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.aimRecoilPosition.position, "Aim \n Recoil \n Position", style);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.walkRecoilPosition.position, "Walk \n Recoil \n Position", style);

					if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionPosition.position, "Surface \n Collision \n Position", style);
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition.position, "Surface \n Collision \n Ray \n Position", style);
					}

					if (IKWeaponManager.firstPersonWeaponInfo.useRunPosition) {
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.runPosition.position, "Run \n Position", style);
					}

					if (IKWeaponManager.firstPersonWeaponInfo.hasAttachments) {
						if (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition) {
							Handles.Label (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition.position, "Edit \n Attachment \n Position", style);
						}

						if (IKWeaponManager.firstPersonWeaponInfo.useSightPosition) {
							if (IKWeaponManager.firstPersonWeaponInfo.sightPosition) {
								Handles.Label (IKWeaponManager.firstPersonWeaponInfo.sightPosition.position, "Sight \n Position", style);
							}

							if (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition) {
								Handles.Label (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition.position, "Sight \n Recoil \n Position", style);
							}
						}
					}

					if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
						style.normal.textColor = IKWeaponManager.gizmoLabelColor;
						style.alignment = TextAnchor.MiddleCenter;
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition.position +
						IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition.forward * IKWeaponManager.firstPersonWeaponInfo.collisionRayDistance,
							"Collision \n Ray \n Distance", style);
					}
				}

				firstPersonHandleRadius = IKWeaponManager.firstPersonWeaponInfo.handleRadius;

				if (IKWeaponManager.showPositionGizmo) {
					if (IKWeaponManager.firstPersonWeaponInfo.useHandleForVertex) {
						Handles.color = IKWeaponManager.firstPersonWeaponInfo.handleGizmoColor;

						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.aimPosition, "move First Aim Position Info Handle", firstPersonHandleRadius);
						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.walkPosition, "move First Walk Position Info Handle", firstPersonHandleRadius);
						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.keepPosition, "move First Keep Position Info Handle", firstPersonHandleRadius);
						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.aimRecoilPosition, "move First Aim Recoil Position Info Handle", firstPersonHandleRadius);

						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.walkRecoilPosition, "move First Walk Recoil Position Info Handle", firstPersonHandleRadius);


						if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
							showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionPosition, "move First Aim Surface Collision Position Info Handle", firstPersonHandleRadius);
							showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition, "move First Aim Surface Collision Ray Position Info Handle", firstPersonHandleRadius);
						}

						if (IKWeaponManager.firstPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition) {
								showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition, "move First Edit Attachment Position Info Handle", firstPersonHandleRadius);
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useSightPosition) {
								if (IKWeaponManager.firstPersonWeaponInfo.sightPosition) {
									showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.sightPosition, "move First Sight Position Info Handle", firstPersonHandleRadius);
								}

								if (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition) {
									showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition, "move First Sight Recoil Position Info Handle", firstPersonHandleRadius);
								}
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useRunPosition && IKWeaponManager.firstPersonWeaponInfo.runPosition) {
								showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.runPosition, "move First Run Position Info Handle", firstPersonHandleRadius);
							}
						}
					}

					if (IKWeaponManager.firstPersonWeaponInfo.usePositionHandle) {
						Handles.color = IKWeaponManager.firstPersonWeaponInfo.handleGizmoColor;

						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.aimPosition, "move First Aim Position Info Handle");
						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.walkPosition, "move First Walk Position Info Handle");
						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.keepPosition, "move First Keep Position Info Handle");
						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.aimRecoilPosition, "move First Aim Recoil Position Info Handle");

						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.walkRecoilPosition, "move First Walk Recoil Position Info Handle");


						if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
							showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionPosition, "move First Aim Surface Collision Position Info Handle");
							showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition, "move First Aim Surface Collision Ray Position Info Handle");
						}

						if (IKWeaponManager.firstPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition) {
								showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition, "move First Edit Attachment Position Info Handle");
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useSightPosition) {
								if (IKWeaponManager.firstPersonWeaponInfo.sightPosition) {
									showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.sightPosition, "move First Sight Position Info Handle");
								}

								if (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition) {
									showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition, "move First Sight Recoil Position Info Handle");
								}
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useRunPosition && IKWeaponManager.firstPersonWeaponInfo.runPosition) {
								showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.runPosition, "move First Run Position Info Handle");
							}
						}
					}
				}
			}
		}
	}

	public void showPositionHandle (Transform currentTransform, string handleName)
	{
		currentRotationHandle = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

		EditorGUI.BeginChangeCheck ();

		curretPositionHandle = currentTransform.position;

		if (Tools.current == Tool.Move) {
			curretPositionHandle = Handles.DoPositionHandle (curretPositionHandle, currentRotationHandle);
		}

		currentRotationHandle = currentTransform.rotation;

		if (Tools.current == Tool.Rotate) {
			currentRotationHandle = Handles.DoRotationHandle (currentRotationHandle, curretPositionHandle);
		}

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (currentTransform, handleName);
			currentTransform.position = curretPositionHandle;
			currentTransform.rotation = currentRotationHandle;
		}
	}

	public void showFreeMoveHandle (Transform currentTransform, string handleName, float handleRadius)
	{
		EditorGUI.BeginChangeCheck ();

		currentFreeHandlePosition = currentTransform.position;
		newFreeHandlePosition = Handles.FreeMoveHandle (currentFreeHandlePosition, Quaternion.identity, handleRadius, snapValue, Handles.CircleHandleCap);

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (currentTransform, handleName);
			currentTransform.position = newFreeHandlePosition;
		}
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Weapon State", "window", GUILayout.Height (30));

		GUILayout.Label ("Weapon Enabled\t " + objectToUse.FindProperty ("weaponEnabled").boolValue);

		isCurrentWeapon = "NO";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("currentWeapon").boolValue) {
				isCurrentWeapon = "YES";
			}
		}
		GUILayout.Label ("Is Current Weapon\t " + isCurrentWeapon);

		carringWeapon = "NO";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("carrying").boolValue) {
				carringWeapon = "YES";
			}
		}
		GUILayout.Label ("Carrying Weapon\t " + carringWeapon);

		aimingWeapon = "NO";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("aiming").boolValue) {
				aimingWeapon = "YES";
			}
		}
		GUILayout.Label ("Aiming Weapon\t " + aimingWeapon);

		surfaceDetected = "NO";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("surfaceDetected").boolValue) {
				surfaceDetected = "YES";
			}
		}
		GUILayout.Label ("Surface Detected\t " + surfaceDetected);

		weaponInRunPosition = "NO";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("weaponInRunPosition").boolValue) {
				weaponInRunPosition = "YES";
			}
		}
		GUILayout.Label ("In Run Position\t " + weaponInRunPosition);

		jumpingOnProcess = "NO";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("jumpingOnProcess").boolValue) {
				jumpingOnProcess = "YES";
			}
		}

		GUILayout.Label ("Jump In Process\t " + jumpingOnProcess);
		GUILayout.Label ("Player Jump Start\t " + objectToUse.FindProperty ("playerOnJumpStart").boolValue);
		GUILayout.Label ("Player Jump End\t " + objectToUse.FindProperty ("playerOnJumpEnd").boolValue);
		GUILayout.Label ("Crouch Active\t " + objectToUse.FindProperty ("crouchingActive").boolValue);
		GUILayout.Label ("Reloading\t\t " + objectToUse.FindProperty ("reloadingWeapon").boolValue);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dual Weapon State", "window", GUILayout.Height (30));
		GUILayout.Label ("Using Dual Weapons\t " + objectToUse.FindProperty ("usingDualWeapon").boolValue);
		GUILayout.Label ("Disabling Dual State\t " + objectToUse.FindProperty ("disablingDualWeapon").boolValue);
		GUILayout.Label ("Right Dual Weapon\t " + objectToUse.FindProperty ("usingRightHandDualWeapon").boolValue);
		GUILayout.Label ("Configured as Dual\t " + objectToUse.FindProperty ("weaponConfiguredAsDualWepaon").boolValue);
		GUILayout.Label ("Linked Weapon\t " + objectToUse.FindProperty ("linkedDualWeaponName").stringValue);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		settings = objectToUse.FindProperty ("showSettings").boolValue;
		elementSettings = objectToUse.FindProperty ("showElementSettings").boolValue;

		EditorGUILayout.BeginVertical ();
		inputListOpenedText = "";
		if (settings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			settings = !settings;
		}
		if (elementSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			elementSettings = !elementSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		objectToUse.FindProperty ("showSettings").boolValue = settings;
		objectToUse.FindProperty ("showElementSettings").boolValue = elementSettings;

		if (settings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the max amount of weapons adn the layer used in weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponGameObject"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("extraRotation"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("aimFovValue"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("aimFovSpeed"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponEnabled"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponPrefabModel"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("inventoryWeaponPrefabObject"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("hideWeaponIfKeptInThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canBeDropped"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canUnlockCursor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponSystemManager"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsManager"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Surface Collision Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("layer"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Prefab Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("relativePathInventory"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("emtpyWeaponPrefab"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Create Weapon Prefab")) {
				IKWeaponManager.createWeaponPrefab ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (elementSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the settings for third and first person", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			showThirdPerson = objectToUse.FindProperty ("showThirdPersonSettings").boolValue;

			buttonColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical ();
			if (showThirdPerson) {
				GUI.backgroundColor = Color.gray;
				inputListOpenedText = "Hide Third Person Settings";
			} else {
				GUI.backgroundColor = buttonColor;
				inputListOpenedText = "Show Third Person Settings";
			}
			if (GUILayout.Button (inputListOpenedText)) {
				showThirdPerson = !showThirdPerson;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			objectToUse.FindProperty ("showThirdPersonSettings").boolValue = showThirdPerson;

			if (showThirdPerson) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Third Person Weapon Settings", "window", GUILayout.Height (30));
				showWeaponSettings (objectToUse.FindProperty ("thirdPersonWeaponInfo"), true);

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (objectToUse.FindProperty ("thirdPersonWeaponInfo.useSwayInfo"));

				if (objectToUse.FindProperty ("thirdPersonWeaponInfo.useSwayInfo").boolValue) {
					showSwaySettings = objectToUse.FindProperty ("showSwaySettings").boolValue;

					EditorGUILayout.BeginVertical ();
					if (showSwaySettings) {
						GUI.backgroundColor = Color.gray;
						inputListOpenedText = "Hide Third Person Sway Settings";
					} else {
						GUI.backgroundColor = buttonColor;
						inputListOpenedText = "Show Third Person Sway Settings";
					}
					if (GUILayout.Button (inputListOpenedText)) {
						showSwaySettings = !showSwaySettings;
					}
					GUI.backgroundColor = buttonColor;
					EditorGUILayout.EndVertical ();

					objectToUse.FindProperty ("showSwaySettings").boolValue = showSwaySettings;
					if (showSwaySettings) {

						if (objectToUse.FindProperty ("thirdPersonWeaponInfo.useRunPosition").boolValue && objectToUse.FindProperty ("thirdPersonWeaponInfo.useRunSwayInfo").boolValue) {
							GUILayout.BeginVertical ("box");

							showWalkSwaySettings = objectToUse.FindProperty ("showWalkSwaySettings").boolValue;

							EditorGUILayout.BeginVertical ();
							if (showWalkSwaySettings) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Walk Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Walk Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showWalkSwaySettings = !showWalkSwaySettings;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							objectToUse.FindProperty ("showWalkSwaySettings").boolValue = showWalkSwaySettings;

							if (showWalkSwaySettings) {
								showSelectedSwaySettings (objectToUse.FindProperty ("thirdPersonSwayInfo"));
							}

							EditorGUILayout.Space ();

							showRunSwaySettings = objectToUse.FindProperty ("showRunSwaySettings").boolValue;

							EditorGUILayout.BeginVertical ();
							if (showRunSwaySettings) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Run Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Run Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showRunSwaySettings = !showRunSwaySettings;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							objectToUse.FindProperty ("showRunSwaySettings").boolValue = showRunSwaySettings;

							if (showRunSwaySettings) {
								showSelectedSwaySettings (objectToUse.FindProperty ("runThirdPersonSwayInfo"));
							}
							EditorGUILayout.EndVertical ();
						} else {
							showSelectedSwaySettings (objectToUse.FindProperty ("thirdPersonSwayInfo"));
						}
					}

					EditorGUILayout.Space ();
				}
			}

			showFirstPerson = objectToUse.FindProperty ("showFirstPersonSettings").boolValue;

			buttonColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical ();
			if (showFirstPerson) {
				GUI.backgroundColor = Color.gray;
				inputListOpenedText = "Hide First Person Settings";
			} else {
				GUI.backgroundColor = buttonColor;
				inputListOpenedText = "Show First Person Settings";
			}
			if (GUILayout.Button (inputListOpenedText)) {
				showFirstPerson = !showFirstPerson;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			objectToUse.FindProperty ("showFirstPersonSettings").boolValue = showFirstPerson;

			if (showFirstPerson) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("First Person Weapon Settings", "window", GUILayout.Height (30));
				showWeaponSettings (objectToUse.FindProperty ("firstPersonWeaponInfo"), false);

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (objectToUse.FindProperty ("firstPersonWeaponInfo.useSwayInfo"));

				if (objectToUse.FindProperty ("firstPersonWeaponInfo.useSwayInfo").boolValue) {

					showSwaySettings = objectToUse.FindProperty ("showSwaySettings").boolValue;

					EditorGUILayout.BeginVertical ();
					if (showSwaySettings) {
						GUI.backgroundColor = Color.gray;
						inputListOpenedText = "Hide First Person Sway Settings";
					} else {
						GUI.backgroundColor = buttonColor;
						inputListOpenedText = "Show First Person Sway Settings";
					}
					if (GUILayout.Button (inputListOpenedText)) {
						showSwaySettings = !showSwaySettings;
					}
					GUI.backgroundColor = buttonColor;
					EditorGUILayout.EndVertical ();

					objectToUse.FindProperty ("showSwaySettings").boolValue = showSwaySettings;
					if (showSwaySettings) {

						if (objectToUse.FindProperty ("firstPersonWeaponInfo.useRunPosition").boolValue && objectToUse.FindProperty ("firstPersonWeaponInfo.useRunSwayInfo").boolValue) {
							GUILayout.BeginVertical ("box");

							showWalkSwaySettings = objectToUse.FindProperty ("showWalkSwaySettings").boolValue;

							EditorGUILayout.BeginVertical ();
							if (showWalkSwaySettings) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Walk Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Walk Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showWalkSwaySettings = !showWalkSwaySettings;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							objectToUse.FindProperty ("showWalkSwaySettings").boolValue = showWalkSwaySettings;

							if (showWalkSwaySettings) {
								showSelectedSwaySettings (objectToUse.FindProperty ("firstPersonSwayInfo"));
							}

							EditorGUILayout.Space ();

							showRunSwaySettings = objectToUse.FindProperty ("showRunSwaySettings").boolValue;

							EditorGUILayout.BeginVertical ();
							if (showRunSwaySettings) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Run Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Run Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showRunSwaySettings = !showRunSwaySettings;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							objectToUse.FindProperty ("showRunSwaySettings").boolValue = showRunSwaySettings;

							if (showRunSwaySettings) {
								showSelectedSwaySettings (objectToUse.FindProperty ("runFirstPersonSwayInfo"));
							}
							EditorGUILayout.EndVertical ();
						} else {
							showSelectedSwaySettings (objectToUse.FindProperty ("firstPersonSwayInfo"));
						}
					}

					EditorGUILayout.Space ();
				}

				showWeaponIdleSettings = objectToUse.FindProperty ("showIdleSettings").boolValue;

				EditorGUILayout.BeginVertical ();
				if (showWeaponIdleSettings) {
					GUI.backgroundColor = Color.gray;
					inputListOpenedText = "Hide Idle Settings";
				} else {
					GUI.backgroundColor = buttonColor;
					inputListOpenedText = "Show Idle Settings";
				}
				if (GUILayout.Button (inputListOpenedText)) {
					showWeaponIdleSettings = !showWeaponIdleSettings;
				}
				GUI.backgroundColor = buttonColor;
				EditorGUILayout.EndVertical ();
				objectToUse.FindProperty ("showIdleSettings").boolValue = showWeaponIdleSettings;
				if (showWeaponIdleSettings) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("First Person Weapon Idle Settings", "window", GUILayout.Height (30));
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("useWeaponIdle"));
					if (objectToUse.FindProperty ("useWeaponIdle").boolValue) {
						EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToActiveWeaponIdle"));
						EditorGUILayout.PropertyField (objectToUse.FindProperty ("idlePositionAmount"));
						EditorGUILayout.PropertyField (objectToUse.FindProperty ("idleRotationAmount"));
						EditorGUILayout.PropertyField (objectToUse.FindProperty ("idleSpeed"));
						GUILayout.BeginVertical ("Weapon State", "window", GUILayout.Height (30));
						EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerMoving"));
						EditorGUILayout.PropertyField (objectToUse.FindProperty ("idleActive"));
						GUILayout.EndVertical ();
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}
			}

			showShotShakeSettings = objectToUse.FindProperty ("showShotShakeettings").boolValue;

			EditorGUILayout.BeginVertical ();
			if (showShotShakeSettings) {
				GUI.backgroundColor = Color.gray;
				inputListOpenedText = "Hide Shot Shake Settings";
			} else {
				GUI.backgroundColor = buttonColor;
				inputListOpenedText = "Show Shot Shake Settings";
			}
			if (GUILayout.Button (inputListOpenedText)) {
				showShotShakeSettings = !showShotShakeSettings;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			objectToUse.FindProperty ("showShotShakeettings").boolValue = showShotShakeSettings;

			if (showShotShakeSettings) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Shot Shake Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("sameValueBothViews"));

				EditorGUILayout.Space ();

				if (objectToUse.FindProperty ("sameValueBothViews").boolValue) {
					showShakeInfo (objectToUse.FindProperty ("thirdPersonshotShakeInfo"), false, true);
				} else {
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("useShotShakeInThirdPerson"));
					if (objectToUse.FindProperty ("useShotShakeInThirdPerson").boolValue) {
						showShakeInfo (objectToUse.FindProperty ("thirdPersonshotShakeInfo"), false, true);
					}
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("useShotShakeInFirstPerson"));
					if (objectToUse.FindProperty ("useShotShakeInFirstPerson").boolValue) {
						showShakeInfo (objectToUse.FindProperty ("firstPersonshotShakeInfo"), true, true);
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shot Camera Noise Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useShotCameraNoise"));
			if (objectToUse.FindProperty ("useShotCameraNoise").boolValue) {
				//				EditorGUILayout.PropertyField (objectToUse.FindProperty ("verticalShotCameraNoiseAmount"));
				//				EditorGUILayout.PropertyField (objectToUse.FindProperty ("horizontalShotCameraNoiseAmount"));

				GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				IKWeaponManager.verticalShotCameraNoiseAmount.x = EditorGUILayout.FloatField (IKWeaponManager.verticalShotCameraNoiseAmount.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref IKWeaponManager.verticalShotCameraNoiseAmount.x, ref IKWeaponManager.verticalShotCameraNoiseAmount.y, -2, 2);
				IKWeaponManager.verticalShotCameraNoiseAmount.y = EditorGUILayout.FloatField (IKWeaponManager.verticalShotCameraNoiseAmount.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				IKWeaponManager.horizontalShotCameraNoiseAmount.x = EditorGUILayout.FloatField (IKWeaponManager.horizontalShotCameraNoiseAmount.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref IKWeaponManager.horizontalShotCameraNoiseAmount.x, ref IKWeaponManager.horizontalShotCameraNoiseAmount.y, -2, 2);
				IKWeaponManager.horizontalShotCameraNoiseAmount.y = EditorGUILayout.FloatField (IKWeaponManager.horizontalShotCameraNoiseAmount.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showThirdPersonGizmo"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showFirstPersonGizmo"));

			if (objectToUse.FindProperty ("showThirdPersonGizmo").boolValue || objectToUse.FindProperty ("showFirstPersonGizmo").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("showHandsWaypointGizmo"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("showWeaponWaypointGizmo"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("showPositionGizmo"));
			}

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			GUILayout.EndVertical ();
		}
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showSelectedSwaySettings (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sway Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSway"));

		EditorGUILayout.Space ();

		if (list.FindPropertyRelative ("useSway").boolValue) {
			GUILayout.BeginVertical ("Sway Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionSway"));
			if (list.FindPropertyRelative ("usePositionSway").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionVertical"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionHorizontal"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionMaxAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionSmooth"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sway Rotation Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRotationSway"));
			if (list.FindPropertyRelative ("useRotationSway").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationVertical"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationHorizontal"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationSmooth"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sway Bob Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBobPosition"));
			if (list.FindPropertyRelative ("useBobPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionAmount"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sway Bob Rotation Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBobRotation"));
			if (list.FindPropertyRelative ("useBobRotation").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationVertical"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationHorizontal"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Position Clamp Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSwayPositionClamp"));
			if (list.FindPropertyRelative ("useSwayPositionClamp").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionHorizontalClamp"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionVerticalClamp"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Rotation Clamp Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSwayRotationClamp"));
			if (list.FindPropertyRelative ("useSwayRotationClamp").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationClampX"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationClampZ"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reset Position Rotation Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetSwayPositionSmooth"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetSwayRotationSmooth"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Extra Sway Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movingExtraPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionPercentageAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationPercentageAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionPercentageAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationPercentageAiming"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minMouseAmountForSway"));
			GUILayout.EndVertical ();

		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showWeaponSettings (SerializedProperty list, bool isThirdPerson)
	{
		EditorGUILayout.Space ();

		showSingleWeaponSettings = list.FindPropertyRelative ("showSingleWeaponSettings").boolValue;

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showSingleWeaponSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Show Single Weapon Settings")) {
			showSingleWeaponSettings = !showSingleWeaponSettings;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showSingleWeaponSettings").boolValue = showSingleWeaponSettings;

		if (showSingleWeaponSettings) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideCursorOnAming"));

			if (isThirdPerson) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("useLowerRotationSpeedAimedThirdPerson"));
				if (objectToUse.FindProperty ("useLowerRotationSpeedAimedThirdPerson").boolValue) {
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("verticalRotationSpeedAimedInThirdPerson"));
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("horizontalRotationSpeedAimedInThirdPerson"));
				}

			} else {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("firstPersonArms"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("canAimInFirstPerson"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("useLowerRotationSpeedAimed"));
				if (objectToUse.FindProperty ("useLowerRotationSpeedAimed").boolValue) {
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("verticalRotationSpeedAimedInFirstPerson"));
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("horizontalRotationSpeedAimedInFirstPerson"));
				}
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawWeaponMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchMovementSpeed"));

			if (isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeOneOrTwoHandWieldSpeed"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Positions", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimRecoilPosition"));

			if (!isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkRecoilPosition"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCrouchPosition"));
				if (list.FindPropertyRelative ("useCrouchPosition").boolValue) {
					
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchPosition"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchRecoilPosition"));
				}
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lowerWeaponPosition"));

			EditorGUILayout.Space ();

			if (isThirdPerson) {
				GUILayout.BeginVertical ("Deactivate IK If Not Aiming Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateIKIfNotAiming"));
				if (list.FindPropertyRelative ("deactivateIKIfNotAiming").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeWeaponOnWalkPositionBeforeDeactivateIK"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK"));

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Copy Weapon Position In Hand")) {
						IKWeaponManager.copyTransformValuesToBuffer (IKWeaponManager.weaponTransform);
					}
						
					if (GUILayout.Button ("Paste Weapon Position In Hand")) {
						IKWeaponManager.pasteTransformValuesToBuffer (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK").objectReferenceValue as Transform);
					}

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Draw Deactivate IK Weapon Path", "window", GUILayout.Height (30));
					showSimpleList (objectToUse.FindProperty ("thirdPersonWeaponInfo.deactivateIKDrawPath"));
					GUILayout.EndVertical ();
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Weapon Rotation Point Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWeaponRotationPoint"));
				if (list.FindPropertyRelative ("useWeaponRotationPoint").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponRotationPoint"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponRotationPointHolder"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponRotationPointHeadLookTarget"));

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationUpPointAmountMultiplier"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationDownPointAmountMultiplier"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationPointSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.useRotationUpClamp"));
					if (list.FindPropertyRelative ("rotationPointInfo.useRotationUpClamp").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationUpClampAmount"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.useRotationDownClamp"));
					if (list.FindPropertyRelative ("rotationPointInfo.useRotationDownClamp").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationDownClamp"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponPivotPoint"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			} else {
				GUILayout.BeginVertical ("Reload Path Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadMovement"));
				if (list.FindPropertyRelative ("useReloadMovement").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSpline"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDuration"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadLookDirectionSpeed"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}

			GUILayout.BeginVertical ("Surface Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkSurfaceCollision"));
			if (list.FindPropertyRelative ("checkSurfaceCollision").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("collisionRayDistance"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("collisionMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionRayPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideCursorOnCollision"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Run Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRunPosition"));
			if (list.FindPropertyRelative ("useRunPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("runMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("runPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideCursorOnRun"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRunSwayInfo"));
			}
			GUILayout.EndVertical ();


			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Jump Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useJumpPositions"));
			if (list.FindPropertyRelative ("useJumpPositions").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpStartPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpEndPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpStartMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpEndtMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetJumpMovementSped"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayAtJumpEnd"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Run Fov Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewFovOnRun"));
			if (list.FindPropertyRelative ("useNewFovOnRun").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeFovSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newFovOnRun"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Melee Attack Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttack"));
			if (list.FindPropertyRelative ("useMeleeAttack").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackStartMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackEndMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackEndDelay"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastDistance"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackDamageAmount"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttackShakeInfo"));
				if (list.FindPropertyRelative ("useMeleeAttackShakeInfo").boolValue) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Melee Attack Shake Settings", "window", GUILayout.Height (30));
					if (isThirdPerson) {
						showShakeInfo (objectToUse.FindProperty ("thirdPersonMeleeAttackShakeInfo"), false, false);
					} else {
						showShakeInfo (objectToUse.FindProperty ("firstPersonMeleeAttackShakeInfo"), true, false);
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyMeleeAtackForce"));
				if (list.FindPropertyRelative ("applyMeleeAtackForce").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackForceAmount"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackForceMode"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackApplyForceToVehicles"));
					if (list.FindPropertyRelative ("meleeAttackApplyForceToVehicles").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleAttackForceToVehicles"));
					}
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttackSound"));
				if (list.FindPropertyRelative ("useMeleeAttackSound").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackSurfaceSound"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackAirSound"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttackParticles"));
				if (list.FindPropertyRelative ("useMeleeAttackParticles").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackParticles"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Attachment Positions", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("hasAttachments"));
			if (list.FindPropertyRelative ("hasAttachments").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftHandMesh"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftHandEditPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentHandSpeed"));

				if (!isThirdPerson) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSightPosition"));
					if (list.FindPropertyRelative ("useSightPosition").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("sightPosition"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("sightRecoilPosition"));
					}
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentCameraPosition"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftHandParent"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("usingSecondPositionForHand"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondPositionForHand"));
			}
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Movement Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canRunWhileCarrying"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canRunWhileAiming"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewCarrySpeed"));
			if (list.FindPropertyRelative ("useNewCarrySpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCarrySpeed"));
			}

			if (!isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewAimSpeed"));
				if (list.FindPropertyRelative ("useNewAimSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("newAimSpeed"));
				}
			}
	
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Recoil Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponHasRecoil"));
			if (list.FindPropertyRelative ("weaponHasRecoil").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("recoilSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("endRecoilSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraRandomRecoil"));
				if (list.FindPropertyRelative ("useExtraRandomRecoil").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraRandomRecoilPosition"));
					if (list.FindPropertyRelative ("useExtraRandomRecoilPosition").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraRandomRecoilPosition"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraRandomRecoilRotation"));
					if (list.FindPropertyRelative ("useExtraRandomRecoilRotation").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraRandomRecoilRotation"));
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (isThirdPerson) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Quick Draw Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useQuickDrawKeepWeapon"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Hands State", "window", GUILayout.Height (30));

				handsInPosition = "NO";
				if (Application.isPlaying) {
					if (list.FindPropertyRelative ("handsInPosition").boolValue) {
						handsInPosition = "YES";
					}
				}
				GUILayout.Label ("Hands In Position To Aim\t" + handsInPosition);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw/Keep Weapon Bezier Path", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBezierCurve"));
				if (list.FindPropertyRelative ("useBezierCurve").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("spline"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("bezierDuration"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookDirectionSpeed"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw/Keep Weapon Path", "window", GUILayout.Height (30));
				showSimpleList (objectToUse.FindProperty ("thirdPersonWeaponInfo.keepPath"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Hands Info", "window", GUILayout.Height (30));
				showHandList (objectToUse.FindProperty ("thirdPersonWeaponInfo.handsInfo"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Head Look Settings", "window", GUILayout.Height (30));
				showHandList (objectToUse.FindProperty ("headLookWhenAiming"));
				if (objectToUse.FindProperty ("headLookWhenAiming").boolValue) {
					showHandList (objectToUse.FindProperty ("headLookSpeed"));
					showHandList (objectToUse.FindProperty ("headLookTarget"));
				}
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHandleForVertex"));
			if (list.FindPropertyRelative ("useHandleForVertex").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("handleRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("handleGizmoColor"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionHandle"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		showDualWeaponSettings = list.FindPropertyRelative ("showDualWeaponSettings").boolValue;

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showDualWeaponSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Show Dual Weapon Settings")) {
			showDualWeaponSettings = !showDualWeaponSettings;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showDualWeaponSettings").boolValue = showDualWeaponSettings;

		if (showDualWeaponSettings) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dual Weapon Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedAsDualWeapon"));
			if (list.FindPropertyRelative ("canBeUsedAsDualWeapon").boolValue) {
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Right Hand Weapon Settings", "window", GUILayout.Height (30));
				showDualWeaponInfo (list.FindPropertyRelative ("rightHandDualWeaopnInfo"), isThirdPerson);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Left Hand Weapon Settings", "window", GUILayout.Height (30));
				showDualWeaponInfo (list.FindPropertyRelative ("leftHandDualWeaponInfo"), isThirdPerson);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
		}
	}

	void showDualWeaponInfo (SerializedProperty list, bool isThirdPerson)
	{
		GUILayout.BeginVertical ("Weapon Positions", "window", GUILayout.Height (30));
		if (isThirdPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimRecoilPosition"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkPosition"));

		if (!isThirdPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkRecoilPosition"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchRecoilPosition"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepPosition"));

		GUILayout.EndVertical ();

		if (!isThirdPerson) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Lower Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lowerWeaponPosition"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Surface Position Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("collisionRayDistance"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionRayPosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Run Position Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("runPosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!isThirdPerson) {
			GUILayout.BeginVertical ("Jump Position Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpStartPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpEndPosition"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Melee Attack Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastPosition"));
		GUILayout.EndVertical ();    

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachment Positions", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentPosition"));
		if (isThirdPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentCameraPosition"));
		}
		GUILayout.EndVertical ();    

		if (!isThirdPerson) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Arms Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonHandMesh"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reload Path Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadMovement"));
			if (list.FindPropertyRelative ("useReloadMovement").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSpline"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDuration"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadLookDirectionSpeed"));
			}
			GUILayout.EndVertical ();
		}

		if (isThirdPerson) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Hands State", "window", GUILayout.Height (30));

			handsInPosition = "NO";
			if (Application.isPlaying) {
				if (list.FindPropertyRelative ("handsInPosition").boolValue) {
					handsInPosition = "YES";
				}
			}
			GUILayout.Label ("Hands In Position To Aim\t" + handsInPosition);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Weapon Bezier Path", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBezierCurve"));
			if (list.FindPropertyRelative ("useBezierCurve").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("spline"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bezierDuration"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookDirectionSpeed"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Weapon Path", "window", GUILayout.Height (30));
			showSimpleList (list.FindPropertyRelative ("keepPath"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Hands Info", "window", GUILayout.Height (30));
			showHandList (list.FindPropertyRelative ("handsInfo"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useQuickDrawKeepWeapon"));

			if (isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeWeaponOnKeepPositionSideBeforeDraw"));
			}
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();


		if (isThirdPerson) {
			GUILayout.BeginVertical ("Deactivate IK If Not Aiming Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeWeaponOnWalkPositionBeforeDeactivateIK"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Copy Weapon Position In Hand")) {
				IKWeaponManager.copyTransformValuesToBuffer (IKWeaponManager.weaponTransform);
			}

			if (GUILayout.Button ("Paste Weapon Position In Hand")) {
				IKWeaponManager.pasteTransformValuesToBuffer (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK").objectReferenceValue as Transform);
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw Deactivate IK Weapon Path", "window", GUILayout.Height (30));
			showSimpleList (list.FindPropertyRelative ("deactivateIKDrawPath"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}
	}

	void showHandElementInfo (SerializedProperty list)
	{
		Color listButtonBackgroundColor;
		showElbowInfo = list.FindPropertyRelative ("showElbowInfo").boolValue;

		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Hand Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limb"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("position"), new GUIContent ("IK Position"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointFollower"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handMovementSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hand Way Points", "window", GUILayout.Height (30));
//		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBezierCurve"));
//		if (list.FindPropertyRelative ("useBezierCurve").boolValue) {
//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spline"));
//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bezierDuration"));
//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookDirectionSpeed"));
//		} else {
//			EditorGUILayout.Space ();

		showSimpleList (list.FindPropertyRelative ("wayPoints"));
//		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hand Settings and State", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handUsedInWeapon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usedToDrawWeapon"));

		handInPosition = "NO";
		if (Application.isPlaying) {
			if (list.FindPropertyRelative ("handInPositionToAim").boolValue) {
				handInPosition = "YES";
			}
		}
		GUILayout.Label ("Hand In Position To Aim\t" + handInPosition);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showElbowInfo) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Elbow Settings")) {
			showElbowInfo = !showElbowInfo;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showElbowInfo").boolValue = showElbowInfo;

		if (showElbowInfo) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.Name"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.elbow"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.position"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.elbowOriginalPosition"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showHandList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			if (list.arraySize < 2) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add Hand")) {
					list.arraySize++;
				}
				if (GUILayout.Button ("Clear")) {
					list.arraySize = 0;
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showHandElementInfo (list.GetArrayElementAtIndex (i));
					}
					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
	}

	void showShakeInfo (SerializedProperty list, bool isFirstPerson, bool isShootShake)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shotForce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		if (isFirstPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePosition"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Test Shake")) {
			if (Application.isPlaying) {
				if (isShootShake) {
					IKWeaponManager.checkWeaponCameraShake ();
				} else {
					IKWeaponManager.checkMeleeAttackShakeInfo ();
				}
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif