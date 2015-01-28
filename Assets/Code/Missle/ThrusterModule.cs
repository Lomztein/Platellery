using UnityEngine;
using System.Collections;

public class ThrusterModule : Module {

	public FuelContainerModule fuelContainer;
	public ParticleSystem par;
	public float thrust;
	public float throttle; // Goes from 0 to 1

	public override void ModuleStart () {

		fuelContainer = parentModule.GetComponent<FuelContainerModule>();
		par.Play ();
		par.startSpeed = 25 * throttle;
		
	}

	public override void ModuleFixedUpdate () {

		if (fuelContainer) {
			if (fuelContainer.fuel > 0) {
				missle.rigidbody.AddForceAtPosition (transform.up * thrust * throttle * Time.fixedDeltaTime, transform.position);
				fuelContainer.fuel -= Time.fixedDeltaTime;

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
