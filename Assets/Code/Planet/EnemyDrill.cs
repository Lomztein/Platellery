using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDrill : MonoBehaviour {

	private int x;
	public  int y = -1;

	public float drillTimer;

	public Planet planet;
	public GameObject drillPrefab;
	public List<GameObject> drills = new List<GameObject>();

	public float health;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Drill", drillTimer, drillTimer);
		x = planet.radius;

		transform.position = new Vector3 (planet.radius, 0, -1);
	}

	void Update () {
	}

	void Drill () {
		if (!Platellery.game.hasLost) {
			planet.ChangeSingleTile (x,y, 0);
			planet.ChangeSingleTile (x-1,y, 0);
			y += 1;

			for (int i = 0; i < drills.Count; i++) {
				drills[i].transform.position += Vector3.up;
			}
			drills.Add ((GameObject)Instantiate (drillPrefab, transform.position + new Vector3 (0,-1,0.1f), Quaternion.identity));
			drills[drills.Count-1].transform.parent = transform;

			if (y == planet.radius) Platellery.LooseTheGame ();
		}
	}

	public void TakeDamage (float d) {
		health -= d;
	}
}
