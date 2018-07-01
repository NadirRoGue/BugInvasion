using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class QuitToMenu : MonoBehaviour {

	void Start () {
		GetComponent<Button> ().onClick.AddListener (onClickEvt);
	}

	// Update is called once per frame
	void Update () {

	}

	public void onClickEvt() {
		SceneManager.LoadScene ("main_menu");
	}
}
