using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outlineObjectSystem : MonoBehaviour
{
	public bool useOutlineEnabled = true;

	public bool outlineActive;
	public bool renderElementsStored;

	public string shaderOutlineWidthName = "_Outline";
	public string shaderOutlineColorName = "_OutlineColor";

	public bool useCustomOutlineValues;
	public float customOutlineWidth = 0.05f;
	public Color customOutlineColor = Color.yellow;

	public bool useTransparencyActive = true;
	public bool transparencyActive;

	public bool useCustomTransparencyValues;
	public float customAlphaTransparency;

	public GameObject meshParent;

	public bool ignoreParticles;

	public bool useMeshesToIgnore;
	public List<Transform> meshesToIgnore = new List<Transform> ();

	List<Renderer> rendererParts = new List<Renderer> ();
	List<Shader> originalShader = new List<Shader> ();

	Shader currentOutlineShader;
	float currentOutlineWidht;
	Color currentOutlieColor;

	List<Transform> objectsToIgnoreChildren = new List<Transform> ();

	List<playerController> playerControllerList = new List<playerController> ();

	void Start ()
	{
		if (meshParent == null) {
			meshParent = gameObject;
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

	public void setOutlineState (bool state, Shader shaderToApply, float shaderOutlineWidth, Color shaderOutlineColor, playerController newPlayerToCheck)
	{
		outlineActive = state;

		if (!useOutlineEnabled) {
			return;
		}

		if (outlineActive) {
			storeRenderElements ();
		
			for (int i = 0; i < rendererParts.Count; i++) {
				if (rendererParts [i]) {
					for (int j = 0; j < rendererParts [i].materials.Length; j++) {
						rendererParts [i].materials [j].shader = shaderToApply;
						if (useCustomOutlineValues) {
							rendererParts [i].materials [j].SetFloat (shaderOutlineWidthName, customOutlineWidth);
							rendererParts [i].materials [j].SetColor (shaderOutlineColorName, customOutlineColor);
						} else {
							rendererParts [i].materials [j].SetFloat (shaderOutlineWidthName, shaderOutlineWidth);
							rendererParts [i].materials [j].SetColor (shaderOutlineColorName, shaderOutlineColor);
						}
					}
				}
			}

			currentOutlineShader = shaderToApply;
			currentOutlineWidht = shaderOutlineWidth;
			currentOutlieColor = shaderOutlineColor;

			if (newPlayerToCheck != null && !playerControllerList.Contains (newPlayerToCheck)) {
				playerControllerList.Add (newPlayerToCheck);
			}
		} else {

			if (playerControllerList.Contains (newPlayerToCheck)) {
				playerControllerList.Remove (newPlayerToCheck);
			}

			if (playerControllerList.Count == 0) {
				for (int i = 0; i < rendererParts.Count; i++) {
					if (rendererParts [i]) {
						for (int j = 0; j < rendererParts [i].materials.Length; j++) {
							rendererParts [i].materials [j].shader = originalShader [i];
						}
					}
				}
			}
		}
	}

	public bool isOutlineActive ()
	{
		return outlineActive;
	}

	public GameObject getMeshParent ()
	{
		return meshParent;
	}

	public void storeRenderElements ()
	{
		if (!renderElementsStored) {
			renderElementsStored = true;

			Component[] components = meshParent.GetComponentsInChildren (typeof(Renderer));
			foreach (Renderer child in components) {
				if (!ignoreParticles || !child.GetComponent<ParticleSystem> ()) {
					if (child.GetComponent<Renderer> ().material.shader) {
						if (!useMeshesToIgnore || !checkChildsObjectsToIgnore (child.transform)) {
							Renderer render = child.GetComponent<Renderer> ();
							for (int i = 0; i < render.materials.Length; i++) {
								rendererParts.Add (render);
								originalShader.Add (render.materials [i].shader);
							}
						}
					}
				}
			}
		}
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

	public void disableOutlineAndRemoveUsers ()
	{
		useOutlineEnabled = false;

		outlineActive = false;
		transparencyActive = false;

		useTransparencyActive = false;

		playerControllerList.Clear ();
		setOutlineState (false, null, 0, Color.white, null);
	}

	public void setTransparencyState (bool state, Shader shaderToApply, float alphaTransparency)
	{
		transparencyActive = state;

		if (!useTransparencyActive) {
			return;
		}

		if (transparencyActive) {

			storeRenderElements ();

			for (int i = 0; i < rendererParts.Count; i++) {
				if (rendererParts [i]) {
					for (int j = 0; j < rendererParts [i].materials.Length; j++) {
						rendererParts [i].materials [j].shader = shaderToApply;
						Color alpha = rendererParts [i].materials [j].color;
						if (useCustomTransparencyValues) {
							alpha.a = customAlphaTransparency;
						} else {
							alpha.a = alphaTransparency;
						}
						rendererParts [i].materials [j].color = alpha;
					}
				}
			}
		} else {
			if (outlineActive) {
				setOutlineState (true, currentOutlineShader, currentOutlineWidht, currentOutlieColor, null);
			} else {
				for (int i = 0; i < rendererParts.Count; i++) {
					if (rendererParts [i]) {
						for (int j = 0; j < rendererParts [i].materials.Length; j++) {
							rendererParts [i].materials [j].shader = originalShader [i];
						}
					}
				}
			}
		}
	}

	public bool isTransparencyActive ()
	{
		return transparencyActive;
	}
}
