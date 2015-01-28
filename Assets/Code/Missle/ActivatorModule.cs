using UnityEngine;
using System.Collections;

public class ActivatorModule : Module {

	public override void ActivateModule () {
		Invoke ("Arm", mods[0].value);
	}

	public void Arm () {
		for (int i = 0 ; i < 4 ; i++) {
			Ray ray = new Ray (transform.position, Quaternion.Euler (0,0,i * 90) * Vector3.right / 0.25f);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {
				hit.collider.SendMessage ("Arm", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
