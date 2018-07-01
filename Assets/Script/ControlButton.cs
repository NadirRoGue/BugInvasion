using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public class ControlButton : MonoBehaviour {

	public Sprite NormalSprite;
	//public Sprite PressedSprite;
	public Sprite InUseSprite;

	private bool _isPressed = false;
	private Button _button;

	void Awake() {
		onAwake ();
	}

	// Use this for initialization
	void Start () {
		_button = GetComponent<Button> ();

		if (_button != null) {
			_button.onClick.AddListener (onClickEvt);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickEvt() {
		if (!_isPressed) {
			_button.GetComponent<Image> ().sprite = InUseSprite;
			_isPressed = true;
			buttonClicked ();
		} else {
			_button.GetComponent<Image> ().sprite = NormalSprite;
			_isPressed = false;
			buttonReleased ();
		}
	}

	public virtual void onAwake() {

	}

	public virtual void buttonClicked() {

	}

	public virtual void buttonReleased() {

	}
}
