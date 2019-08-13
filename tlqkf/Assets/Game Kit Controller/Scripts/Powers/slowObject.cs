using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class slowObject : MonoBehaviour
{
	public GameObject objectToCallFunction;

	public bool useCustomSlowSpeed;
	public float customSlowSpeed;

	public bool useMeshesToIgnore;
	public List<Transform> meshesToIgnore = new List<Transform> ();

	List<Transform> objectsToIgnoreChildren = new List<Transform> ();

	void Start ()
	{
		if (!objectToCallFunction) {
			objectToCallFunction = gameObject;
		}

		if (useMeshesToIgnore) {
			for (int i = 0; i < meshesToIgnore.Count; i++) {
				if (meshesToIgnore [i]) {
					Component[] childrens = meshesToIgnore [i].GetComponentsInChildren (typeof(Transform));
					foreach (Component c in childrens) {
						objectsToIgnoreChildren.Add (c.GetComponent<Transform> ());
					}
				}
			}
		}
	}

	public GameObject getObjectToCallFunction ()
	{
		return objectToCallFunction;
	}

	public bool isUseCustomSlowSpeedEnabled ()
	{
		return useCustomSlowSpeed;
	}

	public float getCustomSlowSpeed ()
	{
		return customSlowSpeed;
	}

	public bool checkChildsObjectsToIgnore (Transform obj)
	{
		bool value = false;
		if (meshesToIgnore.Contains (obj) || objectsToIgnoreChildren.Contains (obj)) {
			value = true;
			return value;
		}
		return value;
	}

	public bool useMeshesToIgnoreEnabled ()
	{
		return useMeshesToIgnore;
	}
}