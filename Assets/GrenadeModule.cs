using UnityEngine;
using System.Collections;

public class GrenadeModule : WarheadModule {

	public float timer;

	public void Arm () {
		Invoke ("Kill", timer);
	}

	void Kill () {
		Destroy (gameObject);
	}
}
