using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class MouseSensitivitySlider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Slider> ().onValueChanged.AddListener (onValueChangedEvt);
		GetComponent<Slider> ().normalizedValue = Config.MOUSE_SENSITIVITY;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onValueChangedEvt(float value) {
		Config.MOUSE_SENSITIVITY = GetComponent<Slider> ().normalizedValue;
	}
}
