using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parentAssignedSystem : MonoBehaviour
{
	public GameObject parentGameObject;

	public void assignParent (GameObject newParent)
	{
		parentGameObject = newParent;
	}

	public GameObject getAssignedParent ()
	{
		return parentGameObject;
	}
}
