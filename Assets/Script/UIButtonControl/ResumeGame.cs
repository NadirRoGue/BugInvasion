using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class ResumeGame : MonoBehaviour {

	public Canvas PauseCanvas;
	public Canvas GameCanvas;

	void Start () {
		GetComponent<Button> ().onClick.AddListener (onClickEvt);
	}

	// Update is called once per frame
	void Update () {

	}

	public void onClickEvt() {
		LevelManager.getInstance ().setGamePaused (false);
		PauseCanvas.enabled = false;
		GameCanvas.enabled = true;
		Time.timeScale = Constants.GAME_TIME_SCALE_CACHE;
	}
}
