using UnityEngine;
using System.Collections;

public class simpleAnimationSystem : MonoBehaviour
{
	public string animationName;
	Animation mainAnimation;

	bool playAnimation;
	bool playingAnimation;

	void Start ()
	{
		mainAnimation = GetComponent<Animation> ();
	}

	void Update ()
	{
		if (playAnimation) {
			if (!mainAnimation.IsPlaying (animationName)) {
				if (!playingAnimation) {
					mainAnimation.Play (animationName);
					playingAnimation = true;
				} else {
					playingAnimation = false;
					playAnimation = false;
				}
			}
		}
	}

	public void playForwardAnimation ()
	{
		playAnimation = true;
		mainAnimation [animationName].speed = 1;
	}

	public void playBackwardAnimation ()
	{
		playAnimation = true;
		mainAnimation [animationName].speed = -1; 
		mainAnimation [animationName].time = mainAnimation [animationName].length;
	}
}
