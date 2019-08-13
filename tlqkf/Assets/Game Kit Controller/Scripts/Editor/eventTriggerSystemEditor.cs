using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(eventTriggerSystem))]
[CanEditMultipleObjects]
public class eventTriggerSystemEditor : Editor
{
	SerializedObject list;
	bool useSameFunctionInList;
	bool useSameDelay;
	bool triggeredByButton;
	bool useSameObjectToCall;
	eventTriggerSystem manager;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		manager = (eventTriggerSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Same Function To Call Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useSameFunctionInList"));
		useSameFunctionInList = list.FindProperty ("useSameFunctionInList").boolValue;
		if (useSameFunctionInList) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Same Function List", "window", GUILayout.Height (30));
			showSimpleList (list.FindProperty ("sameFunctionList"), "Function");
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Same Object To Call Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useSameObjectToCall"));
		useSameObjectToCall = list.FindProperty ("useSameObjectToCall").boolValue;
		if (useSameObjectToCall) {
			EditorGUILayout.PropertyField (list.FindProperty ("callThisObject"));
			if (!list.FindProperty ("callThisObject").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("sameObjectToCall"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Object To Trigger Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useObjectToTrigger"));
		if (list.FindProperty ("useObjectToTrigger").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("objectNeededToTrigger"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tag Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useTagToTrigger"));
		if (list.FindProperty ("useTagToTrigger").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("useTagList"));
			if (list.FindProperty ("useTagList").boolValue) {
				showSimpleList (list.FindProperty ("tagList"), "Tag");
			} else {
				EditorGUILayout.PropertyField (list.FindProperty ("tagNeededToTrigger"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Same Delay Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("dontUseDelay"));
		useSameDelay = list.FindProperty ("useSameDelay").boolValue;
		EditorGUILayout.PropertyField (list.FindProperty ("useSameDelay"));
		if (useSameDelay) {
			EditorGUILayout.PropertyField (list.FindProperty ("generalDelay"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger Settings", "window", GUILayout.Height (30));
		triggeredByButton = list.FindProperty ("triggeredByButton").boolValue;
		EditorGUILayout.PropertyField (list.FindProperty ("triggeredByButton"));
		if (!triggeredByButton) {
			EditorGUILayout.PropertyField (list.FindProperty ("triggerEventType"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Layer Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useLayerMask"));
		if (list.FindProperty ("useLayerMask").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("layerMask"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("justCallOnTrigger"));
		EditorGUILayout.PropertyField (list.FindProperty ("callFunctionEveryTimeTriggered"));
		if (!list.FindProperty ("callFunctionEveryTimeTriggered").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("eventTriggered"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("coroutineActive"));
		EditorGUILayout.PropertyField (list.FindProperty ("setParentToNull"));
		EditorGUILayout.PropertyField (list.FindProperty ("triggerEventAtStart"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (list.FindProperty ("triggerEventType").enumValueIndex == 0) {
			GUILayout.BeginVertical ("Event Trigger List", "window", GUILayout.Height (30));
			showList (list.FindProperty ("eventList"));
			GUILayout.EndVertical ();
		} else if (list.FindProperty ("triggerEventType").enumValueIndex == 1) {
			GUILayout.BeginVertical ("Exit Event Trigger List", "window", GUILayout.Height (30));
			showList (list.FindProperty ("exitEventList"));
			GUILayout.EndVertical ();
		} else {
			GUILayout.BeginVertical ("Enter Event Trigger List", "window", GUILayout.Height (30));
			showList (list.FindProperty ("enterEventList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Exit Event Trigger List", "window", GUILayout.Height (30));
			showList (list.FindProperty ("exitEventList"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showEventInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));

		if (!useSameObjectToCall) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToCall"));	
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventFunction"));
		if (!list.FindPropertyRelative ("useEventFunction").boolValue) {
			
			if (!useSameFunctionInList) {
				showSimpleList (list.FindPropertyRelative ("functionNameList"), "Function");

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBroadcastMessage"));	
				if (list.FindPropertyRelative ("useBroadcastMessage").boolValue) {
					showSimpleList (list.FindPropertyRelative ("broadcastMessageStringList"), "Message");
				}
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventFunction"));
		}
	
		if (!useSameDelay) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondsDelay"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendGameObject"));	
		if (list.FindPropertyRelative ("sendGameObject").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToSend"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendObjectDetected"));	

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendObjectDetectedByEvent"));	
		if (list.FindPropertyRelative ("sendObjectDetectedByEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendObjectDetected"));
		} else {
			if (list.FindPropertyRelative ("sendObjectDetected").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendObjectDetectedFunction"));
			}
		}
	
		GUILayout.EndVertical ();
	}

	void showList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Events: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showEventInfo (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("+")) {
					manager.InsertEventAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + listName + "s: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif
