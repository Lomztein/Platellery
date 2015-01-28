using UnityEngine;
using System.Collections;

public class SeperatorModule : Module {

	public float seperationForce;

	public override void ActivateModule () {

		if (isActive) {
			Debug.Log ("Seperating in " + mods[0].value.ToString () + " seconds.");
			Invoke ("Seperate", mods[0].value);
		}

	}

	void Seperate () {

		for (int i = 0; i < childModules.Count; i++) {

			Vector3 dir = childModules[i].transform.position - transform.position;
			childModules[i].SeperateFromHere ();
			childModules[i].missle.rigidbody.AddForceAtPosition (dir.normalized * seperationForce, childModules[i].transform.position);
			childModules[i].SendMessage ("Arm",SendMessageOptions.DontRequireReceiver);

		}
	}
}
