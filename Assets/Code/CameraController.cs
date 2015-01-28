using UnityEngine;
using System.Collections;

	/*
	 * WARNING
	 * THIS CODE MIGHT MAKE YOU PUKE
	 * IT ALMOST MADE ME
	 * SERIOUSLY
	 * WHAT THE FUCK?
	 */

public class CameraController : MonoBehaviour {

	public Planet focusPlanet;
	public float moveSpeed;

	private float startingDistance;
	private float distanceToCenter;
	private float zoomFactor;

	public Transform followingMissle;

	public Vector3 targetPos;
	private Quaternion targetRot;

	// Use this for initialization
	void Start () {
		distanceToCenter = Vector3.Distance (Planet.current.center, transform.position + Vector3.back * transform.position.z);
		startingDistance = distanceToCenter;
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
	}

	void Move () {
		Vector3 movement = Vector3.zero;
		distanceToCenter = Vector3.Distance (Planet.current.center, targetPos);
		
		if (followingMissle) {
			targetPos = followingMissle.position + Vector3.back * 10;
		}else if (!MissleEditor.current.editorCamera.gameObject.activeInHierarchy) {
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

			zoomFactor += Input.GetAxis ("Mouse ScrollWheel");
			movement.y = -(Vector3.Distance (targetPos, focusPlanet.center) - distanceToCenter);
			if (distanceToCenter < 15) distanceToCenter = 15;
		}
		zoomFactor = Mathf.Clamp (zoomFactor, 0.01f,0.99f);

		// Put movement in action
		targetPos += transform.rotation * (movement * (distanceToCenter / startingDistance));
		transform.position = Vector3.Lerp (transform.position, Vector3.Lerp (Planet.current.center + Vector3.back * 10 + transform.up, targetPos, zoomFactor), moveSpeed * Time.deltaTime);
		camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, Mathf.Lerp (Planet.current.radius + 10, 15, zoomFactor), moveSpeed * Time.deltaTime); 
		transform.rotation = Quaternion.LookRotation (Vector3.forward, transform.position + Vector3.back * transform.position.z - focusPlanet.center);

	}

	public void MoveToPlanet (Planet newPlanet) {
		focusPlanet = newPlanet;
		distanceToCenter = focusPlanet.radius + camera.orthographicSize / 2;
		targetPos = focusPlanet.center + Vector3.up * distanceToCenter + Vector3.one / 2 + Vector3.back * 10;
		transform.position = targetPos;
	}

	public void FollowMissle (Transform missle) {
		followingMissle = missle;
		zoomFactor = 1;
	}
}
