using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Module : MonoBehaviour {

	public enum Type { FuelContainer, Thruster, Warhead, Grenade, Seperator, Activator, Structural };

	public string moduleName;
	public string moduleDesc;
	public Type moduleType;

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

	public virtual void OnParentUpdate () {
	}

	public void Activate () {
		isActive = true;
		ActivateModule ();
		if (parentModule) OnParentUpdate ();
	}

	void Collide (Collision col) {
		if (col.relativeVelocity.magnitude > collisionTolerance && isActive) {
			Die ();
		}
	}

	void FixedUpdate () {
		if (isActive) ModuleFixedUpdate ();

		if (Platellery.game.activeMissles.Count > 0) if (gameObject == Platellery.game.activeMissles[0]) {
			float dif = Vector3.Angle(missle.rigidbody.velocity, missle.transform.up);
			float cpScale = (((dif -90) - 90)/ 720) - 0.125f;
			Debug.Log (cpScale);
		}

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

		missle.CheckModules ();
		Planet.CreateExplosionEffect (transform.position.x, transform.position.y, 0.5f);
		Destroy (gameObject);
	}

	public void SeperateFromHere () {
		GameObject newM = (GameObject)Instantiate (MissleEditor.current.misslePrefab, transform.position, transform.rotation);
		newM.rigidbody.isKinematic = false;
		newM.rigidbody.velocity = missle.rigidbody.velocity;
		missle.modules.Remove (this);
		missle = newM.GetComponent<Missle>();
		missle.Launch ();
		SyncMissleToMasterParent (missle, this);
	}

	void SyncMissleToMasterParent (Missle m, Module start) {
		missle.modules.Remove (this);
		missle = m;
		missle.modules.Add (this);
		transform.parent = m.transform;
		for (int i = 0 ; i < childModules.Count; i++) {
			if (childModules[i] != start) childModules[i].SyncMissleToMasterParent (m, start);
		}
	}

	public void DrawModuleDescription (Rect rect) {
		// Window, header and description.
		GUI.Label (rect,"", GUI.skin.customStyles[0]);
		GUI.Label (new Rect (rect.x + 20, rect.y + 20, rect.width - 40, 20),moduleName, GUI.skin.customStyles[1]);
		GUI.Label (new Rect (rect.x + 20, rect.y + 50, rect.width - 40, rect.height - 70),moduleDesc, GUI.skin.customStyles[2]);
	}
}
