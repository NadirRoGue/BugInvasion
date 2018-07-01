using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class AssetHolderScript : MonoBehaviour {

	public Canvas CountdownCanvas;
	public Canvas MessageCanvas;
	public Canvas EndGameCanvas;

	public static AssetHolderScript INSTANCE;

	void Awake() {
		if (INSTANCE == null)
			INSTANCE = this;
	}

	void Start () {
		if (INSTANCE == null)
			INSTANCE = this;
	}
	
	void Update () {
		
	}
}
