using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public List<GameObject> activeMissles = new List<GameObject>();

	public int maxMissles = 10;

	public Slider drillProgress;
	public Slider drillHealth;
	public EnemyDrill drill;
	public Planet planet;

	public static Game game;

	public bool hasStarted;
	public static bool hasEnded;

	public static int currentStage = -1;
	public GameObject[] stageIntros;

	public GUISkin skin;

	public static bool debugMode = false;
	public GameObject editorCamera;
	public MissleEditor editor;
	public static CameraController cameraController;
	public static Tutorial tutorial;

	// Menu stuff
	public GameObject pauseMenu;
	public Slider musicSlider;
	public Slider soundSlider;

	public GameObject winGameMenu;
	public GameObject lostGameMenu;
	public Text winGameText;

	public static float soundLevel = 1;
	public static float musicLevel = 1;

	// HUD stuff
	public GameObject flightHUD;
	public GameObject editorHUD;

	public GameObject startMenu;

	// Use this for initialization
	void Start () {
		game = this;
		tutorial = GetComponent<Tutorial>();
		cameraController = Camera.main.GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {

		if (pauseMenu.activeInHierarchy) {
			soundLevel = soundSlider.value;
			musicLevel = musicSlider.value;
		}

		if (Input.GetButton ("DebugMode")) debugMode = !debugMode;

		TestPause ();

		if (!hasStarted)
			cameraController.ForceMove (new Vector3 (5f, 0) * Time.deltaTime, planet.radius + 5f);
	}

	void TestPause () {
		if (Input.GetButtonDown ("Cancel") && cameraController.followingMissle == null) {
			if (!pauseMenu.activeInHierarchy) {
				Pause ();
				return;
			}
			if (pauseMenu.activeInHierarchy) {
				Resume ();
				return;
			}
		}
	}

	public static void OnMissleSpawned (GameObject missle) {
		game.activeMissles.Add (missle);
		if (game.activeMissles.Count > game.maxMissles) {
			Destroy (game.activeMissles[0]);
			game.activeMissles.RemoveAt (0);
		}
	}

	public static void WinTheGame () {
		cameraController.DisableInteraction ();
		cameraController.MoveToPosition (game.drill.transform.position, 180f, 10f);
		game.drill.StartCoroutine (game.drill.ExplodeTheShitOutOfThisThing ());

		game.winGameMenu.SetActive (true);
		game.winGameText.text = "You've beaten the game with only " + Planet.current.CalculateDestroyedPercentageInt ().ToString () + " percent of the planet destroyed!";
		hasEnded = true;
	}

	public static void WinTheStage () {
		cameraController.DisableInteraction ();
		cameraController.MoveToPosition (new Vector3 (Planet.current.radius, 0f, 0f), 180f, 10f);
		game.drill.StartCoroutine (game.drill.ExplodeTheShitOutOfThisThing ());
		game.Invoke ("ContinueNextStage", 5f);
	}

	public void ContinueNextStage () {
		currentStage++;
		cameraController.MoveToPosition (Planet.current.center, 0f, Planet.current.radius);
		stageIntros[currentStage].SetActive (true);
		hasEnded = true;
		SendMessage ("Stage" + currentStage.ToString (), SendMessageOptions.DontRequireReceiver);
	}

	public void RespawnDrill () {
		game.drill.gameObject.SetActive (true);
		game.drill.health = 500f;
		game.drill.isDead = false;
	}

	public void StartStage () {
		stageIntros[currentStage].SetActive (false);
		cameraController.MoveToPosition (new Vector3 (planet.radius, planet.radius * 2f, 0), 0f, 15f);
		cameraController.EnableInteraction ();
		hasEnded = false;
	}

	public static void LooseTheGame () {
		game.lostGameMenu.SetActive (true);
		cameraController.DisableInteraction ();
		cameraController.MoveToPosition (Planet.current.center, 0, Planet.current.radius);
		game.StartCoroutine ("ExploderizePlanet");
		hasEnded = true;
	}

	void Stage0 () {
		RespawnDrill ();
		drill.SpawnTurrets ();
	}

	void Stage1 () {
		MissleEditor.current.TogglePartButton (2);
		RespawnDrill ();
		drill.SpawnTurrets ();
	}

	void Stage2 () {
		MissleEditor.current.TogglePartButton (7);
		RespawnDrill ();
		drill.SpawnTurrets ();
	}

	void Stage3 () {
		WinTheGame ();
	}

	public static void Pause () {
		Time.timeScale = 0f;
		Game.game.pauseMenu.SetActive (true);
	}

	public static void Resume () {
		Time.timeScale = 1f;
		Game.game.pauseMenu.SetActive (false);
	}

	public void QuitGame () {
		Application.Quit ();
	}

	public void RestartGame () {
		hasEnded = false;
		Application.LoadLevel (Application.loadedLevel);
		Time.timeScale = 1f;
	}

	IEnumerator ExploderizePlanet () {
		while (true) {
			if (Random.Range (0,20) == 0) planet.CreateExplosion (Random.Range (0, planet.radius * 2), Random.Range (0, planet.radius * 2), Random.Range (5,10), 9);
			yield return new WaitForFixedUpdate ();
		}
	}

	public void StartGame (bool withTutorial) {
		// HUD.SetActive (true);
		cameraController.enableMovement = true;
		startMenu.SetActive (false);
		if (!withTutorial) {
			flightHUD.SetActive (true);
		}
		hasStarted = true;
		if (withTutorial) tutorial.AskTutorial ();
	}

	public void OpenCredits () {
	}
}
