using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectToOverrideSystem : MonoBehaviour
{
	public bool canBeOverriden = true;

	public GameObject objectToOverride;

	public bool canBeOverridenActive ()
	{
		return canBeOverriden;
	}

	public GameObject getObjectToOverride ()
	{
		return objectToOverride;
	}
}
