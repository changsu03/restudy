using UnityEngine;
using System.Collections;

public class inventoryObject : MonoBehaviour
{
	public inventoryInfo inventoryObjectInfo;

	public bool useZoomRange = true;
	public float maxZoom = 30;
	public float minZoom = 100;
	public float initialZoom = 46;
}