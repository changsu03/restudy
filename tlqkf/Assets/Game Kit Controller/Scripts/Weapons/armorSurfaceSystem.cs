using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorSurfaceSystem : MonoBehaviour
{
	public bool armorActive = true;
	public GameObject armorOwner;

	public List<projectileSystem> projectilesStored = new List<projectileSystem> ();

	void Start ()
	{
		if (armorOwner == null) {
			armorOwner = gameObject;
		}
	}

	public void addProjectile (projectileSystem newProjectile)
	{
		if (!projectilesStored.Contains (newProjectile)) {
			projectilesStored.Add (newProjectile);
		}
	}

	public void throwProjectilesStored (Vector3 throwDirection)
	{
		for (int i = 0; i < projectilesStored.Count; i++) {
			projectilesStored [i].returnBullet (throwDirection, armorOwner);
		}
	}

	public bool isArmorEnabled ()
	{
		return armorActive;
	}
}
