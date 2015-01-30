using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {

	public virtual string GetTile (int x, int y, float d, float a) {
		return "";
	}

	public void Initialize (Planet p, Dictionary<string, int> t, int r, float c) {
		planet = p;
		tile = t;
		radius = r;
		temperature = c;
	}

	public Planet planet;
	public Dictionary<string, int> tile;
	public int radius;
	public float temperature;

}
