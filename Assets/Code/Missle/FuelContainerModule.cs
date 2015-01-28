using UnityEngine;
using System.Collections;

public class FuelContainerModule : Module {

	public float fuel;

	public FuelContainerModule parentFuel;

	public override void ModuleStart () {
		parentFuel = parentModule.GetComponent<FuelContainerModule>();
		fuel = mods[0].value;
	}

	public override void ModuleFixedUpdate () {

		if (fuel < mods[0].value) {

			if (parentFuel) if (parentFuel.fuel > 0) {
				fuel += Time.deltaTime;
				parentFuel.fuel -= Time.deltaTime;
			}
		}
	}
}
