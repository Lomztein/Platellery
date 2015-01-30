using UnityEngine;
using System.Collections;
[System.Serializable]

public class Atmosphere {

	public float altitude;
	public AnimationCurve density;
	public Gradient colors;

	public float GetDensity (float altitude) {
		return density.Evaluate (altitude);
	}

	public Color GetAtmosColor (float altitude) {
		return colors.Evaluate (altitude);
	}

	public float PositionToAltitude01 (Vector3 pos) {
		float d = Vector3.Distance (pos, Planet.current.center) - Planet.current.radius;
		return Mathf.Clamp01 (d/altitude);
	}
}
