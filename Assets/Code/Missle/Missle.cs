using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missle : MonoBehaviour {

	public List<Module> modules;
	public Transform velTransform;

	void FixedUpdate () {
		if (!rigidbody.isKinematic) 
			rigidbody.velocity += Planet.current.GetPositionalGravity (transform.position) * Time.fixedDeltaTime;

		rigidbody.drag = Planet.current.atmosphere.GetDensity (Planet.current.atmosphere.PositionToAltitude01 (transform.position));
		if (rigidbody.velocity.magnitude > 0.1f) {
			velTransform.forward = rigidbody.velocity.normalized;
		}
	}

	public void Launch () {
		BroadcastMessage ("Activate", SendMessageOptions.DontRequireReceiver);
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
