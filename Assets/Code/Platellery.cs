using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Platellery : MonoBehaviour {

	public List<GameObject> activeMissles = new List<GameObject>();

	public int maxMissles = 10;

	public Slider drillProgress;
	public Slider drillHealth;
	public EnemyDrill drill;
	public Planet planet;

	public static Platellery game;

	public bool hasWon;
	public bool hasLost;

	public GUISkin skin;

	public static bool debugMode = true;
	public GameObject editorCamera;
	public MissleEditor editor;
	public static CameraController cameraController;
	
	// Use this for initialization
	void Start () {
		game = this;
		drillProgress.maxValue = planet.radius;
		drillHealth.maxValue = drill.health;
		cameraController = Camera.main.GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {
		drillProgress.value = planet.radius - drill.y;
		drillHealth.value = drill.health;
	}

	public static void OnMissleSpawned (GameObject missle) {
		game.activeMissles.Add (missle);
		if (game.activeMissles.Count > game.maxMissles) {
			Destroy (game.activeMissles[0]);
			game.activeMissles.RemoveAt (0);
		}
	}

	public static void WinTheGame () {
		game.hasWon = true;
	}

	public static void LooseTheGame () {
		game.StartCoroutine ("ExploderizePlanet");
		game.hasLost = true;
	}

	IEnumerator ExploderizePlanet () {
		while (true) {
			if (Random.Range (0,20) == 0) planet.CreateExplosion (Random.Range (0, planet.radius * 2), Random.Range (0, planet.radius * 2), Random.Range (5,10), 9);
			yield return new WaitForFixedUpdate ();
		}
	}

	void OnGUI () {
		GUI.skin = skin;
		if (hasWon) {
			if (GUI.Button (new Rect (Screen.width / 3, Screen.height / 2 - 50, Screen.width / 3, 100), "YOU ARE VICTORIOUS.\nRESET?"))
				Application.LoadLevel (Application.loadedLevel);
		}
		if (hasLost) {
			if (GUI.Button (new Rect (Screen.width / 3, Screen.height / 2 - 50, Screen.width / 3, 100), "YOU HAVE FAILED.\nRESTART?"))
				Application.LoadLevel (Application.loadedLevel);
		}
		if (!editorCamera.activeInHierarchy) {
			if (GUI.Button (new Rect (20, 20, 200, 60), "EDITOR", skin.customStyles[0])) editor.OpenEditor ();
			if (GUI.Button (new Rect (Screen.width - 140, 20, 120, 30), "RESTART", skin.customStyles[0])) Application.LoadLevel (Application.loadedLevel);
		}
	}
}
