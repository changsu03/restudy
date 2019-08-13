using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabbedObjectState : MonoBehaviour
{
	public GameObject currentHolder;
	public bool insideZeroGravityRoom;
	public bool grabbed;

	public zeroGravityRoomSystem currentZeroGravityRoom;

	public void setGrabbedState (bool state)
	{
		grabbed = state;
	}

	public bool isGrabbed ()
	{
		return grabbed;
	}

	public void setCurrentHolder (GameObject current)
	{
		currentHolder = current;
	}

	public GameObject getCurrentHolder ()
	{
		return currentHolder;
	}

	public void setInsideZeroGravityRoomState (bool state)
	{
		insideZeroGravityRoom = state;
	}

	public bool isInsideZeroGravityRoom ()
	{
		return insideZeroGravityRoom;
	}

	public void setCurrentZeroGravityRoom (zeroGravityRoomSystem gravityRoom)
	{
		currentZeroGravityRoom = gravityRoom;
	}

	public zeroGravityRoomSystem getCurrentZeroGravityRoom ()
	{
		return currentZeroGravityRoom;
	}

	public void checkGravityRoomState ()
	{
		if (insideZeroGravityRoom) {
			currentZeroGravityRoom.setObjectInsideState (gameObject);
		}
	}
}
