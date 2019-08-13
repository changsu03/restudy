using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class CharacterCreatorEditor : EditorWindow
{
	public characterType charType = characterType.Player;

	public AICharacterType AIType = AICharacterType.Armed;

	public bool deletePreviousPlayer = true;

	public bool usePreviousPlayerFrefab;

	public GameObject previousPlayerPrefab;

	public enum characterType
	{
		Player,
		Enemy,
		Friend
	}

	public enum AICharacterType
	{
		Armed,
		Combat,
		Unarmed
	}

	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;
	GameObject currentCharacterGameObject;
	Animator characterAnimator;
	Vector2 rectSize = new Vector2 (510, 600);
	Vector2 previewSize = new Vector2 (100, 400);
	Editor characterPreview;
	bool modelIsHumanoid;
	bool correctAnimatorAvatar;
	bool characterSelected;
	GameObject character;

	bool characterCreated;

	GameObject newCharacterModel;

	float timeToBuild = 0.2f;
	float timer;
	string assetsPath;
	string butonText;

	string prefabsPath = "Assets/Game Kit Controller/Prefabs/";

	string playerPrefabPath = "Player Controller/";
	string playerPrefabName = "Player And Game Management";

	string friendPrefabPath = "AI/Friend/";
	string friendArmedPrefabName = "AI Friend Armed";
	string friendCombatPrefabName = "AI Friend Combat";
	string friendUnarmedPrefabName = "AI Friend Unarmed";

	string enemyPrefabPath = "AI/Enemy/";
	string enemyArmedPrefabName = "AI Enemy Armed";
	string enemyCombatPrefabName = "AI Enemy Combat";
	string enemyUnarmedPrefabName = "AI Enemy Unarmed";

	bool characterIsPlayerType;
	bool hasWeaponsEnabled;
	bool isInstantiated = true;

	bool usePrefabsManager = true;

	bool characterCheckResult;

	bool buttonPressed;

	Transform head;
	Transform neck;
	Transform chest;
	Transform spine;

	Transform hips;

	Transform rightLowerArm;
	Transform leftLowerArm;
	Transform rightHand;
	Transform leftHand;

	Transform rightLowerLeg;
	Transform leftLowerLeg;
	Transform rightFoot;
	Transform leftFoot;
	Transform rightToes;
	Transform leftToes;

	[MenuItem ("Game Kit Controller/Create New Character")]
	public static void createNewPlayer ()
	{
		GetWindow<CharacterCreatorEditor> ();
	}

	void OnEnable ()
	{
		assetsPath = Application.dataPath;
		loadAllAssets (assetsPath);
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;
		this.titleContent = new GUIContent ("Character", null, "Game Kit Controller Character Creator");
		GUILayout.BeginVertical ("Character Creator Window", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("box");

		charType = (characterType)EditorGUILayout.EnumPopup ("Character Type", charType);        

		if (charType == characterType.Player) {
			currentEvent = Event.current;
			windowRect = GUILayoutUtility.GetLastRect ();
			windowRect.position = new Vector2 (0, windowRect.position.y);
			windowRect.width = this.maxSize.x;
			if (windowRect.Contains (currentEvent.mousePosition)) {
				loadAllAssets (assetsPath);
			}
		} else {
			AIType = (AICharacterType)EditorGUILayout.EnumPopup ("AI Type", AIType);       
		}

		if (buttonPressed) {
			if (!characterCheckResult) {
				EditorGUILayout.HelpBox ("The character hasn't a standard skeleton, check the console", MessageType.Error);
			}

			if (currentCharacterGameObject == null) {
				buttonPressed = false;
			}
		} else {
			if (!currentCharacterGameObject) {
				EditorGUILayout.HelpBox ("The FBX model needs to be humanoid", MessageType.Info);
			} else if (!characterSelected) {
				EditorGUILayout.HelpBox ("The object needs an animator component", MessageType.Error);
			} else if (!modelIsHumanoid) {
				EditorGUILayout.HelpBox ("The model is not humanoid", MessageType.Error);
			} else if (!correctAnimatorAvatar) {
				EditorGUILayout.HelpBox (currentCharacterGameObject.name + " is not a valid humanoid", MessageType.Info);
			}
		}

		currentCharacterGameObject = EditorGUILayout.ObjectField ("FBX Model", currentCharacterGameObject, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
		if (charType == characterType.Player) {
			deletePreviousPlayer = (bool)EditorGUILayout.Toggle ("Remove Previous Player", deletePreviousPlayer);
			if (!deletePreviousPlayer) {
				usePreviousPlayerFrefab = (bool)EditorGUILayout.Toggle ("Use Current Settings", usePreviousPlayerFrefab);
				if (usePreviousPlayerFrefab) {
					previousPlayerPrefab = EditorGUILayout.ObjectField ("Previous Prefab", previousPlayerPrefab, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
				}
			}
		}

		if (GUI.changed && currentCharacterGameObject != null) {
			characterPreview = Editor.CreateEditor (currentCharacterGameObject);
		}

		GUILayout.EndVertical ();
		if (currentCharacterGameObject) {
			characterAnimator = currentCharacterGameObject.GetComponent<Animator> ();
		} else {
			characterAnimator = null;
		}

		if (characterAnimator != null) {
			characterSelected = true;
		} else {
			characterSelected = false;
		}

		if (characterSelected && characterAnimator.isHuman) {
			modelIsHumanoid = true;
		} else {
			modelIsHumanoid = false;
		}

		if (characterSelected && characterAnimator.avatar.isValid) {
			correctAnimatorAvatar = true;
		} else {
			correctAnimatorAvatar = false;
		}

		if (currentCharacterGameObject) {
			GUILayout.FlexibleSpace ();
			if (characterPreview != null) {
				characterPreview.OnInteractivePreviewGUI (GUILayoutUtility.GetRect (previewSize.x, previewSize.y), "window");
			}
		}

		if (correctAnimatorAvatar && modelIsHumanoid) {
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (charType == characterType.Player) {
				butonText = "Create Player";
			} else if (charType == characterType.Enemy) {
				butonText = "Create Enemy";
			} else if (charType == characterType.Friend) {
				butonText = "Create Friend";
			}

			if (buttonPressed) {
				butonText = "Reset Character Creator";
			}

			if (GUILayout.Button (butonText)) {
				if (!buttonPressed) {
					characterCheckResult = checkCreateCharacter ();

					if (characterCheckResult) {
						createCharacter ();
					}

					if (temporalCharacterObject != null) {
						DestroyImmediate (temporalCharacterObject);
					}

					if (!characterCheckResult) {
						buttonPressed = true;
					}
				} else {
					currentCharacterGameObject = null;
				}
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		}
		GUILayout.EndVertical ();
	}

	void loadAllAssets (string path)
	{
		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}

		DirectoryInfo directoryInfo = new DirectoryInfo (path);
		DirectoryInfo[] directoryInfoList = directoryInfo.GetDirectories ("*", SearchOption.TopDirectoryOnly);
		FileInfo[] fileInfoList = directoryInfo.GetFiles ("*.asset", SearchOption.TopDirectoryOnly);

		foreach (FileInfo fileInfo in fileInfoList) {
			var dir = @fileInfo.FullName;
			var filePath = dir.Remove (0, path.ToString ().Length);
		}

		foreach (DirectoryInfo directory in directoryInfoList) {
			loadAllAssets (directory.FullName);
		}
	}

	bool checkCreateCharacter ()
	{
		getCharacterBones ();

		if (!checkAllBonesFound ()) {

			string skeletonMessage = "WARNING: Not all bones have been found on this model: ";
			if (head == null) {
				skeletonMessage += "\n head not found";
			}

			if (neck == null) {
				skeletonMessage += "\n neck not found";
			}

			if (chest == null) {
				skeletonMessage += "\n chest not found";
			}

			if (spine == null) {
				skeletonMessage += "\n spine not found";
			}

			if (hips == null) {
				skeletonMessage += "\n hips not found";
			}

			if (rightLowerArm == null) {
				skeletonMessage += "\n righ lower arm not found";
			}

			if (leftLowerArm == null) {
				skeletonMessage += "\n left lower arm not found";
			}

			if (rightHand == null) {
				skeletonMessage += "\n right hand not found";
			}

			if (leftHand == null) {
				skeletonMessage += "\n left hand not found";
			}

			if (rightLowerLeg == null) {
				skeletonMessage += "\n right lower leg not found";
			}

			if (leftLowerLeg == null) {
				skeletonMessage += "\n left lower leg not found";
			}

			if (rightFoot == null) {
				skeletonMessage += "\n right foot not found";
			}

			if (leftFoot == null) {
				skeletonMessage += "\n left foot not found";
			}

			if (rightToes == null) {
				skeletonMessage += "\n right toes not found";
			}

			if (leftToes == null) {
				skeletonMessage += "\n left toes not found";
			}

			Debug.Log (skeletonMessage);

			return false;
		}

		return true;
	}

	void createCharacter ()
	{
		if (charType == characterType.Player) {
			GameObject previousCharacter = GameObject.Find (playerPrefabName);

			if (previousCharacter == null) {
				inputManager currentInputManager = FindObjectOfType<inputManager> ();

				if (currentInputManager) {
					previousCharacter = currentInputManager.gameObject;
				}
			}

			string prefabPath = "";

			if (usePrefabsManager) {
				prefabsManager currentPrefabsManager = FindObjectOfType<prefabsManager> ();
				if (currentPrefabsManager) {
					prefabPath = currentPrefabsManager.getPrefabPath ("Player", "Player");
				} else {
					prefabPath = prefabsPath + playerPrefabPath + playerPrefabName;
				}
			} else {
				prefabPath = prefabsPath + playerPrefabPath + playerPrefabName;
			}

			prefabPath += ".prefab";
			character = (GameObject)AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));

			bool instantiatePlayerGameObject = true;
			if (previousCharacter) {
				if (deletePreviousPlayer) {
					DestroyImmediate (previousCharacter);
				}
			}

			if (usePreviousPlayerFrefab) {
				character = previousPlayerPrefab;
				instantiatePlayerGameObject = false;
				isInstantiated = false;
			}

			if (character) {
				createCharacterGameObject (playerPrefabName, instantiatePlayerGameObject);
				characterIsPlayerType = true;
				hasWeaponsEnabled = true;
			} else {
				Debug.Log ("Player prefab not found in path " + prefabPath);
			}
		} else if (charType == characterType.Enemy) {
			string prefabPath = prefabsPath + enemyPrefabPath + enemyArmedPrefabName;
			string characterName = enemyArmedPrefabName;

			if (AIType == AICharacterType.Armed) {
				hasWeaponsEnabled = true;
			} else if (AIType == AICharacterType.Combat) {
				prefabPath = prefabsPath + enemyPrefabPath + enemyCombatPrefabName;
				characterName = enemyCombatPrefabName;
			} else if (AIType == AICharacterType.Unarmed) {
				prefabPath = prefabsPath + enemyPrefabPath + enemyUnarmedPrefabName;
				characterName = enemyUnarmedPrefabName;
			}

			prefabPath += ".prefab";
			character = (GameObject)AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));

			if (character) {
				createCharacterGameObject (characterName, true);
			} else {
				Debug.Log ("Enemy prefab not found in path " + prefabPath);
			}
		} else if (charType == characterType.Friend) {
			string prefabPath = prefabsPath + friendPrefabPath + friendArmedPrefabName;
			string characterName = friendArmedPrefabName;

			if (AIType == AICharacterType.Armed) {
				hasWeaponsEnabled = true;
			} else if (AIType == AICharacterType.Combat) {
				prefabPath = prefabsPath + friendPrefabPath + friendCombatPrefabName;
				characterName = friendCombatPrefabName;
			} else if (AIType == AICharacterType.Unarmed) {
				prefabPath = prefabsPath + friendPrefabPath + friendUnarmedPrefabName;
				characterName = friendUnarmedPrefabName;
			}

			prefabPath += ".prefab";
			character = (GameObject)AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));

			if (character) {
				createCharacterGameObject (characterName, true);
			} else {
				Debug.Log ("Friend prefab not found in path " + prefabPath);
			}
		}
	}

	void Update ()
	{
		if (characterCreated) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					character.GetComponentInChildren<buildPlayer> ().setCharacterVariables (characterIsPlayerType, hasWeaponsEnabled, isInstantiated, newCharacterModel);
					character.GetComponentInChildren<buildPlayer> ().buildCharacter ();
					characterCreated = false;
					timer = 0;

					if (characterCheckResult) {
						Debug.Log ("Character has a standard skeleton, all bones were found properly");

						this.Close ();
					}
				}
			}
		}
	}

	public void createCharacterGameObject (string name, bool instantiateGameObject)
	{
		if (instantiateGameObject) {
			character = (GameObject)Instantiate (character, Vector3.zero, Quaternion.identity);
		}

		character.name = name;
		Transform playerCOM = character.GetComponentInChildren<IKSystem> ().IKBodyCOM;
		GameObject previousModel = playerCOM.GetChild (0).gameObject;

		GameObject newModel = GameObject.Instantiate (currentCharacterGameObject, previousModel.transform.position, previousModel.transform.rotation) as GameObject;
		newCharacterModel = newModel;
		newModel.name = currentCharacterGameObject.name;

		characterAnimator = newModel.GetComponentInChildren<Animator> ();

		character.GetComponentInChildren<Animator> ().avatar = characterAnimator.avatar;
		characterCreated = true;
	}

	GameObject temporalCharacterObject;

	public void getCharacterBones ()
	{
		temporalCharacterObject = GameObject.Instantiate (currentCharacterGameObject, Vector3.zero, Quaternion.identity) as GameObject;

		characterAnimator = temporalCharacterObject.GetComponentInChildren<Animator> ();

		if (characterAnimator == null) {
			Debug.Log ("WARNING: There is not an animator component in this model, make sure to attach that component before create a new character");
			return;
		}

		head = characterAnimator.GetBoneTransform (HumanBodyBones.Head);
		neck = characterAnimator.GetBoneTransform (HumanBodyBones.Neck);
		if (neck == null) {
			if (head != null) {
				neck = head.parent;
			} else {
				Debug.Log ("WARNING: no head found, assign it manually to make sure all of them are configured correctly");
			}
		}	

		chest = characterAnimator.GetBoneTransform (HumanBodyBones.Chest);
		spine = characterAnimator.GetBoneTransform (HumanBodyBones.Spine);

		if (spine) {
			if (chest) {
				if (spine != chest.parent) {
					spine = chest.parent;
				}
			} else {
				Debug.Log ("WARNING: no chest found, assign it manually to make sure all of them are configured correctly");
			}
		} else {
			Debug.Log ("WARNING: no spine found, assign it manually to make sure all of them are configured correctly");
		}

		hips = characterAnimator.GetBoneTransform (HumanBodyBones.Hips);

		rightLowerArm = characterAnimator.GetBoneTransform (HumanBodyBones.RightLowerArm);
		leftLowerArm = characterAnimator.GetBoneTransform (HumanBodyBones.LeftLowerArm);
		rightHand = characterAnimator.GetBoneTransform (HumanBodyBones.RightHand);
		leftHand = characterAnimator.GetBoneTransform (HumanBodyBones.LeftHand);

		rightLowerLeg = characterAnimator.GetBoneTransform (HumanBodyBones.RightLowerLeg);
		leftLowerLeg = characterAnimator.GetBoneTransform (HumanBodyBones.LeftLowerLeg);
		rightFoot = characterAnimator.GetBoneTransform (HumanBodyBones.RightFoot);
		leftFoot = characterAnimator.GetBoneTransform (HumanBodyBones.LeftFoot);
		rightToes = characterAnimator.GetBoneTransform (HumanBodyBones.RightToes);
		leftToes = characterAnimator.GetBoneTransform (HumanBodyBones.LeftToes);
	}

	public bool checkAllBonesFound ()
	{
		return 
			(head != null) &&
		(neck != null) &&
		(chest != null) &&
		(spine != null) &&
		(hips != null) &&
		(rightLowerArm != null) &&
		(leftLowerArm != null) &&
		(rightHand != null) &&
		(leftHand != null) &&
		(rightLowerLeg != null) &&
		(leftLowerLeg != null) &&
		(rightFoot != null) &&
		(leftFoot != null) &&
		(rightToes != null) &&
		(leftToes != null);
	}
}