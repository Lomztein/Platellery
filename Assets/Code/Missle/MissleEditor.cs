using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MissleEditor : MonoBehaviour {

	public GameObject[] parts;
	private Module[] partModules;
	private int hoveringID;

	public GameObject misslePrefab;
	public GameObject currentMissle;

	public Transform moduleButtonStart;
	public GameObject moduleButtonPrefab;
	public float moduleButtonDistance;
	public RectTransform buttonParent;

	public List<GameObject> currentParts = new List<GameObject>();
	public List<Button> currentButtons = new List<Button>();

	public int placingPartID;
	public bool canPlacePlacingPart;
	public GameObject placingPartSprite;
	public GameObject focuserSprite;
	private Vector3 placementPos;
	private int placementAngle;
	private bool isDragging;
	private int isFlipped;

	private GameObject focusPart;
	private Module focusModule;

	public Camera editorCamera;

	private Texture2D[] buttons;

	public static MissleEditor current;
	public GUISkin skin;

	public LayerMask missleLayer;

	public bool canInteract;
	public bool canBuild;

	// Use this for initialization
	void Start () {
		ResetEditor ();
		GetFocusPart (1);
		InitializePartButtons ();
		InitializeAssetModuleArray ();
		current = this;
	}

	void InitializeAssetModuleArray () {
		partModules = new Module[parts.Length];
		for (int i = 0; i < parts.Length ; i++) {
			partModules[i] = parts[i].GetComponent<Module>();
		}
	}

	void GetFocusPart (int index) {
		placingPartID = index;
		placingPartSprite.GetComponent<SpriteRenderer>().sprite = parts[placingPartID].transform.GetChild (0).GetComponent<SpriteRenderer>().sprite;
	}

	void ResetEditor () {
		focusModule = null;
		Destroy (currentMissle);
		for (int i = 0; i < currentParts.Count; i++) {
			focusPart = currentParts[i];
			RemovePart ();
		}
		currentParts.Clear ();
		placementPos = transform.position;
		currentMissle = (GameObject)Instantiate (misslePrefab, transform.position, Quaternion.identity);
		GetFocusPart (0);
		PlacePart ();
		focusModule.parentModule = null;
		GetFocusPart (1);
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<AudioSource>().volume = Game.musicLevel;

		hoveringID = -1;

		if (focusModule) {
			focuserSprite.transform.position = focusModule.transform.position + Vector3.back;
		}else{
			focuserSprite.transform.position = Vector3.back * 11;
		}
		
		placingPartSprite.transform.position = currentMissle.transform.position;
		canPlacePlacingPart = false;

		Ray ray = editorCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Input.GetAxis ("Vertical") > 0) placementAngle = 0;
		if (Input.GetAxis ("Vertical") < 0) placementAngle = 2;
		if (Input.GetAxis ("Horizontal") > 0) placementAngle = 1;
		if (Input.GetAxis ("Horizontal") < 0) placementAngle = 3;
		if (Input.GetButtonDown ("Mirror")) if (isFlipped == 0) { isFlipped = 1; }else{ isFlipped = 0; }

		if (Physics.Raycast (ray, out hit, 20f) && canBuild) {
			focusPart = hit.collider.gameObject;
			Bounds bounds = focusPart.transform.GetChild (0).GetComponent<Renderer>().bounds;
			
			Vector3 pos = Vector3.zero;
			Vector3 loc = new Vector3 ((hit.point.x - focusPart.transform.position.x),(hit.point.y - focusPart.transform.position.y));
			Vector3 ext = bounds.extents;
			Vector3 pExt = parts[placingPartID].transform.GetChild (0).GetComponent<Renderer>().bounds.extents;

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
			placingPartSprite.transform.rotation = Quaternion.Euler (0,isFlipped * 180,placementAngle * 90);

			if (!Physics.Raycast (new Ray (placementPos + Vector3.back * 5, Vector3.forward), 10, missleLayer)) {
				Debug.DrawRay (placementPos + Vector3.back * 5, Vector3.forward * 20, Color.white, 0.02f);
				canPlacePlacingPart = true;
			}

			if (Input.GetButtonDown ("Mouse1")) {
				focusModule = focusPart.GetComponent<Module>();
				if (canPlacePlacingPart) PlacePart ();
				isDragging = true;
			}

			if (Input.GetButtonUp ("Mouse1") && isDragging) {
				Module m = focusPart.GetComponent<Module>();
				if (m != focusModule) {
					if (focusModule.parentModule) focusModule.parentModule.childModules.Remove (focusModule);
					focusModule.parentModule = m;
					m.childModules.Add (focusModule);
					focusModule.OnParentUpdate ();
				}
				isDragging = false;
			}

			placementAngle += Mathf.RoundToInt (Input.GetAxis ("Mouse ScrollWheel") * 10);
			if (Input.GetButtonDown ("Mouse2")) RemovePart ();
		}

		if (focusModule) if (focusModule.parentModule) {
			if (Input.GetButton ("Mouse1")) {
				if (focusModule.parentLine) focusModule.parentLine.SetPosition (1, focusPart.transform.position + Vector3.back);
			}else{
				if (focusModule.parentLine) focusModule.parentLine.SetPosition (1, focusModule.parentModule.transform.position);
			}
		}

		if (Input.GetButtonDown ("Mouse2")) focusModule = null;

		if (!Input.GetButton ("Mouse1"))
			isDragging = false;

		if (placingPartID == -1) {
			placingPartSprite.GetComponent<Renderer>().material.color = Color.clear;
		}else if (canPlacePlacingPart) {
			placingPartSprite.GetComponent<Renderer>().material.color = Color.green;
		}else{
			placingPartSprite.GetComponent<Renderer>().material.color = Color.red;
		}

		if (currentParts.Count == 0) ResetEditor ();
	}

	void PlacePart () {
		GameObject newPart = (GameObject)Instantiate (parts[placingPartID], placementPos, Quaternion.Euler (0,isFlipped * 180,placementAngle * 90));
		currentParts.Add (newPart);
		focusModule = newPart.GetComponent<Module>();
		focusModule.missle = currentMissle.GetComponent<Missle>();
		newPart.transform.parent = currentMissle.transform;

		if (focusPart) {
			focusPart.GetComponent<Module>().childModules.Add (focusModule);
			focusModule.parentModule = focusPart.GetComponent<Module>();
		}

		focusModule.missle.modules.Add (focusModule);
	}

	public void ForcePlacePart (GameObject focus, Vector3 position, int partID) {
		placementPos = transform.position + position;
		focusPart = focus;
		GetFocusPart (partID);
		PlacePart ();
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
		for (int i = 0; i < parts.Length; i++) {
			GameObject butt = (GameObject)Instantiate (moduleButtonPrefab, moduleButtonStart.position + Vector3.down * moduleButtonDistance * i, Quaternion.identity);
			butt.transform.FindChild ("Image").GetComponent<Image>().sprite = parts[i].transform.GetChild (0).GetComponent<SpriteRenderer>().sprite;
			butt.transform.SetParent (buttonParent, true);
			AddListenerToModuleButton (butt.GetComponent<Button>(), i);
			currentButtons.Add (butt.GetComponent<Button>());
		}
	}

	public void TogglePartButton (int index) {
		currentButtons[index].interactable = !currentButtons[index].interactable;
	}

	void AddListenerToModuleButton (Button button, int i) {
		button.onClick.AddListener (() => {
			GetFocusPart (i);
		});
	}

	public void OpenEditor (bool enable) {
		editorCamera.gameObject.SetActive (true);
		enabled = true;
		Game.game.editorHUD.SetActive (true);
		Game.game.flightHUD.SetActive (false);
		GetComponent<AudioSource>().Play ();
		Camera.main.GetComponent<AudioListener>().enabled = false;
		canBuild = enable;
		canInteract = enable;
	}

	public void CloseEditor () {
		editorCamera.gameObject.SetActive (false);
		enabled = false;
		Game.game.flightHUD.SetActive (true);
		Game.game.editorHUD.SetActive (false);
		GetComponent<AudioSource>().Stop ();
		Camera.main.GetComponent<AudioListener>().enabled = true;
	}

	public void LaunchMissle () {
		if (canInteract) {
			GameObject newMissle = (GameObject)Instantiate (currentMissle, new Vector3 (Planet.current.radius, Planet.current.radius * 2 + 0.25f + GetMissleBounds ().y, 0), Quaternion.identity);
			Camera.main.GetComponent<CameraController>().FollowMissle (newMissle.transform);
			Missle m = newMissle.GetComponent<Missle>();
			m.Invoke ("InvokedLaunch",2);
			CloseEditor ();
			m.inEditor = false;
		}
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

		return max - min + Vector2.one * 0.5f;
	}

	void OnGUI () {
		GUI.skin = skin;
		if (editorCamera.gameObject.activeInHierarchy) {
			for (int i = 0; i < parts.Length; i++) {
				Rect r = new Rect (Screen.width - 80, 20 + 80 * i, 60, 60);
				if (r.Contains (new Vector3 (Input.mousePosition.x, -Input.mousePosition.y + Screen.height, 0)))
					hoveringID = i;
			}

			if (hoveringID > -1) {
				partModules[hoveringID].DrawModuleDescription (new Rect (20, Screen.height - 150, Screen.width / 3 - 40, 130));
			}else if (focusModule) { 
				if (canBuild) {
					focusModule.DrawModuleDescription (new Rect (20, Screen.height - 150, Screen.width / 3 - 40, 130));
					for (int i = 0 ; i <focusModule.mods.Length ; i++) {
						focusModule.mods[i].Draw (new Rect (240, 20 + i * 30, Screen.width - 340, 20), true);
					}
				}else{
					focusModule.DrawModuleDescription (new Rect (20, Screen.height - 150, Screen.width / 3 - 40, 130));
					for (int i = 0 ; i <focusModule.mods.Length ; i++) {
						focusModule.mods[i].Draw (new Rect (240, 20 + i * 30, Screen.width - 340, 20), false);
					}
				}
			}
		}
	}

	void OnDrawGizmos () {
		if (canPlacePlacingPart) Gizmos.DrawSphere (placementPos, 0.25f);
	}
}
