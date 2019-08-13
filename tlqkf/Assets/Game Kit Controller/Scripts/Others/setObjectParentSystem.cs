using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setObjectParentSystem : MonoBehaviour
{
	public Transform parentTransform;

	public List<Transform> childList = new List<Transform> ();

	public void setObjectsParent ()
	{
		for (int i = 0; i < childList.Count; i++) {
			childList [i].SetParent (parentTransform);
		}
	}
}
