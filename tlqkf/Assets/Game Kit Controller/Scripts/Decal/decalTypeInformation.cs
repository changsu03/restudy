using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class decalTypeInformation : MonoBehaviour
{
	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool getImpactListEveryFrame;

	public bool parentScorchOnThisObject;

	public bool showGizmo;
	public Color gizmoLabelColor;

	decalManager impactDecalManager;

	public void getImpactListInfo ()
	{
		if (!impactDecalManager) {
			impactDecalManager = FindObjectOfType<decalManager> ();
		} 
		if (impactDecalManager) {
			impactDecalList = new string[impactDecalManager.impactListInfo.Count + 1];
			for (int i = 0; i < impactDecalManager.impactListInfo.Count; i++) {
				string name = impactDecalManager.impactListInfo [i].name;
				impactDecalList [i] = name;
			}
			updateComponent ();
		}
	}

	public int getDecalImpactIndex ()
	{
		return impactDecalIndex;
	}

	public bool isParentScorchOnThisObjectEnabled ()
	{
		return parentScorchOnThisObject;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<decalTypeInformation> ());
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		if (!Application.isPlaying && getImpactListEveryFrame) {
			getImpactListInfo ();
		}
	}
}
