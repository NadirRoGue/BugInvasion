using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class AmmunitionController : MonoBehaviour {

	public Ammunition _ammunition {
		get;
		set;
	}

	public Vector3 _spawnPos {
		get;
		set;
	}

	public Vector3 _hitPos {
		get;
		set;
	}

	private Vector3 _dirVector;
	private Creature _effector;

	public void setKeyPos(Vector3 source, Vector3 dest, Creature effector) {
		_spawnPos = source;
		_hitPos = dest;
		_effector = effector;
	}

	// Use this for initialization
	void Start () {
		transform.position = _spawnPos;

		_dirVector = _hitPos - _spawnPos;
		_dirVector.Normalize ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += _dirVector * _ammunition._launchSpeed * Time.deltaTime;
	}

	public Creature getEffector() {
		return _effector;
	}
}
