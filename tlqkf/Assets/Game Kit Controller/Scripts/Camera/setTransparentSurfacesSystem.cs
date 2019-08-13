using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class setTransparentSurfacesSystem : MonoBehaviour
{
	public Shader transparentShader;
	public float alphaBlendSpeed = 5;
	public float alphaTransparency = 0.2f;

	public List<surfaceInfo> surfaceInfoList = new List<surfaceInfo> ();

	surfaceInfo newSurfaceInfo;
	materialInfo newMaterialInfo;

	surfaceInfo currentSurfaceToCheck;

	public void addNewSurface (GameObject newSurface)
	{
		newSurfaceInfo = new surfaceInfo ();
		newSurfaceInfo.surfaceGameObject = newSurface;
		Component[] components = newSurfaceInfo.surfaceGameObject.GetComponentsInChildren (typeof(Renderer));
		foreach (Renderer child in components) {
			if (child.GetComponent<Renderer> ().material.shader) {
				Renderer render = child.GetComponent<Renderer> ();
				for (int j = 0; j < render.materials.Length; j++) {

					newSurfaceInfo.rendererParts.Add (render);

					newMaterialInfo = new materialInfo ();
					newMaterialInfo.surfaceMaterial = render.materials [j];

					Color alpha = render.materials [j].color;

					newMaterialInfo.originalAlphaColor = alpha.a;

					newSurfaceInfo.materialInfoList.Add (newMaterialInfo);
					newSurfaceInfo.originalShader.Add (render.materials [j].shader);
					render.materials [j].shader = transparentShader;

					setAlphaValue (newMaterialInfo.alphaBlendCoroutine, newMaterialInfo.surfaceMaterial, true, render.materials [j].shader, newMaterialInfo.originalAlphaColor);
				}
			}
		}
		surfaceInfoList.Add (newSurfaceInfo);
	}

	public void removeSurface (int surfaceIndex)
	{
		currentSurfaceToCheck = surfaceInfoList [surfaceIndex];
		for (int j = 0; j < currentSurfaceToCheck.materialInfoList.Count; j++) {
			setAlphaValue (currentSurfaceToCheck.materialInfoList [j].alphaBlendCoroutine, currentSurfaceToCheck.materialInfoList [j].surfaceMaterial, 
				false, currentSurfaceToCheck.originalShader [j], currentSurfaceToCheck.materialInfoList [j].originalAlphaColor);
		}
		surfaceInfoList.RemoveAt (surfaceIndex);
	}

	public bool listContainsSurface (GameObject surfaceToCheck)
	{
		for (int i = 0; i < surfaceInfoList.Count; i++) {
			if (surfaceInfoList [i].surfaceGameObject == surfaceToCheck) {
				return true;
			}
		}
		return false;
	}

	public void addPlayerIDToSurface (int playerID, GameObject surfaceToCheck)
	{
		for (int i = 0; i < surfaceInfoList.Count; i++) {
			if (surfaceInfoList [i].surfaceGameObject == surfaceToCheck) {
				if (!surfaceInfoList [i].playerIDs.Contains (playerID)) {
					surfaceInfoList [i].playerIDs.Add (playerID);
					surfaceInfoList [i].numberOfPlayersFound++;
					return;
				}
			}
		}
	}

	public void removePlayerIDToSurface (int playerID, int surfaceIndex)
	{
		if (surfaceInfoList [surfaceIndex].playerIDs.Contains (playerID)) {
			surfaceInfoList [surfaceIndex].playerIDs.Remove (playerID);
			surfaceInfoList [surfaceIndex].numberOfPlayersFound--;
		}
	}

	public void setAlphaValue (Coroutine alphaBlendCoroutine, Material currentMaterial, bool changingToTransparent, Shader originalShader, float originalAlphaColor)
	{
		if (alphaBlendCoroutine != null) {
			StopCoroutine (alphaBlendCoroutine);
		}
		alphaBlendCoroutine = StartCoroutine (setAlphaValueCoroutine (currentMaterial, changingToTransparent, originalShader, originalAlphaColor));
	}

	IEnumerator setAlphaValueCoroutine (Material currentMaterial, bool changingToTransparent, Shader originalShader, float originalAlphaColor)
	{
		float targetValue = originalAlphaColor;
		if (changingToTransparent) {
			targetValue = alphaTransparency;
		}
		Color alpha = currentMaterial.color;
		while (alpha.a != targetValue) {
			alpha.a = Mathf.MoveTowards (alpha.a, targetValue, Time.deltaTime * alphaBlendSpeed);
			currentMaterial.color = alpha;
			yield return null;
		}

		if (!changingToTransparent) {
			currentMaterial.shader = originalShader;
		}
	}

	public void checkSurfacesToRemove ()
	{
		for (int i = 0; i < surfaceInfoList.Count; i++) {
			currentSurfaceToCheck = surfaceInfoList [i];
	
			if (currentSurfaceToCheck.numberOfPlayersFound == 0) {
				for (int j = 0; j < currentSurfaceToCheck.materialInfoList.Count; j++) {
					setAlphaValue (currentSurfaceToCheck.materialInfoList [j].alphaBlendCoroutine, currentSurfaceToCheck.materialInfoList [j].surfaceMaterial, 
						false, currentSurfaceToCheck.originalShader [j], currentSurfaceToCheck.materialInfoList [j].originalAlphaColor);
				}

				surfaceInfoList.RemoveAt (i);
				i--;
			}
		}
	}


	[System.Serializable]
	public class surfaceInfo
	{
		public GameObject surfaceGameObject;
		public List<Renderer> rendererParts = new List<Renderer> ();
		public List<Shader> originalShader = new List<Shader> ();
		public List<materialInfo> materialInfoList = new List<materialInfo> ();
		public List<int> playerIDs = new List<int> ();
		public int numberOfPlayersFound;
	}

	[System.Serializable]
	public class materialInfo
	{
		public Material surfaceMaterial;
		public Coroutine alphaBlendCoroutine;
		public float originalAlphaColor;
	}
}
