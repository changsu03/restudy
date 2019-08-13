using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class playerOptionsEditorSystem : MonoBehaviour
{

	public List<optionInfo> optionInfoList = new List<optionInfo> ();
	optionInfo currentOptionInfo;

	void Start ()
	{
		for (int i = 0; i < optionInfoList.Count; i++) {
			if (optionInfoList [i].currentValue) {
				optionInfoList [i].scrollBar.value = 1;
			} else {
				optionInfoList [i].scrollBar.value = 0;
			}

			optionInfoList [i].startAssigned = true;
		}
	}

	public void setOptionByScrollBar (Scrollbar scrollBarToSearch)
	{
		for (int i = 0; i < optionInfoList.Count; i++) {
			currentOptionInfo = optionInfoList [i];

			if (currentOptionInfo.startAssigned && currentOptionInfo.scrollBar == scrollBarToSearch) {
				if (currentOptionInfo.scrollBar.value < 0.5f) {
					if (currentOptionInfo.scrollBar.value > 0) {
						currentOptionInfo.scrollBar.value = 0;
					}
					currentOptionInfo.optionEvent.Invoke (false);
					optionInfoList [i].currentValue = false;
				} else {
					if (currentOptionInfo.scrollBar.value < 1) {
						currentOptionInfo.scrollBar.value = 1;
					}
					currentOptionInfo.optionEvent.Invoke (true);
					optionInfoList [i].currentValue = true;
				}

				return;
			}
		}
	}

	[System.Serializable]
	public class optionInfo
	{
		public string Name;
		public eventParameters.eventToCallWithBool optionEvent;
		public Scrollbar scrollBar;
		public bool currentValue = true;

		public bool startAssigned;
	}
}
