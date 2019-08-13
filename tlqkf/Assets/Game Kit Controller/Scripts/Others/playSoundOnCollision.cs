using UnityEngine;
using System.Collections;

public class playSoundOnCollision : MonoBehaviour
{

	AudioSource mainAudioSource;

	void OnCollisionEnter (Collision col)
	{
		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}
		if (mainAudioSource) {
			mainAudioSource.Play ();
		}
	}
}