using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissleEditor : MonoBehaviour {

	public GameObject[] parts;
	public GameObject misslePrefab;
	public GameObject currentMissle;

	public List<GameObject> currentParts = new List<GameObject>();

	public int placingPartID;
	public bool canPlacePlacingPart;
	public GameObject placingPartSprite;
	public GameObject focuserSprite;
	private Vector3 placementPos;
	private int placementAngle;
	private bool isDragging;

	private GameObject focusPart;
	private Module focusModule;

	public Camera editorCamera;

	private Texture2D[] buttons;

	public static MissleEditor current;
	public GUISkin skin;

	public LayerMask missleLayer;

	// Use this for initialization
	void Start () {
		ResetEditor ();
		GetFocusPart (1);
		InitializePartButtons ();
		current = this;
	}

	void GetFocusPart (int index) {
		placingPartID = index;
		placingPartSprite.GetComponent<SpriteRenderer>().sprite = parts[placingPartID].transform.GetChild (0).GetComponent<SpriteRenderer>().sprite;
	}

	void ResetEditor () {
		for (int i = 0; i < currentParts.Count; i++) {
			Destroy (currentParts[i]);
		}

		currentMissle = (GameObject)Instantiate (misslePrefab, transform.position, Quaternion.identity);
		GameObject f = (GameObject)Instantiate (parts[0], transform.position, Quaternion.identity);
		currentMissle.GetComponent<Missle>().modules.Add (f.GetComponent<Module>());
		f.transform.parent = currentMissle.transform;
		f.GetComponent<Module>().missle = currentMissle.GetComponent<Missle>();
		currentParts.Add (f);
	}
	
	// Update is called once per frame
	void Update () {

		if (focusModule) {
			focuserSprite.transform.position = focusModule.transform.position + Vector3.back;
		}else{
			focuserSprite.transform.position = Vector3.back * 11;
		}
		
		placingPartSprite.transform.position = currentMissle.transform.position;
		canPlacePlacingPart = false;

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

			if (loc.y > ext.y - margin) {
				pos.y = (pExt.y + ext.y);
			}else if (loc.y < -ext.y + margin) {
				pos.y = -(pExt.y + ext.y);
			}else if (loc.x > ext.x - margin) {
				pos.x = (pExt.x + ext.x);
			}else if (loc.x < -ext.x + margin) {
				pos.x = -(pExt.x + ext.x);
			}

			placementPos = focusPart.transform.position + pos;
			placingPartSprite.transform.position = placementPos;
			placingPartSprite.transform.rotation = Quaternion.Euler (0,0,placementAngle * 90);

			if (!Physics.Raycast (new Ray (placementPos + Vector3.back * 5, Vector3.forward), 10, missleLayer)) {
				Debug.DrawRay (placementPos + Vector3.back * 5, Vector3.forward * 20, Color.white, 0.02f);
				canPlacePlacingPart = true;
			}

			if (Input.GetButtonDown ("Fire1")) {
				focusModule = focusPart.GetComponent<Module>();
				if (canPlacePlacingPart) PlacePart ();
				isDragging = true;
			}

			if (Input.GetButtonDown ("Fire2")) RemovePart ();
			if (Input.GetButtonUp ("Fire1") && isDragging) {
				Module m = focusPart.GetComponent<Module>();
				if (focusModule.parentModule) focusModule.parentModule.childModules.Remove (focusModule);
				focusModule.parentModule = m;
				m.childModules.Add (focusModule);
				isDragging = false;
			}

			if (Input.GetButton ("Fire1")) {
				if (focusModule.parentLine) focusModule.parentLine.SetPosition (1, focusPart.transform.position + Vector3.back);
			}

			placementAngle += Mathf.RoundToInt (Input.GetAxis ("Mouse ScrollWheel") * 10);
		}

		if (!Input.GetButton ("Fire1"))
			isDragging = false;

		if (placingPartID == -1) {
			placingPartSprite.renderer.material.color = Color.clear;
		}else if (canPlacePlacingPart) {
			placingPartSprite.renderer.material.color = Color.green;
		}else{
			placingPartSprite.renderer.material.color = Color.red;
		}

		if (currentParts.Count == 0) ResetEditor ();
	}

	void PlacePart () {
		GameObject newPart = (GameObject)Instantiate (parts[placingPartID], placementPos, Quaternion.Euler (0,0,placementAngle * 90));
		currentParts.Add (newPart);
		focusModule = newPart.GetComponent<Module>();
		focusModule.missle = currentMissle.GetComponent<Missle>();
		newPart.transform.parent = currentMissle.transform;

		if (focusPart) {
			focusPart.GetComponent<Module>().childModules.Add (focusModule);
			focusModule.parentModule = focusPart.GetComponent<Module>();
		}

		focusModule.missle.modules.Add (focusModule);
		focusModule.ModuleStart ();
	}
	
	void RemovePart () {
		focusModule = focusPart.GetComponent<Module>();
		if (focusModule.childModules.Count == 0) {
			focusModule.Die ();
			currentParts.Remove (focusPart);
		}
	}

	void FindCenter () {
	}

	void InitializePartButtons () {
		buttons = new Texture2D[parts.Length];
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i] = parts[i].transform.GetChild (0).GetComponent<SpriteRenderer>().sprite.texture;
		}
	}

	public void OpenEditor () {
		editorCamera.gameObject.SetActive (true);
	}

	public void CloseEditor () {
		editorCamera.gameObject.SetActive (false);
	}
	
	public void LaunchMissle () {
		GameObject newMissle = (GameObject)Instantiate (currentMissle, new Vector3 (Planet.current.radius, Planet.current.radius * 2 + 0.25f + GetMissleBounds ().y, 0), Quaternion.identity);
		editorCamera.gameObject.SetActive (false);
		Camera.main.GetComponent<CameraController>().FollowMissle (newMissle.transform);
		newMissle.GetComponent<Missle>().Invoke ("Launch", 2);
	}

	Vector2 GetMissleBounds () {
		Vector2 min = new Vector2 (float.MaxValue, float.MaxValue);
		Vector2 max = new Vector2 (float.MinValue, float.MinValue);

		for (int i = 0; i < currentParts.Count ; i++ ) {
			Vector2 loc = new Vector2 (currentParts[i].transform.position.x, currentParts[i].transform.position.y);
			if (loc.x > max.x) max.x = loc.x;
			if (loc.x < min.x) min.x = loc.x;
			if (loc.y > max.y) max.y = loc.y;
			if (loc.y < min.y) min.y = loc.y;
		}

		Debug.Log (max + ", " + min + ", " + (max - min));
		return max - min + Vector2.one * 0.5f;
	}

	void OnGUI () {
		GUI.skin = skin;
		if (editorCamera.gameObject.activeInHierarchy) {
			for (int i = 0; i < buttons.Length; i++) {
				if (GUI.Button (new Rect (Screen.width - 80, 20 + 80 * i, 60, 60), "", skin.customStyles[0])) 
					GetFocusPart (i);
				GUI.DrawTexture (new Rect (Screen.width - 70, 30 + 80 * i, 40, 40), buttons[i], ScaleMode.ScaleToFit, true, 0);
			}
			if (GUI.Button (new Rect (Screen.width / 3, Screen.height - 100, Screen.width / 3, 50), "LAUNCH!", skin.customStyles[0])) LaunchMissle ();
			if (GUI.Button (new Rect (20, 20, 200, 60), "BACK", skin.customStyles[0])) CloseEditor ();

			if (focusModule) {
				for (int i = 0 ; i <focusModule.mods.Length ; i++) {
					focusModule.mods[i].Draw (new Rect (240, 20 + i * 30, Screen.width - 340, 20));
				}
			}
			
		}else{
			if (GUI.Button (new Rect (20, 20, 200, 60), "EDITOR", skin.customStyles[0])) OpenEditor ();
		}
	}

	void OnDrawGizmos () {
		if (canPlacePlacingPart) Gizmos.DrawSphere (placementPos, 0.25f);
	}
}
