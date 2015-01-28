using UnityEngine;
using System.Collections;

public class GrenadeModule : Module {

	public float explosionRange;
	public int explosionStrength;

	public void Arm () {
		if (IsInvoking ()) Invoke ("Kill", mods[0].value);
	}

	public override void ActivateModule () {
		if (mods[1].ToBool ()) Invoke ("Kill", mods[0].value);
	}

	void Kill () {
		Planet.current.CreateExplosion (transform.position.x, transform.position.y, explosionRange, explosionStrength);
		Die ();
	}
}
