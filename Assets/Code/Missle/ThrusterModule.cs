using UnityEngine;
using System.Collections;

public class ThrusterModule : Module {

	public FuelContainerModule fuelContainer;
	public float thrust;

	public override void ModuleStart () {

		fuelContainer = parentModule.GetComponent<FuelContainerModule>();

	}

	public override void ModuleFixedUpdate () {

		if (fuelContainer) if (fuelContainer.fuel > 0) {
			missle.rigidbody.AddForceAtPosition (transform.up * thrust * Time.fixedDeltaTime, transform.position);
			fuelContainer.fuel -= Time.fixedDeltaTime;
		}

	}
}
