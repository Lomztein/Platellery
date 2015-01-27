using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missle : MonoBehaviour {

	public List<Module> modules;

	void Start () {

	}

	// Update is called once per frame
	void Update () {
		// if (modules.Count == 0) Destroy (gameObject);
	}

	public void Launch () {
		BroadcastMessage ("Activate");
		rigidbody.isKinematic = false;
	}

	public void SeperateSeperators () {
		for (int i = 0; i < modules.Count; i++) {
			// Get each seperator, and seperate.
		}
	}

	void OnCollisionEnter (Collision col) {
		col.contacts[0].thisCollider.SendMessage ("Collide", col);
	}
}
