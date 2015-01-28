using UnityEngine;
using System.Collections;

public class FuelContainerModule : Module {

	public float maxFuel;
	public float fuel;

	public FuelContainerModule parentFuel;

	public override void ModuleStart () {
		parentFuel = parentModule.GetComponent<FuelContainerModule>();
	}

	public override void ModuleFixedUpdate () {

		if (fuel < maxFuel) {

			if (parentFuel) if (parentFuel.fuel > 0) {
				fuel += Time.deltaTime;
				parentFuel.fuel -= Time.deltaTime;
			}
		}
	}
}
