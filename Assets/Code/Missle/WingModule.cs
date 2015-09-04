using UnityEngine;
using System.Collections;

public class WingModule : Module {

	public float strength;
	
	void Update () {
		if (missle.transform == CameraController.cam.followingMissle)  {
			missle.rigidbody.AddTorque (0, 0, strength * Time.deltaTime * Input.GetAxis ("Horizontal")
			                            * Planet.current.atmosphere.GetDensity (Planet.current.atmosphere.PositionToAltitude01 (transform.position))
			                            * missle.rigidbody.velocity.magnitude);
		}
	}
}
