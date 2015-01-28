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

	public LineRenderer parentLine;

	public ModuleMod[] mods;

	// Use this for initialization
	void Start () {
		if (parentModule) {
			parentLine = GetComponent<LineRenderer>();
			parentLine.SetPosition (0, transform.position + Vector3.back);
			parentLine.SetPosition (1, parentModule.transform.position + Vector3.back);
			parentLine.SetWidth (0.25f, 0.25f);
		}
	}

	public virtual void ModuleStart () {
	}

	public virtual void DetonateModule () {
	}

	public virtual void ActivateModule () {
	}

	public void Activate () {
		isActive = true;
		ActivateModule ();
	}

	void Collide (Collision col) {
		if (col.relativeVelocity.magnitude > collisionTolerance && isActive) {
			Die ();
		}
	}

	void FixedUpdate () {
		if (isActive) ModuleFixedUpdate ();

		// float aeroAngle = Angle.CalculateRelativeAngle (transform, transform.position + missle.rigidbody.velocity) - transform.eulerAngles.z;
	}

	public virtual void ModuleFixedUpdate () {
	}

	void OnCreate () {
		missle.modules.Add (this);
	}

	public void Die () {

		DetonateModule ();
		missle.modules.Remove (this);
		if (parentModule) parentModule.childModules.Remove (this);
		
		for (int i = 0; i < childModules.Count ; i++) {
			if (childModules[i]) {
				childModules[i].SeperateFromHere ();
			}else{
				childModules.RemoveAt (i);
			}
		}

		Destroy (gameObject);
	}

	public void SeperateFromHere () {
		GameObject newM = (GameObject)Instantiate (MissleEditor.current.misslePrefab, transform.position, transform.rotation);
		newM.rigidbody.isKinematic = false;
		newM.rigidbody.velocity = missle.rigidbody.velocity;
		missle = newM.GetComponent<Missle>();
		missle.Launch ();
		SyncMissleToMasterParent (missle);
	}

	void SyncMissleToMasterParent (Missle m) {
		missle.modules.Remove (this);
		missle = m;
		missle.modules.Add (this);
		transform.parent = m.transform;
		for (int i = 0 ; i < childModules.Count; i++) {
			childModules[i].SyncMissleToMasterParent (m);
		}
	}
}
