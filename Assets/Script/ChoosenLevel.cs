using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class ChoosenLevel : MonoBehaviour {

	public string Level = "none";

	public static ChoosenLevel INSTANCE {
		get;
		set;
	}

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
		INSTANCE = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
