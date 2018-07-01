using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class ParticleSystemCollector : MonoBehaviour {

	private float _timeLeft;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (!GetComponent<ParticleSystem> ().IsAlive ()) {
			Destroy (gameObject);
			enabled = false;
		} 
	}
}
