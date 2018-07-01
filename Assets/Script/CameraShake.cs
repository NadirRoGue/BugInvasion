using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class CameraShake : MonoBehaviour {

	public Camera SourceCamera;

	public static CameraShake INSTANCE {
		get;
		set;
	}

	private float _cameraShakeInterval = 0.03f;
	private float _currentInterval = 0.0f;
	private float _prevShake = 0.0f;
	private float _shake = 0.9f;
	private float _accumulatedShake = 0.0f;
	private float _sign = 1.0f;

	private Quaternion _originalQ;

	private bool _onExecution = false;

	public void prepareShake() {
		_cameraShakeInterval = 0.03f;
		_currentInterval = 0.0f;
		 _prevShake = 0.0f;
		_shake = 0.9f;
		_accumulatedShake = _shake;
		_sign = 1.0f;

		_originalQ = SourceCamera.transform.rotation;

		_onExecution = true;
	}

	// Use this for initialization
	void Start () {
		if (INSTANCE == null) {
			INSTANCE = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_onExecution) {
			if (_shake > float.Epsilon) {

				if (_currentInterval >= _cameraShakeInterval) {

					_sign = -_sign;

					_prevShake = _shake;
					_shake = _shake - 0.1f;
					_accumulatedShake = _sign * (_prevShake + _shake);
					_currentInterval = 0.0f;
				} 

				if (_shake > 0.0f) {
					float deltaRot = (_accumulatedShake / _cameraShakeInterval) * Time.deltaTime;

					_currentInterval += Time.deltaTime;

					SourceCamera.transform.Rotate (new Vector3 (deltaRot, 0, 0));
				}
			} else {
				SourceCamera.transform.rotation = _originalQ; // precision correction
				_onExecution = false;
			}
		}
	}
}
