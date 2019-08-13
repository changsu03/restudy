using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class vendingMachine : MonoBehaviour
{
	public GameObject objectToSpawn;
	public Transform spawnPosition;
	public float radiusToSpawn;

	public bool spawnObjectList;
	public List<GameObject> objectListToSpawn = new List<GameObject> ();

	public bool showGizmo;

	//simple script to spawn vehicles in the scene, or other objects
	public void getObject ()
	{
		if (spawnObjectList) {
			for (int i = 0; i < objectListToSpawn.Count; i++) {
				spawnObject (objectListToSpawn[i]);
			}

		} else {
			spawnObject (objectToSpawn);
		}
	}

	public void spawnObject (GameObject newObject)
	{
		Vector3 positionToSpawn = spawnPosition.position;
		if (radiusToSpawn > 0) {
			Vector2 circlePosition = Random.insideUnitCircle * radiusToSpawn;
			Vector3 newSpawnPosition = new Vector3 (circlePosition.x, 0, circlePosition.y);
			positionToSpawn += newSpawnPosition;
		}

		GameObject objectToSpawnClone = (GameObject)Instantiate (newObject, positionToSpawn, spawnPosition.rotation);
		objectToSpawnClone.name = objectToSpawn.name;
	}

	void OnDrawGizmos ()
	{
		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (spawnPosition.position, radiusToSpawn);
		}
	}
}