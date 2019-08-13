using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class powersAndAbilitiesSystem : MonoBehaviour
{
	public List<powerInfo> powerInfoList = new List<powerInfo> ();

	public void disableGeneralPower (string powerName)
	{
		for (int i = 0; i < powerInfoList.Count; i++) {
			if (powerInfoList [i].Name == powerName) {
				powerInfoList [i].eventToDisable.Invoke ();
			}
		}
	}

	public void enableGeneralPower (string powerName)
	{
		for (int i = 0; i < powerInfoList.Count; i++) {
			if (powerInfoList [i].Name == powerName) {
				powerInfoList [i].eventToEnable.Invoke ();
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<powersAndAbilitiesSystem> ());
		#endif
	}

	[System.Serializable]
	public class powerInfo
	{
		public string Name;
		public UnityEvent eventToEnable;
		public UnityEvent eventToDisable;
	}
}
