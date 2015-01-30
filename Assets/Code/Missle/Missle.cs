using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missle : MonoBehaviour {

	public List<Module> modules;
	public Transform velTransform;
	public bool inEditor = true;

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
		Platellery.OnMissleSpawned (gameObject);

		float mass = 0;
		for (int i = 0; i < modules.Count; i++) {
			if (modules[i].moduleType != Module.Type.Structural) {
				mass += 1;
			}else{
				mass += 0.2f;
			}
			Destroy (modules[i].GetComponent<LineRenderer>());
		}

		if (modules.Count > 0) rigidbody.mass = mass;
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

	void OnDestroy () {
		Platellery.game.activeMissles.Remove (gameObject);
	}
}
