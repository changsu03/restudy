using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class characterFactionManager : MonoBehaviour
{
	public string factionName;
	public int factionIndex;

	public List<GameObject> currentDetectedEnemyList = new List<GameObject> ();

	public Transform characterTransform;

	public string[] factionStringList;
	public factionSystem factionManager;

	void Start ()
	{
		if (factionManager == null) {
			factionManager = FindObjectOfType<factionSystem> ();
		}

		factionManager.addCharacterToList (this);
		characterTransform = transform;
	}

	public void changeCharacterToFaction (string factionToChange)
	{
		for (int i = 0; i < factionManager.factionList.Count; i++) {
			if (factionManager.factionList [i].Name == factionToChange) {
				factionName = factionToChange;
				factionIndex = i;
			}
		}
	}

	public bool isCharacterFriendly (string characterToCheckFacionName)
	{
		return factionManager.isCharacterFriendly (factionIndex, characterToCheckFacionName);
	}

	public bool isCharacterEnemy (string characterToCheckFacionName)
	{
		return factionManager.isCharacterEnemy (factionIndex, characterToCheckFacionName);
	}

	public bool isAttackerEnemy (string characterToCheckFacionName)
	{
		return factionManager.isAttackerEnemy (factionIndex, characterToCheckFacionName);
	}

	public bool isCharacterNeutral (string characterToCheckFacionName)
	{
		return factionManager.isCharacterNeutral (factionIndex, characterToCheckFacionName);
	}

	public string getFactionName ()
	{
		return factionName;
	}

	public bool checkIfCharacterBelongsToFaction (string factionName, GameObject character)
	{
		return factionManager.checkIfCharacterBelongsToFaction (factionName, character);
	}

	public void getFactionList ()
	{
		if (!factionManager) {
			factionManager = FindObjectOfType<factionSystem> ();
		} 
		if (factionManager) {
			factionStringList = new string[factionManager.factionList.Count];
			for (int i = 0; i < factionManager.factionList.Count; i++) {
				string name = factionManager.factionList [i].Name;
				factionStringList [i] = name;
			}
			updateComponent ();
		}
	}

	public void checkCharactersAround ()
	{
		SendMessage ("checkCharactersAroundAI", SendMessageOptions.DontRequireReceiver);
	}

	public void alertFaction (GameObject target)
	{
		SendMessage ("enemyAlert", target, SendMessageOptions.DontRequireReceiver);
	}

	public string[] getFactionStringList ()
	{
		return factionStringList;
	}

	public void addDetectedEnemyFromFaction (GameObject enemy)
	{
		if (!currentDetectedEnemyList.Contains (enemy)) {
			currentDetectedEnemyList.Add (enemy);
		}
		//factionManager.addDetectedEnemyFromFaction (factionIndex, enemy);
	}

	public void removeDetectedEnemyFromFaction (GameObject enemy)
	{
		if (currentDetectedEnemyList.Contains (enemy)) {
			currentDetectedEnemyList.Remove (enemy);
		}
		//factionManager.removeDetectedEnemyFromFaction (factionIndex, enemy);
	}

	public void clearDetectedEnemyFromFaction ()
	{
		currentDetectedEnemyList.Clear ();
	}

	public bool isCharacterDetectedAsEnemyByOtherFaction (GameObject characterToCheck)
	{
		return factionManager.isCharacterDetectedAsEnemyByOtherFaction (characterToCheck);
	}

	public void removeCharacterDeadFromFaction ()
	{
		clearDetectedEnemyFromFaction ();
		factionManager.removeCharacterToList (this);
	}

	public void alertFactionOnSpotted (float alertCloseFactionRadius, GameObject target, Vector3 alertPosition)
	{
		factionManager.alertFactionOnSpotted (factionIndex, alertCloseFactionRadius, target, alertPosition);
	}

	public Transform getCharacterTransform ()
	{
		return characterTransform;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<characterFactionManager> ());
		#endif
	}
}
