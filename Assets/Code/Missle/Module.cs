using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Module : MonoBehaviour {

	public Missle missle;
	public Module parentModule;
	public List<Module> childModules;

	public float collisionTolerance;
	public float aeroDynamicness;

	public bool isActive;

	// Use this for initialization
	void Start () {
		ModuleStart ();
	}

	public virtual void ModuleStart () {
	}

	public void Activate () {
		isActive = true;
	}

	void Collide (Collision col) {
		if (col.relativeVelocity.magnitude > collisionTolerance && isActive) {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (isActive) ModuleFixedUpdate ();
	}

	public virtual void ModuleFixedUpdate () {
	}

	void OnCreate () {
		missle.modules.Add (this);
	}
		
	void OnDestroy () {
		missle.modules.Remove (this);
	}
}
