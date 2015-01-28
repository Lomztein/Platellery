using UnityEngine;
using System.Collections;

public class WarheadModule : Module {

	public float explosionRange;
	public int explosionStrength;

	public override void DetonateModule () {
		Planet.current.CreateExplosion (transform.position.x, transform.position.y, explosionRange * (mods[0].value/100f), explosionStrength * (mods[0].value/100f));
	}
}
