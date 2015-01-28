using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missle : MonoBehaviour {

	public List<Module> modules;

	void FixedUpdate () {
		if (!rigidbody.isKinematic) 
			rigidbody.velocity += Planet.current.GetPositionalGravity (transform.position) * Time.fixedDeltaTime;
	}

	public void Launch () {
		BroadcastMessage ("Activate", SendMessageOptions.DontRequireReceiver);
		CheckModules ();
		rigidbody.isKinematic = false;
		rigidbody.mass = Mathf.Max (1, modules.Count);
	}

	public void SeperateSeperators () {
		for (int i = 0; i < modules.Count; i++) {
			// Get each seperator, and seperate.
		}
	}

	public void CheckModules () {
		if (modules.Count == 0) Destroy (gameObject);
	}

	void OnCollisionEnter (Collision col) {
		col.contacts[0].thisCollider.SendMessage ("Collide", col, SendMessageOptions.DontRequireReceiver);
	}
}
