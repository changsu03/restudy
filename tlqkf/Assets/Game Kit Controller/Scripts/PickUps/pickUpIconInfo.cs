using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class pickUpIconInfo
{
	public int ID;
	public string name;
	public GameObject iconObject;
	public GameObject texture;
	public GameObject target;
	public bool paused;
	public RectTransform iconRectTransform;
}
