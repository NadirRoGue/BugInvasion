using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class ScreenMessageController : MonoBehaviour {

	public Canvas Canvas;
	public Text TextObject;

	private float _messageTimeLeft = -1.0f;

	public static ScreenMessageController INSTANCE {
		get;
		set;
	}

	public void showTempMessage(string msg, float time) {
		Canvas.enabled = true;
		_messageTimeLeft = time;
		TextObject.text = msg;
		enabled = true;
	}

	public void showMessage(string msg) {
		Canvas.enabled = true;
		TextObject.text = msg;
		_messageTimeLeft = -1.0f;
		enabled = true;
	}

	void Start () {
		if (INSTANCE == null)
			INSTANCE = this;	
	}
	
	void Update () {
		if (_messageTimeLeft > 0.0f) {
			_messageTimeLeft -= Time.deltaTime;

			if (_messageTimeLeft <= 0.0f) {
				Canvas.enabled = false;
				enabled = false;
			}
		}
	}
}
