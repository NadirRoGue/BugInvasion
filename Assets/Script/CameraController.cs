using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class CameraController : MonoBehaviour {

	public Camera PlayerView;

	private const float _movementSpeed = 50.0f;
	private const float _rotationSpeed = 50.0f;

	//private int _screenWidth;
	//private int _screenHeight;

	private float _minX, _maxX;
	private float _minZ, _maxZ;

	private const int MOUSE_BUTTON_LEFT = 0;
	private const int MOUSE_BUTTON_RIGHT = 1;

	private Vector2 _previousPosition;
	private bool _resettedPan = true;

	private bool _ressetedRot = true;

	void Start () {
		//_screenWidth = Screen.width / 2;
		//_screenHeight = Screen.height / 2;

		int terrainWidth = World.getInstance ().getWorldWidth ();
		int terrainHeight = World.getInstance ().getWorldHeight ();

		_minX = 0.0f;
		_maxX = terrainWidth * Constants.VERTEX_SPACING;

		float tenPercentX = _maxX * 0.1f;
		_minX += tenPercentX;
		_maxX -= tenPercentX;

		_minZ = 0.0f;
		_maxZ = terrainHeight * Constants.VERTEX_SPACING;

		float tenPercentZ = _maxZ * 0.1f;
		_minZ += tenPercentZ;
		_maxZ -= tenPercentZ;

		float mapXCenter = (terrainHeight / 2.0f) * Constants.VERTEX_SPACING;
		float mapZCenter = (terrainWidth / 2.0f) * Constants.VERTEX_SPACING;

		PlayerView.transform.position = new Vector3 (mapXCenter, Constants.NOISE_HEIGHT_MULTIPLIER / 2.0f, mapZCenter);			
	}
	
	void Update () {

		if (Input.GetMouseButton (MOUSE_BUTTON_LEFT)) {
			_ressetedRot = true;

			Vector3 mousePos = Input.mousePosition;
			Vector2 realPos = new Vector2 (mousePos.x, mousePos.y);

			if (_resettedPan) {
				_previousPosition = realPos;
				_resettedPan = false;
			} else {
				Vector2 delta = realPos - _previousPosition;

				Vector3 pos = PlayerView.transform.position;

				// The camera is rotated about 50º on X to look at the terrain
				// Y coordinate from forward vector must be suppressed to avoid zoom-in and zoom-out when panning
				Vector3 nonYForward = PlayerView.transform.forward;
				nonYForward.y = 0.0f;

				Vector3 zMov = nonYForward * (delta.y * _movementSpeed * Config.MOUSE_SENSITIVITY * Time.deltaTime);
				Vector3 xMov = PlayerView.transform.right * (delta.x * _movementSpeed * Config.MOUSE_SENSITIVITY * Time.deltaTime);

				pos -= (zMov + xMov);

				pos.x = Mathf.Max (Mathf.Min (pos.x, _maxX), _minX);
				pos.z = Mathf.Max (Mathf.Min (pos.z, _maxZ), _minZ);

				PlayerView.transform.position = pos;

				_previousPosition = realPos;
			}
		} else if (Input.GetMouseButton (MOUSE_BUTTON_RIGHT)) {
			_resettedPan = true;

			Vector3 mousePos = Input.mousePosition;
			Vector2 realPos = new Vector2 (mousePos.x, mousePos.y);

			if (_ressetedRot) {
				_previousPosition = realPos;
				_ressetedRot = false;
			} else {
				float deltaX = realPos.x - _previousPosition.x;

				Quaternion q = PlayerView.transform.rotation;
				Vector3 euler = q.eulerAngles;
				euler.y += (deltaX * _rotationSpeed * Config.MOUSE_SENSITIVITY * Time.deltaTime);
				q = Quaternion.Euler (euler);
				PlayerView.transform.rotation = q;

				_previousPosition = realPos;
			}

		} else {
			_resettedPan = _ressetedRot = true;
		}
	}
}
