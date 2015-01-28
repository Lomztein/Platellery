using UnityEngine;
using System.Collections;

public class WarheadModule : Module {

	public float explosionRange;
	public int explosionStrength;

	public override void BlowUp () {
		Planet.current.CreateExplosion (transform.position.x, transform.position.y, explosionRange, explosionStrength);
	}
}
