using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class PanelTransitionButton : MonoBehaviour {

	public Canvas _source;
	public Canvas _target;

	void Start () {
		GetComponent<Button> ().onClick.AddListener (onClickEvt);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void onClickEvt() {
		_source.enabled = false;
		_target.enabled = true;
	}
}
