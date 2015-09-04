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
	private float zoomFactor = 0.99f;

	public Transform followingMissle;

	private Vector3 targetPos;
	private Quaternion targetRot;
	private float targetSize;

	public Transform arrow;
	private SpriteRenderer arrowRenderer;

	public bool enableMovement = true;
	public static CameraController cam;

	// Use this for initialization
	void Start () {
		cam = this;
		distanceToCenter = Vector3.Distance (Planet.current.center, transform.position + Vector3.back * transform.position.z);
		startingDistance = distanceToCenter;
		arrowRenderer = arrow.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Cancel")) followingMissle = null;
		if (followingMissle) {
			arrowRenderer.color = new Color (1f,1f,1f,1f-zoomFactor);
			Quaternion rot = Quaternion.LookRotation (Vector3.forward, followingMissle.position - focusPlanet.center);
			arrow.transform.position = rot * new Vector3 (0, focusPlanet.radius + 2.5f, 0) + focusPlanet.center;
			arrow.transform.rotation = Quaternion.Euler (0,0,Angle.CalculateAngle (focusPlanet.center, followingMissle.position));
		}else{
			arrowRenderer.color = new Color (1f,1f,1f,0f);
		}

		if (enableMovement) {
			Move ();
		}else{
			EndMove ();
		}
	}

	void Move () {
		Vector3 movement = Vector3.zero;
		distanceToCenter = Vector3.Distance (Planet.current.center, targetPos);
		
		if (followingMissle) {
			targetPos = followingMissle.position + Vector3.back * 10;
		}
		if (!MissleEditor.current.editorCamera.gameObject.activeInHierarchy) {
			// Horizontal movement
				if (!followingMissle) {
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
				movement.y = -(Vector3.Distance (targetPos, focusPlanet.center) - distanceToCenter);
			}

			zoomFactor += Input.GetAxis ("Mouse ScrollWheel");
			if (distanceToCenter < 15) distanceToCenter = 15;
		}
		zoomFactor = Mathf.Clamp (zoomFactor, 0f,0.999f);

		targetRot = Quaternion.LookRotation (Vector3.forward, transform.position + Vector3.back * transform.position.z - focusPlanet.center);
		targetPos += targetRot * (movement * (distanceToCenter / startingDistance)) * zoomFactor;
		targetSize = Mathf.Lerp (Planet.current.radius + 10, 15, zoomFactor);
		EndMove ();

	}

	public void ForceMove (Vector3 movement, float distance) {
		movement.y = -(Vector3.Distance (targetPos, focusPlanet.center) - distance);
		targetRot = Quaternion.LookRotation (Vector3.forward, transform.position + Vector3.back * transform.position.z - focusPlanet.center);
		targetPos += targetRot * (movement * (distanceToCenter / startingDistance)) * zoomFactor;
		targetSize = Mathf.Lerp (Planet.current.radius + 10, 15, zoomFactor);
		EndMove ();
	}

	void EndMove () {
		// Put movement in action
		transform.position = Vector3.Lerp (transform.position, Vector3.Lerp (Planet.current.center + Vector3.back * 10, targetPos, zoomFactor), moveSpeed * Time.deltaTime);
		GetComponent<Camera>().orthographicSize = Mathf.Lerp (GetComponent<Camera>().orthographicSize, targetSize, moveSpeed * Time.deltaTime); 
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Lerp (Quaternion.identity, targetRot , zoomFactor),moveSpeed * Time.deltaTime);
	}

	public void MoveToPosition (Vector3 pos, float angle, float size) {
		targetPos = pos + Vector3.back * 10;
		targetRot = Quaternion.Euler (new Vector3 (0,0,angle));
		targetSize = size;
	}

	public void MoveToPlanet (Planet newPlanet) {
		focusPlanet = newPlanet;
		distanceToCenter = focusPlanet.radius + GetComponent<Camera>().orthographicSize / 2;
		targetPos = focusPlanet.center + Vector3.up * distanceToCenter + Vector3.one / 2 + Vector3.back * 10;
		transform.position = targetPos;
	}

	public void FollowMissle (Transform missle) {
		followingMissle = missle;
		zoomFactor = 1;
	}
}
