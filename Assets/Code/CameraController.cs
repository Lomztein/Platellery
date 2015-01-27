using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Planet focusPlanet;
	public float moveSpeed;

	private float distanceToCenter;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
	}

	void Move () {
		Vector3 movement = Vector3.zero;

		// Horizontal movement
		if (Input.mousePosition.x < 10) {
			movement.x = -moveSpeed * Time.deltaTime;
		}else if (Input.mousePosition.x > Screen.width - 10) {
			movement.x = moveSpeed * Time.deltaTime;
		}

		// Vertical movement
		if (Input.mousePosition.y < 10) {
			distanceToCenter -= moveSpeed * Time.deltaTime;
		}else if (Input.mousePosition.y > Screen.height - 10) {
			distanceToCenter += moveSpeed * Time.deltaTime;
		}

		movement.y = -(Vector3.Distance (transform.position - transform.forward * transform.position.z, focusPlanet.center) - distanceToCenter);
		if (distanceToCenter < camera.orthographicSize) distanceToCenter = camera.orthographicSize;

		// Put movement in action
		transform.rotation = Quaternion.LookRotation (Vector3.forward, transform.position - transform.forward * transform.position.z - focusPlanet.center);
		transform.Translate (movement);

	}

	public void MoveToPlanet (Planet newPlanet) {
		focusPlanet = newPlanet;
		distanceToCenter = focusPlanet.radius + camera.orthographicSize / 2;
		transform.position = focusPlanet.center + Vector3.up * distanceToCenter + Vector3.one / 2 + Vector3.back * 10;
	}
}
