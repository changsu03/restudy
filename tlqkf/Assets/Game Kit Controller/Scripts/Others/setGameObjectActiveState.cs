using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setGameObjectActiveState : MonoBehaviour
{
	public bool setInitialState;
	public bool initialState;

	public bool objectActive;

	public GameObject objectToActive;

	void Start ()
	{
		if (objectToActive == null) {
			objectToActive = gameObject;
		}
			
		if (objectToActive.activeSelf) {
			objectActive = true;
		}

		if (setInitialState) {
			setActiveState (initialState);
		}
	}

	public void changeActiveState ()
	{
		objectActive = !objectActive;

		setActiveState (objectActive);
	}

	public void setActiveState (bool state)
	{
		objectActive = state;
		objectToActive.SetActive (state);
	}
}
