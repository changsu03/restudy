using UnityEngine;
using System.Collections;

public class slowDownTarget : projectileSystem
{
	public float slowDownDuration = 6;
	public Color slowObjectsColor = Color.blue;
	[Range (0, 1)] public float slowValue = 0.2f;

	void OnTriggerEnter (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			characterDamageReceiver currentCharacterDamageReceiver = objectToDamage.GetComponent<characterDamageReceiver> ();
			if (currentCharacterDamageReceiver) {
				objectToDamage = currentCharacterDamageReceiver.character;
			} 

			slowObject currenSlowObject = objectToDamage.GetComponent<slowObject> ();
			if (currenSlowObject) {

				slowObjectsColor currentSlowObjectsColor = currenSlowObject.getObjectToCallFunction ().GetComponent<slowObjectsColor> ();
				if (currentSlowObjectsColor) {
					currentSlowObjectsColor.startSlowObject (slowObjectsColor, slowValue, slowDownDuration);
				} else {
					currenSlowObject.getObjectToCallFunction ().AddComponent<slowObjectsColor> ().startSlowObject (slowObjectsColor, slowValue, slowDownDuration);
				}
			}

			disableBullet (currentProjectileInfo.impactDisableTimer);
		}
	}
}
