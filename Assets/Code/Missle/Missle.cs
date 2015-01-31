﻿using UnityEngine;
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

	public void InvokedLaunch () {
		Launch (false);
	}

	public void Launch (bool fromSeperation) {
		if (!fromSeperation) BroadcastMessage ("Activate", SendMessageOptions.DontRequireReceiver);
		rigidbody.isKinematic = false;
		Platellery.OnMissleSpawned (gameObject);
		CalculateMass ();
	}

	public void CalculateMass () {
		float mass = 0;
		for (int i = 0; i < modules.Count; i++) {
			if (modules[i].moduleType != Module.Type.Structural) {
				mass += 1;
			}else{
				mass += 0.2f;
			}
			Destroy (modules[i].GetComponent<LineRenderer>());
		}
		
		if (modules.Count > 0) {
			rigidbody.mass = mass;
		}else{
			Destroy (gameObject);
		}
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

	void OnGUI () {
		if (Platellery.debugMode && Platellery.cameraController.followingMissle == transform) {
			GUI.Label (new Rect (20,300,Screen.width-20, Screen.height-300), 
			           "Mass: " + rigidbody.mass.ToString () +
			           "\nDrag: " + rigidbody.drag.ToString () + 
			           "\nAltitude: " + Planet.current.atmosphere.PositionToAltitude (transform.position).ToString () +
			           "\nSpeed: " + rigidbody.velocity.magnitude.ToString () + 
			           "\nMissles: " + Platellery.game.activeMissles.Count + " / " + Platellery.game.maxMissles.ToString () + 
			           "\nLocModules: " + modules.Count.ToString ());
		}
	}
}
