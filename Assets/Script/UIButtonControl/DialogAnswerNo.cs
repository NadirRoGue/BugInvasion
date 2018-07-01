using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class DialogAnswerNo : MonoBehaviour {

	public Canvas DialogCanvas;
	// Use this for initialization
	void Start () {
		GetComponent<Button> ().onClick.AddListener (onClickEvt);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickEvt() {
		DialogCanvas.enabled = false;
	}
}
