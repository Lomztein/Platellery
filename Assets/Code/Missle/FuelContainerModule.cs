using UnityEngine;
using System.Collections;

public class FuelContainerModule : Module {

	public float maxFuel;
	public float fuel;

	public override void ModuleFixedUpdate () {

		if (fuel <= 0) {
			missle.SeperateSeperators ();
		}
	}}
