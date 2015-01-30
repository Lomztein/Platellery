using UnityEngine;
using System.Collections;

public class SeperatorModule : Module {

	public float seperationForce;
	private bool isUsed;

	public override void ActivateModule () {

		if (isActive) {
			Invoke ("Seperate", mods[0].value);
		}

	}

	void Arm () {
		Seperate ();
	}

	void Seperate () {

		if (!isUsed) {

			isUsed = true;
			for (int i = 0; i < childModules.Count; i++) {

				Vector3 dir = childModules[i].transform.position - transform.position;
				childModules[i].SendMessage ("Arm",SendMessageOptions.DontRequireReceiver);
				childModules[i].SeperateFromHere ();
				childModules[i].missle.rigidbody.AddForceAtPosition (dir.normalized * seperationForce, childModules[i].transform.position);
			}
		}
	}
}
