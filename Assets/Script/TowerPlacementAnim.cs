using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class TowerPlacementAnim : MonoBehaviour {

	private Vector3 _spawnPosition;
	private Vector3 _initPosition;

	private const float _towerWeightKg = 100.0f;
	private const float _gravityAcceleration = 9.81f;
	private float _velocity = 0.0f;

	private bool _targetReached = false;

	public void setParameters(Vector3 spawn) {
		_spawnPosition = spawn;
	}

	void Start () {
		_initPosition = _spawnPosition;
		_initPosition.y += 300.0f;
		transform.position = _initPosition;
	}
	
	void Update () {
		// Tower drop
		if (!_targetReached) {
			_velocity += (_towerWeightKg * _gravityAcceleration * Time.deltaTime);

			_initPosition.y -= _velocity * Time.deltaTime;

			transform.position = _initPosition;

			if (transform.position.y < _spawnPosition.y) {
				transform.position = _spawnPosition;
				_targetReached = true;
			}
		} else {
			CameraShake.INSTANCE.prepareShake ();
			enabled = false; // disable itself
		}
	}
}
