using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class remoteEventSystem : MonoBehaviour
{
	public List<eventInfo> eventInfoList = new List<eventInfo> ();


	public void callRemoveEvent (string eventName)
	{
		for (int i = 0; i < eventInfoList.Count; i++) {
			if (eventInfoList [i].Name == eventName) {

				if (eventInfoList [i].useRegularEvent) {
					eventInfoList [i].eventToActive.Invoke ();
				}
			}
		}
	}

	public void callRemoveEventWithAmount (string eventName, float amount)
	{
		for (int i = 0; i < eventInfoList.Count; i++) {
			if (eventInfoList [i].Name == eventName) {
				if (eventInfoList [i].useAmountOnEvent) {
					eventInfoList [i].eventToActiveAmount.Invoke (amount);
				}
			}
		}
	}

	public void callRemoveEventWithBool (string eventName, bool state)
	{
		for (int i = 0; i < eventInfoList.Count; i++) {
			if (eventInfoList [i].Name == eventName) {
				if (eventInfoList [i].useBoolOnEvent) {
					eventInfoList [i].eventToActiveBool.Invoke (state);
				}
			}
		}
	}

	public void callRemoveEventWithGameObject (string eventName, GameObject objectToSend)
	{
		for (int i = 0; i < eventInfoList.Count; i++) {
			if (eventInfoList [i].Name == eventName) {
				if (eventInfoList [i].useGameObjectOnEvent) {
					eventInfoList [i].eventToActiveGameObject.Invoke (objectToSend);
				}
			}
		}
	}

	public void callRemoveEventWithTransform (string eventName, Transform transformToSend)
	{
		for (int i = 0; i < eventInfoList.Count; i++) {
			if (eventInfoList [i].Name == eventName) {
				if (eventInfoList [i].useTransformOnEvent) {
					eventInfoList [i].eventToActiveTransform.Invoke (transformToSend);
				}
			}
		}
	}

	[System.Serializable]
	public class eventInfo
	{
		public string Name;

		public bool useRegularEvent = true;
		public UnityEvent eventToActive;

		public bool useAmountOnEvent;
		public eventParameters.eventToCallWithAmount eventToActiveAmount;

		public bool useBoolOnEvent;
		public eventParameters.eventToCallWithBool eventToActiveBool;

		public bool useGameObjectOnEvent;
		public eventParameters.eventToCallWithGameObject eventToActiveGameObject;

		public bool useTransformOnEvent;
		public eventParameters.eventToCallWithTransform eventToActiveTransform;
	}
}
