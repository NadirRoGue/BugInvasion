using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class EnemyDeathController : MonoBehaviour {

	private float _deathCountdown = 3.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		_deathCountdown -= Time.deltaTime;

		if (_deathCountdown <= 0.0f) {
			Destroy (gameObject);
			enabled = false;
		}
	}
}
