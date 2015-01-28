using UnityEngine;
using System.Collections;

public class ThrusterModule : Module {

	public FuelContainerModule fuelContainer;
	public ParticleSystem par;
	public Transform thrustVector;
	public float thrust;

	private bool isThrusting;

	public override void OnParentUpdate () {
		fuelContainer = parentModule.GetComponent<FuelContainerModule>();
	}

	public override void ActivateModule () {
		isThrusting = mods[1].ToBool ();
	}

	void Toggle () {
		isThrusting = !isThrusting;
	}

	void Arm () {
		Toggle ();
	}

	void Update () {
		par.startSpeed = 25 * (mods[0].value/100f);
		par.emissionRate = 750 * (mods[0].value/100f);
		thrustVector.transform.localRotation = Quaternion.Euler (0,0,mods[2].value);
	}

	public override void ModuleFixedUpdate () {
		
		if (fuelContainer) {
			if (isThrusting) {
				if (fuelContainer.fuel > 0) {
					if (isActive) {
						missle.rigidbody.AddForceAtPosition (thrustVector.up * thrust * (mods[0].value/100f) * Time.fixedDeltaTime, transform.position);
						fuelContainer.fuel -= Time.fixedDeltaTime * (mods[0].value/100f);
						StartParticles ();
						return;
					}
				}
			}
		}
		StopParticles ();
	}

	void StartParticles () {
		if (!par.isPlaying) {
			par.Play ();
		}
	}

	void StopParticles () {
		if (par.isPlaying) {
			par.Stop ();
		}
	}
}
