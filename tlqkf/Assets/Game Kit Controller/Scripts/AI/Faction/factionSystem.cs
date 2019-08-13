using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class factionSystem : MonoBehaviour
{
	public List<factionInfo> factionList = new List<factionInfo> ();

	public string[] factionStringList;
	public bool removingFaction;


	public List<characterFactionManager> characterFactionManagerList = new List<characterFactionManager> ();

	public void addCharacterToList (characterFactionManager character)
	{
		characterFactionManagerList.Add (character);
	}

	public void removeCharacterToList (characterFactionManager character)
	{
		characterFactionManagerList.Remove (character);
	}

	public void addFaction ()
	{
		factionInfo newFactionInfo = new factionInfo ();
		newFactionInfo.Name = "New Faction";
		factionList.Add (newFactionInfo);

		getFactionStringList ();

		if (factionList.Count > 1) {
			for (int i = 0; i < factionList.Count; i++) {
				for (int j = 0; j < factionList.Count; j++) {
					bool containsRelation = factionContainsRelation (i, factionStringList [j]);
					//print (factionList [i].Name + " " + factionStringList [j] + " " + containsRelation);
					if (!containsRelation) {
						relationInfo newRelationInfo = new relationInfo ();
						newRelationInfo.factionName = factionStringList [j];
						newRelationInfo.factionIndex = j;
						factionList [i].relationWithFactions.Add (newRelationInfo);
					}
				}
			}
		}
		updateComponent ();
	}

	public void removeFaction (int factionIndexToRemove)
	{
		removingFaction = true;
		for (int i = 0; i < factionList.Count; i++) {
			for (int j = 0; j < factionList [i].relationWithFactions.Count; j++) {
				//print ("Current Faction " + factionList [i].Name + " compare to " + factionList [i].relationWithFactions [j].factionName + " " + factionList [factionIndexToRemove].Name);
				if (factionList [i].relationWithFactions [j].factionName == factionList [factionIndexToRemove].Name) {
					factionList [i].relationWithFactions.RemoveAt (j);
					if (j != factionList [i].relationWithFactions.Count - 1) {
						j--;
					}
				}
			}
		}

		string[] auxFactionStringList = new string[factionList.Count - 1];
		int numberOfFactions = 0;
		for (int i = 0; i < factionList.Count; i++) {
			if (i != factionIndexToRemove && numberOfFactions < (factionList.Count - 1)) {
				string name = factionList [i].Name;
				auxFactionStringList [numberOfFactions] = name;
				numberOfFactions++;
			}
		}

		for (int i = 0; i < factionList.Count; i++) {
			for (int j = 0; j < factionList [i].relationWithFactions.Count; j++) {
				for (int k = 0; k < auxFactionStringList.Length; k++) {
					//print (i + " " + j + " " + factionList [i].relationWithFactions [j].factionName + " " + auxFactionStringList [k]);
					if (factionList [i].relationWithFactions [j].factionName == auxFactionStringList [k]) {
						//print ("adjust " + factionList [i].relationWithFactions [j].factionIndex + " " + k);
						factionList [i].relationWithFactions [j].factionIndex = k;
						// (factionList [i].relationWithFactions [j].factionIndex);
					}
				}
			}
		}

		StartCoroutine (waitToRemove (factionIndexToRemove));
	}


	IEnumerator waitToRemove (int factionIndexToRemove)
	{
		yield return new WaitForSeconds (0.01f);
		factionList.Remove (factionList [factionIndexToRemove]);

		getFactionStringList ();

		removingFaction = false;
		updateComponent ();
	}

	public string[] getFactionStringList ()
	{
		factionStringList = new string[factionList.Count];
		for (int i = 0; i < factionList.Count; i++) {
			string name = factionList [i].Name;
			factionStringList [i] = name;
		}
		return factionStringList;
	}

	public bool factionContainsRelation (int factionIndex, string factionWithRelation)
	{
		if (factionList [factionIndex].Name != factionWithRelation) {
			for (int j = 0; j < factionList [factionIndex].relationWithFactions.Count; j++) {
				if (factionList [factionIndex].relationWithFactions [j].factionName == factionWithRelation) {
					return true;
				}
			}
			return false;
		} 
		return true;
	}

	public bool isCharacterFriendly (int ownFactionIndex, string characterToCheckFacionName)
	{
		if (factionList [ownFactionIndex].Name != characterToCheckFacionName) {
			for (int j = 0; j < factionList [ownFactionIndex].relationWithFactions.Count; j++) {
				if (factionList [ownFactionIndex].relationWithFactions [j].factionName == characterToCheckFacionName) {
					if (factionList [ownFactionIndex].relationWithFactions [j].relation == relationInfo.relationType.friend) {
						return true;
					} else {
						return false;
					}
				}
			}
		} else {
			return true;
		}
		return false;
	}

	public bool isCharacterEnemy (int ownFactionIndex, string characterToCheckFacionName)
	{
		if (factionList [ownFactionIndex].Name != characterToCheckFacionName) {
			for (int j = 0; j < factionList [ownFactionIndex].relationWithFactions.Count; j++) {
				if (factionList [ownFactionIndex].relationWithFactions [j].factionName == characterToCheckFacionName) {
					if (factionList [ownFactionIndex].relationWithFactions [j].relation == relationInfo.relationType.enemy) {
						return true;
					} else {
						return false;
					}
				}
			}
		} else {
			return false;
		}
		return true;
	}

	public bool isAttackerEnemy (int ownFactionIndex, string characterToCheckFacionName)
	{
		//print (factionList [ownFactionIndex].Name + " " + characterToCheckFacionName);
		if (factionList [ownFactionIndex].Name != characterToCheckFacionName) {
			for (int j = 0; j < factionList [ownFactionIndex].relationWithFactions.Count; j++) {
				if (factionList [ownFactionIndex].relationWithFactions [j].factionName == characterToCheckFacionName) {
					if (factionList [ownFactionIndex].relationWithFactions [j].relation == relationInfo.relationType.enemy) {
						return true;
					} else {
						if (factionList [ownFactionIndex].turnToEnemyIfAttack) {
							if (factionList [ownFactionIndex].turnFactionToEnemy) {
								changeFactionRelation (ownFactionIndex, characterToCheckFacionName, relationInfo.relationType.enemy);
							}
							return true;
						} else {
							return false;
						}
					}
				}
			}
		} else {
			if (factionList [ownFactionIndex].friendlyFireTurnIntoEnemies) {
				return true;
			}
			return false;
		}
		return true;
	}

	public bool isCharacterNeutral (int ownFactionIndex, string characterToCheckFacionName)
	{
		if (factionList [ownFactionIndex].Name != characterToCheckFacionName) {
			for (int j = 0; j < factionList [ownFactionIndex].relationWithFactions.Count; j++) {
				if (factionList [ownFactionIndex].relationWithFactions [j].factionName == characterToCheckFacionName) {
					if (factionList [ownFactionIndex].relationWithFactions [j].relation == relationInfo.relationType.neutral) {
						return true;
					} else {
						return false;
					}
				}
			}
		} else {
			return false;
		}
		return true;
	}

	public void changeFactionRelation (int ownFactionIndex, string otherFactionName, relationInfo.relationType relationType)
	{
		if (factionList [ownFactionIndex].Name != otherFactionName) {
			for (int j = 0; j < factionList [ownFactionIndex].relationWithFactions.Count; j++) {
				if (factionList [ownFactionIndex].relationWithFactions [j].factionName == otherFactionName) {
					factionList [ownFactionIndex].relationWithFactions [j].relation = relationType;
				}
			}
		}

		for (int i = 0; i < characterFactionManagerList.Count; i++) {
			if (characterFactionManagerList [i].factionName == factionList [ownFactionIndex].Name) {
				characterFactionManagerList [i].checkCharactersAround ();
			}
		}
	}

	public bool checkIfCharacterBelongsToFaction (string factionName, GameObject character)
	{
		characterFactionManager characterFactionToCheck = character.GetComponent<characterFactionManager> ();
		if (characterFactionToCheck) {
			if (characterFactionToCheck.factionName == factionName) {
				return true;
			}
		}
		return false;
	}

	//	public void addDetectedEnemyFromFaction (int factionIndex, GameObject enemy)
	//	{
	//		factionInfo currentfactionInfo = factionList [factionIndex];
	//		if (!currentfactionInfo.currentDetectedEnemyList.Contains (enemy)) {
	//			currentfactionInfo.currentDetectedEnemyList.Add (enemy);
	//		}
	//	}
	//
	//	public void removeDetectedEnemyFromFaction (int factionIndex, GameObject enemy)
	//	{
	//		factionInfo currentfactionInfo = factionList [factionIndex];
	//		if (currentfactionInfo.currentDetectedEnemyList.Contains (enemy)) {
	//			currentfactionInfo.currentDetectedEnemyList.Remove (enemy);
	//		}
	//	}

	public bool isCharacterDetectedAsEnemyByOtherFaction (GameObject characterToCheck)
	{
		for (int i = 0; i < characterFactionManagerList.Count; i++) {
			if (characterFactionManagerList [i].currentDetectedEnemyList.Contains (characterToCheck)) {
				return true;
			}
		}
//		for (int i = 0; i < factionList.Count; i++) {
//			if (factionList [i].currentDetectedEnemyList.Contains (characterToCheck)) {
//				return true;
//			}
//		}
		return false;
	}

	public void alertFactionOnSpotted (int ownFactionIndex, float alertCloseFactionRadius, GameObject target, Vector3 alertPosition)
	{
		for (int i = 0; i < characterFactionManagerList.Count; i++) {
			if (characterFactionManagerList [i].factionName == factionList [ownFactionIndex].Name) {
				float distance = GKC_Utils.distance (characterFactionManagerList [i].getCharacterTransform ().position, alertPosition);
				if (distance <= alertCloseFactionRadius) {
					characterFactionManagerList [i].alertFaction (target);
				}
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<factionSystem> ());
		#endif
	}

	[System.Serializable]
	public class factionInfo
	{
		public string Name;
		public bool turnToEnemyIfAttack;
		public bool turnFactionToEnemy;
		public bool friendlyFireTurnIntoEnemies;
		public List<relationInfo> relationWithFactions = new List<relationInfo> ();
	}

	[System.Serializable]
	public class relationInfo
	{
		public string factionName;
		public int factionIndex;
		public relationType relation;

		public enum relationType
		{
			friend,
			enemy,
			neutral
		}
	}
}
