using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	public GameObject tutorialMenu;
	public GameObject[] tutorialSteps;
	public GameObject stepParent;
	private int progress;

	// Use this for initialization
	public void AskTutorial () {
		tutorialMenu.SetActive (true);
		Game.Resume ();
	}
	
	public void EndTutorial () {
		stepParent.SetActive (false);
		tutorialMenu.SetActive (false);

		Game.game.drill.enabled = true;
		Game.game.enabled = true;
		Game.game.editor.canInteract = true;
		Game.game.editor.canBuild = true;
		Game.cameraController.enableMovement = true;
		if (!Game.game.editorCamera.activeSelf) Game.game.flightHUD.SetActive (true);
	}

	public void ContinueTutorial () {
		tutorialMenu.SetActive (false);
		stepParent.SetActive (true);
		Game.game.drill.enabled = false;
		Game.game.enabled = false;
		Game.game.editor.canInteract = false;
		Game.game.editor.canBuild = false;
		Game.cameraController.enableMovement = false;
		NextStep ();
	}

	void Update () {
		if (Input.GetButtonDown ("Submit")) {
			NextStep ();
		}
		if (Input.GetButtonDown ("Cancel")) {
			EndTutorial ();
		}
	}

	public void NextStep () {
		if (progress > 0) tutorialSteps[progress-1].SetActive (false);
		if (progress < tutorialSteps.Length) tutorialSteps[progress].SetActive (true);
		SendMessage ("Step" + progress.ToString (), SendMessageOptions.DontRequireReceiver);
		progress++;
	}

	void Step1 () {
		Game.cameraController.MoveToPosition (Planet.current.center, 0, Planet.current.radius);
	}

	void Step2 () {
		Game.cameraController.MoveToPosition (new Vector3 (Planet.current.radius, -1), 180f, 3f);
	}

	void Step4 () {
		Game.game.editor.OpenEditor (false);
	}

	void Step7 () {
		MissleEditor e = Game.game.editor;
		e.ForcePlacePart (e.currentParts[0], new Vector3 (0, -0.5f), 0);
		e.ForcePlacePart (e.currentParts[1], new Vector3 (0, -1f), 0);
		e.ForcePlacePart (e.currentParts[2], new Vector3 (0, -1.5f), 0);
		e.ForcePlacePart (e.currentParts[3], new Vector3 (0.5f, -1.5f), 1);
		e.ForcePlacePart (e.currentParts[3], new Vector3 (-0.5f, -1.5f), 1);
		e.ForcePlacePart (e.currentParts[0], new Vector3 (0f, 0.5f), 2);
		Game.game.editor.canBuild = true;
	}

	void Step13 () {
		EndTutorial ();
	}
}
