using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class FarViewManager : MonoBehaviour {

	void Start () {
		Material [] mats = { World.getInstance ().getWorldMaterial () };
		GetComponent<MeshRenderer> ().materials = mats;
	}
	
	void Update () {
		
	}
}
