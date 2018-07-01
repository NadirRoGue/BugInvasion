using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class CountDownController : MonoBehaviour {

	public Canvas Canvas;
	public Text CountdownText;
	public Button SkipButton;

	private float _timeLeft = 20.0f;
	private bool _customEnabled = false;

	public static CountDownController INSTANCE {
		get;
		set;
	}

	public void initializeCountdown() {
		_timeLeft = 20.0f;
		_customEnabled = true;
		//Canvas.hideFlags = HideFlags.HideInHierarchy;
		Canvas.enabled = true;
	}

	void awake() { 
		/*if (INSTANCE == null) {
			INSTANCE = this;
			_customEnabled = true;
		}*/
	}

	void Start () {
		SkipButton.onClick.AddListener (onClickEvt);
		_customEnabled = true;
		if (INSTANCE == null) {
			INSTANCE = this;
		}
	}
	
	void Update () {
		if (_customEnabled) {
			if (_timeLeft > 0.0f) {
				_timeLeft -= Time.deltaTime;

				int integer = Mathf.RoundToInt (_timeLeft);

				CountdownText.text = "" + integer;

			} else {
				Canvas.enabled = false;
				Canvas.hideFlags = HideFlags.HideInHierarchy;
				_customEnabled = false;
				LevelManager.getInstance ().countDownFinished ();
			}
		}
	}

	public void onClickEvt() {
		_timeLeft = 0.0f;
		Canvas.enabled = false;
		Canvas.hideFlags = HideFlags.HideInHierarchy;
		_customEnabled = false;
		LevelManager.getInstance ().countDownFinished ();
	}
}
