using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class LevelSelection : MonoBehaviour {

	public string Level;
	public bool IsRetry = false;

	// Use this for initialization
	void Start () {
		Button button = GetComponent<Button> ();

		if (button != null) {
			button.onClick.AddListener (onClickEvt);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickEvt() {
		if (!IsRetry) {
			ChoosenLevel.INSTANCE.Level = Level;
		}
		SceneManager.LoadScene ("map_base");
	}
}
