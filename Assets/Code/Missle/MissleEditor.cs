using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissleEditor : MonoBehaviour {

	public GameObject[] parts;
	public GameObject currentMissle;

	private List<GameObject> currentParts = new List<GameObject>();

	public int placingPartID;
	public bool canPlacePlacingPart;
	public GameObject placingPartSprite;
	private Vector3 placementPos;
	private float placementAngle;

	private GameObject focusPart;

	public Camera editorCamera;

	private Texture2D[] buttons;

	// Use this for initialization
	void Start () {
		ResetEditor ();
		GetFocusPart (1);
		InitializePartButtons ();
	}

	void GetFocusPart (int index) {
		placingPartID = index;
		placingPartSprite.GetComponent<SpriteRenderer>().sprite = parts[placingPartID].transform.GetChild (0).GetComponent<SpriteRenderer>().sprite;
	}

	void ResetEditor () {
		for (int i = 0; i < currentParts.Count; i++) {
			Destroy (currentParts[i]);
		}

		GameObject f = (GameObject)Instantiate (parts[0], transform.position, Quaternion.identity);
		f.transform.parent = currentMissle.transform;
		f.GetComponent<Module>().missle = currentMissle.GetComponent<Missle>();
		currentParts.Add (f);
	}
	
	// Update is called once per frame
	void Update () {

		placingPartSprite.transform.position = Vector3.back * 1000;

		Ray ray = editorCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 20f)) {
			focusPart = hit.collider.gameObject;
			Bounds bounds = focusPart.transform.GetChild (0).renderer.bounds;

			Vector3 pos = Vector3.zero;
			Vector3 loc = new Vector3 ((hit.point.x - focusPart.transform.position.x),(hit.point.y - focusPart.transform.position.y));
			Vector3 ext = bounds.extents;
			Vector3 pExt = parts[placingPartID].transform.GetChild (0).renderer.bounds.extents;

			float margin = 0.1f;

			Debug.Log (ext);

			if (loc.y > ext.y - margin) {
				pos.y = (pExt.y + ext.y);
			}else if (loc.y < -ext.y + margin) {
				pos.y = -(pExt.y + ext.y);
			}else if (loc.x > ext.x - margin) {
				pos.x = (pExt.x + ext.x);
			}else if (loc.x < -ext.x + margin) {
				pos.x = -(pExt.x + ext.x);
			}

			// pos.x *= (pExt.x + ext.x) - (pCen.x - cen.x);
			// pos.y *= (pExt.y + ext.y) - (pCen.y - cen.y);

			placementPos = focusPart.transform.position + pos + Vector3.forward;
			placingPartSprite.transform.position = placementPos;
			
			if (!Physics.CheckSphere (placementPos, 0.25f))
				canPlacePlacingPart = true;

			if (Input.GetButtonDown ("Fire1") && canPlacePlacingPart) PlacePart ();
		}

		if (placingPartID == -1) {
			placingPartSprite.renderer.material.color = Color.clear;
		}else if (canPlacePlacingPart) {
			placingPartSprite.renderer.material.color = Color.green;
		}else{
			placingPartSprite.renderer.material.color = Color.red;
		}
	}

	void PlacePart () {
		GameObject newPart = (GameObject)Instantiate (parts[placingPartID], placementPos, Quaternion.Euler (0,0,placementAngle));
		currentParts.Add (newPart);
		Module module = newPart.GetComponent<Module>();
		module.missle = currentMissle.GetComponent<Missle>();
		if (focusPart) focusPart.GetComponent<Module>().childModules.Add (module);
		newPart.transform.parent = currentMissle.transform;
	}

	void FindCenter () {
	}

	void InitializePartButtons () {
		buttons = new Texture2D[parts.Length];
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i] = parts[i].transform.GetChild (0).GetComponent<SpriteRenderer>().sprite.texture;
		}
	}

	void OnGUI () {
		for (int i = 0; i < buttons.Length; i++) {
			if (GUI.Button (new Rect (Screen.width - 80, 20 + 80 * i, 60, 60), "")) 
				GetFocusPart (i);
			GUI.DrawTexture (new Rect (Screen.width - 80, 20 + 80 * i, 60, 60), buttons[i], ScaleMode.ScaleToFit, true, 0);
		}
	}
}
