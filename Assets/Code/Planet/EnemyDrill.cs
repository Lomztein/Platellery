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
	public bool isDead;

	public GameObject cannon;
	public float cannonDist;
	public List<GameObject> cannons = new List<GameObject>();

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Drill", drillTimer, drillTimer);
		x = planet.radius;

		transform.position = new Vector3 (planet.radius, 0, -1);
	}

	public void SpawnTurrets () {
		float y = transform.position.y + 1f;
		Vector3 pos = new Vector3 (transform.position.x, y, 0);
		
		GameObject can = (GameObject)Instantiate (cannon, pos + Vector3.right * cannonDist, Quaternion.Euler (0, 0, 180));
		cannons.Add (can);
		can = (GameObject)Instantiate (cannon, pos + Vector3.left * cannonDist, Quaternion.Euler (0, 0, 180));
		cannons.Add (can);
		can.GetComponent<Cannon> ().cannonTransform.localScale = new Vector3 (1, 1, 1);
		can.GetComponent<Cannon> ().flipped = 1;
		// Shit's confuzzling, but whatever.
	}

	void Drill () {
		if (!Game.hasEnded && Game.game.hasStarted) {
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
		if (health < 0 && !isDead) {
			isDead = true;
			Game.WinTheStage ();
		}
	}

	public IEnumerator ExplodeTheShitOutOfThisThing () {
		int explosions = Random.Range (6,9);
		for (int i = 0; i < explosions; i++) {
			Vector3 pos = transform.position + Random.insideUnitSphere * 2f;
			planet.CreateExplosion (pos.x, pos.y, Random.Range (4f, 7f), 50f);
			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}
		planet.CreateExplosion (transform.position.x, transform.position.y, 15, 50f);
		foreach (GameObject obj in cannons) {
			Destroy (obj);
		}
		gameObject.SetActive (false);
	}
}
