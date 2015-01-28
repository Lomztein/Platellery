using UnityEngine;
using System.Collections;

public class ThrusterModule : Module {

	public FuelContainerModule fuelContainer;
	public ParticleSystem par;
	public float thrust;

	private bool isThrusting;

	public override void ModuleStart () {

		fuelContainer = parentModule.GetComponent<FuelContainerModule>();
		par.Play ();
		
	}

	public override void ActivateModule () {
		isThrusting = mods[1].ToBool ();
	}

	void Arm () {
		ActivateModule ();
	}

	void Update () {
		par.startSpeed = 25 * (mods[0].value/100f);
	}

	public override void ModuleFixedUpdate () {
		
		if (fuelContainer && isThrusting) {
			if (fuelContainer.fuel > 0) {
				missle.rigidbody.AddForceAtPosition (transform.up * thrust * (mods[0].value/100f) * Time.fixedDeltaTime, transform.position);
				fuelContainer.fuel -= Time.fixedDeltaTime * (mods[0].value/100f);

				if (isActive) {
					if (!par.isPlaying) {
						par.Play ();
					}
				}
			}else{
				if (par.isPlaying) {
					par.Stop ();
				}
			}
		}
	}
}
