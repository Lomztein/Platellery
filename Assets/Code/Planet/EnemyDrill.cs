using UnityEngine;
using System.Collections;

public class EnemyDrill : MonoBehaviour {

	public int x;
	public int y;

	public float drillTimer;

	public Planet planet;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Drill", drillTimer, drillTimer);
		x = planet.radius;

		transform.position = new Vector3 (planet.radius, 0, -1);
	}

	void Drill () {
		planet.ChangeSingleTile (x,y, 0);
		y += 1;
	}
}
