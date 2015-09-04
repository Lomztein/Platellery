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

	public GameObject cannon;
	public float cannonDist;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Drill", drillTimer, drillTimer);
		x = planet.radius;

		transform.position = new Vector3 (planet.radius, 0, -1);

		float y = transform.position.y + 1f;
		Vector3 pos = new Vector3 (transform.position.x, y, 0);

		Instantiate (cannon, pos + Vector3.right * cannonDist, Quaternion.Euler (0, 0, 180));
		GameObject can = (GameObject)Instantiate (cannon, pos + Vector3.left * cannonDist, Quaternion.Euler (0, 0, 180));
		can.GetComponent<Cannon> ().cannonTransform.localScale = new Vector3 (1, 1, 1);
		can.GetComponent<Cannon> ().flipped = 1;
		// Shit's confuzzling, but whatever.
	}

	void Update () {
	}

	void Drill () {
		if (!Game.game.hasLost && !Game.game.hasWon) {
			planet.ChangeSingleTile (x,y, 0);
			planet.ChangeSingleTile (x-1,y, 0);
			y += 1;

			for (int i = 0; i < drills.Count; i++) {
				drills[i].transform.position += Vector3.up;
			}
			drills.Add ((GameObject)Instantiate (drillPrefab, transform.position + new Vector3 (0,-1,0.1f), Quaternion.identity));
			drills[drills.Count-1].transform.parent = transform;

			if (y == planet.radius) Game.LooseTheGame ();
		}
	}

	public void TakeDamage (float d) {
		health -= d;
		if (health < 0) {
			Game.WinTheGame ();
		}
	}
}
