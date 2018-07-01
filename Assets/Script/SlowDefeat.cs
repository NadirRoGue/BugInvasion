using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class SlowDefeat : MonoBehaviour {

	private static readonly float _slowExitDuration = 3.0f; // seconds
	private float _current = _slowExitDuration;

	void Start () {
		Time.timeScale = 1.0f;
	}
	
	void Update () {

		_current -= Time.deltaTime;

		if (_current > 0.2f) {

			float timeScale = _current / _slowExitDuration;
			Time.timeScale = timeScale;
		} else {
			Time.timeScale = 0.0f;
			LevelManager.getInstance ().notifySlowDefeatEnd ();
			Destroy (gameObject);
			enabled = false;
		}
		 /*
		float nextTimeScale = Time.timeScale - Time.deltaTime;
		if (nextTimeScale > 0.0f) {
			Time.timeScale = nextTimeScale;
		} else {
			Time.timeScale = 0.0f;
			LevelManager.getInstance ().notifySlowDefeatEnd ();
			Destroy (gameObject);
			enabled = false;
		}
		*/
	}
}
