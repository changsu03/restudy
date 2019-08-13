using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class pickUpsScreenInfo : MonoBehaviour
{
	public bool pickUpScreenInfoEnabled;
	public GameObject originalText;
	public RectTransform originalTextRectTransform;

	public float durationTimerPerText;
	public float verticalDistance;
	public float horizontalOffset;

	public bool useIconsEnabled;
	public GameObject originalIcon;
	public float iconHeight;
	public float verticalIconOffset;
	public float horizontalIconOffset;

	public bool usedByAI;
	public List<pickupScreenInfo> textList = new List<pickupScreenInfo> ();

	float lastTexTime;
	Vector3 originalTextRectTransformPosition;

	Vector2 targetPosition;

	public float currentHeight;

	void Start ()
	{
		originalTextRectTransformPosition = originalTextRectTransform.anchoredPosition;
	}

	//display in the screen the type of pick ups that the objects grabs, setting their names and amount grabbed, setting the text position and the time that
	//is visible
	void Update ()
	{
		//if there are text elements, then check the timer, and delete them
		if (textList.Count > 0) {
			if (Time.time > lastTexTime + durationTimerPerText) {
				Destroy (textList [0].pickupText.gameObject);
				if (textList [0].hasIcon) {
					Destroy (textList [0].pickupIcon.gameObject);
				}
				setPositions ();
				textList.RemoveAt (0);
				lastTexTime = Time.time;
				if (textList.Count > 0) {
					if (useIconsEnabled) {
						currentHeight = Vector2.Distance (originalTextRectTransformPosition, textList [textList.Count - 1].pickupText.anchoredPosition);
					} else {
						currentHeight -= verticalDistance;
					}
				} else {
					currentHeight = 0;
				}
			}
		}
	}

	//the player has grabbed a pick up, so display the info in the screen, instantiating a new text component
	public void recieveInfo (string info)
	{
		if (usedByAI) {
			return;
		}

		if (pickUpScreenInfoEnabled) {
			GameObject newText = (GameObject)Instantiate (originalText, originalText.transform.position, Quaternion.identity);
			RectTransform newTextRectTransform = newText.GetComponent<RectTransform> ();
			newTextRectTransform.gameObject.SetActive (true);
			newTextRectTransform.SetParent (originalText.transform.parent);
			newTextRectTransform.localScale = Vector3.one;
			if (info.Length > 12) {
				float extraPositionX = info.Length - 12;
				newTextRectTransform.sizeDelta = new Vector2 (newTextRectTransform.sizeDelta.x + extraPositionX * 20, newTextRectTransform.sizeDelta.y);
			}

			newTextRectTransform.anchoredPosition = originalTextRectTransformPosition.y * Vector2.up
			- Vector2.right * (newTextRectTransform.sizeDelta.x / 2) + Vector2.right * horizontalOffset;

			if (textList.Count > 0) {
				if (textList [textList.Count - 1].hasIcon) {
					currentHeight -= verticalIconOffset / 2;
				}
			}

			targetPosition = Vector2.up * (currentHeight);

			newTextRectTransform.anchoredPosition += targetPosition;

			currentHeight += verticalDistance;

			newText.GetComponent<Text> ().text = info;

			pickupScreenInfo newPickupScreenInfo = new pickupScreenInfo ();
			newPickupScreenInfo.pickupText = newTextRectTransform;
			newPickupScreenInfo.targetPosition = newTextRectTransform.anchoredPosition.y;

			textList.Add (newPickupScreenInfo);
		
			lastTexTime = Time.time;
		}
	}

	public void recieveInfo (string info, Texture icon)
	{
		if (usedByAI) {
			return;
		}
		if (pickUpScreenInfoEnabled) {

			if (!useIconsEnabled) {
				recieveInfo (info);
				return;
			}

			GameObject newText = (GameObject)Instantiate (originalText, originalText.transform.position, Quaternion.identity);
			RectTransform newTextRectTransform = newText.GetComponent<RectTransform> ();
			newTextRectTransform.gameObject.SetActive (true);
			newTextRectTransform.SetParent (originalText.transform.parent);
			newTextRectTransform.localScale = Vector3.one;
			if (info.Length > 12) {
				float extraPositionX = info.Length - 12;
				newTextRectTransform.sizeDelta = new Vector2 (newTextRectTransform.sizeDelta.x + extraPositionX * 20, newTextRectTransform.sizeDelta.y);
			}

			newTextRectTransform.anchoredPosition = originalTextRectTransformPosition.y * Vector2.up
			- Vector2.right * (newTextRectTransform.sizeDelta.x / 2) + Vector2.right * horizontalOffset - Vector2.right * (iconHeight + horizontalIconOffset);

			if (textList.Count > 0) {
				if (!textList [textList.Count - 1].hasIcon) {
					currentHeight += verticalIconOffset / 2;
				}
			}

			targetPosition = Vector2.up * (currentHeight);

			newTextRectTransform.anchoredPosition += targetPosition;

			currentHeight += (verticalDistance + verticalIconOffset);

			newText.GetComponent<Text> ().text = info;

			GameObject newIcon = (GameObject)Instantiate (originalIcon, originalIcon.transform.position, Quaternion.identity);
			RectTransform newIconRectTransform = newIcon.GetComponent<RectTransform> ();
			newIconRectTransform.gameObject.SetActive (true);
			newIconRectTransform.SetParent (originalIcon.transform.parent);
			newIconRectTransform.localScale = Vector3.one;

			newIconRectTransform.anchoredPosition += new Vector2 (horizontalIconOffset, 0);

			newIconRectTransform.anchoredPosition += targetPosition;
		
			newIcon.GetComponent<RawImage> ().texture = icon;

			pickupScreenInfo newPickupScreenInfo = new pickupScreenInfo ();
			newPickupScreenInfo.pickupText = newTextRectTransform;
			newPickupScreenInfo.pickupIcon = newIconRectTransform;
			newPickupScreenInfo.hasIcon = true;
			newPickupScreenInfo.targetPosition = newTextRectTransform.anchoredPosition.y;

			textList.Add (newPickupScreenInfo);
			lastTexTime = Time.time;
		}
	}

	void setPositions ()
	{
		for (int i = 0; i < textList.Count; i++) {
			if (i > 0) {
				targetPosition = textList [i - 1].targetPosition * Vector2.up;

				if (textList [i].hasIcon) {
					textList [i].pickupIcon.anchoredPosition = Vector2.right * textList [i].pickupIcon.anchoredPosition.x + targetPosition;
				}
				textList [i].pickupText.anchoredPosition = Vector2.right * textList [i].pickupText.anchoredPosition.x + targetPosition;
			}
		}

		for (int i = 0; i < textList.Count; i++) {
			textList [i].targetPosition = textList [i].pickupText.anchoredPosition.y;
		}
	}

	[System.Serializable]
	public class pickupScreenInfo
	{
		public RectTransform pickupText;
		public bool hasIcon;
		public RectTransform pickupIcon;
		public float targetPosition;
	}
}
