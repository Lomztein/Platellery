using UnityEngine;
using System.Collections;

public class FuelContainerModule : Module {

	private float startFuel;
	public float fuel;

	public FuelContainerModule parentFuel;

	public override void OnParentUpdate () {
		parentFuel = parentModule.GetComponent<FuelContainerModule>();
	}

	public override void ModuleStart () {
		fuel = mods[0].value;
		startFuel = mods[0].value;
	}

	public override void ModuleFixedUpdate () {

		if (fuel < mods[0].value) {

			if (parentFuel) if (parentFuel.fuel > 0) {
				float f = Mathf.Max  (startFuel-fuel, parentFuel.fuel);
				fuel += f;
				parentFuel.fuel -= f;
			}
		}
	}
}
