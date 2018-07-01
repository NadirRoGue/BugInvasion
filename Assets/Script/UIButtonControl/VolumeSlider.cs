using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class VolumeSlider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Slider> ().onValueChanged.AddListener (onValueChangedEvt);
		GetComponent<Slider> ().normalizedValue = Config.SOUND_VOLUME;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onValueChangedEvt(float value) {
		Config.SOUND_VOLUME = GetComponent<Slider> ().normalizedValue;
		SoundTable.getInstance ().scalePlayers ();
	}
}
