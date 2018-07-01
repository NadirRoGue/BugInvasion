using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class ShutDownHook : MonoBehaviour {

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit() {
		World.getInstance ().savePlayerData ();
	}
}
