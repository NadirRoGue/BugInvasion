using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class PauseGame : MonoBehaviour {

	public Canvas PauseCanvas;
	public Canvas GameCanvas;

	void Start () {
	}
	
	void Update () {

		if (LevelManager.getInstance().isGameActive() && !PauseCanvas.enabled && Input.GetKeyDown(KeyCode.Escape)) {
			LevelManager.getInstance ().setGamePaused (true);
			Constants.GAME_TIME_SCALE_CACHE = Time.timeScale;
			Time.timeScale = 0.0f;
			GameCanvas.enabled = false;
			PauseCanvas.enabled = true;

			PickController.hidePlacementMenu ();
		}
	}
}
