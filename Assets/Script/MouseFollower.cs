using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public sealed class MouseFollower : MonoBehaviour {

	public Camera MyCamera;

	private Transform _yaw;
	private Transform _pitch;

	private int _nextTargetYaw;
	private int _nextTargetPitch;

	private int _accumulatedYaw;
	private int _accumulatedPitch;

	private float _fovy;
	private float _fovx;

	private float _yawCorrection = 0.0f;

	void Start () {
		_yaw = GetComponent<Transform> ().Find ("control_bone/yaw_bone");
		_pitch = _yaw.Find ("pitch_bone");

		_fovy = MyCamera.fieldOfView * Mathf.Deg2Rad;
		_fovx = 2 * Mathf.Atan (Mathf.Tan (_fovy / 2) * MyCamera.aspect);

		//_towerRotation = transform.rotation.eulerAngles.y;

		Vector3 towerToCam = (MyCamera.transform.position - transform.position).normalized;
		_yawCorrection = Mathf.Acos (Vector3.Dot (towerToCam, transform.forward)) * Mathf.Rad2Deg;
	}
	
	void Update () {

		// Transform mouse screen position into a 3D point 
		Vector3 mousePos = Input.mousePosition;

		Vector2 screenCoordiantes = new Vector2 (mousePos.x, mousePos.y);
		screenCoordiantes = normalizeScreenCoordinates (screenCoordiantes);

		Vector3 camPos = MyCamera.transform.position;
		float near = MyCamera.nearClipPlane;

		float bySin = (near / Mathf.Cos (_fovy)) * Mathf.Sin(_fovy);
		float byCos = (near / Mathf.Cos(_fovx)) * Mathf.Sin(_fovx);

		// Exagerate point in the XY plane to get angles big enough to trigger a movement

		Vector3 point = new Vector3 (
			                camPos.x + (byCos * screenCoordiantes.x * 1.0f), 
			                camPos.y + (bySin * screenCoordiantes.y * 10.0f),
			                camPos.z + near + 0.01f);

		// Transform the point into polar coordinates to use the angles to control the cannon
		Vector3 delta = point - transform.position;
		float r = delta.magnitude;
		float ro = Mathf.Acos (delta.x / r) * Mathf.Rad2Deg;
		float phi = Mathf.Atan (delta.y / r) * Mathf.Rad2Deg;

		Quaternion yawQ = _yaw.rotation;
		Vector3 yawEuler = yawQ.eulerAngles;
		yawEuler.y = -ro - _yawCorrection;
		yawQ = Quaternion.Euler (yawEuler);
		_yaw.rotation = yawQ;

		Quaternion pitchQ = _pitch.rotation;
		Vector3 pitchEuler = pitchQ.eulerAngles;
		pitchEuler.x = -phi;
		pitchQ = Quaternion.Euler (pitchEuler);
		_pitch.rotation = pitchQ;
	}

	// Normalize screen coordinates within -1,1 range
	private Vector2 normalizeScreenCoordinates(Vector2 screenCoords) {
		if (screenCoords.x < 0) {
			screenCoords.x = -1;
		} else if (screenCoords.x > Screen.currentResolution.width) {
			screenCoords.x = 1;
		} else {
			int halfWidth = Screen.currentResolution.width / 2;
			screenCoords.x = (screenCoords.x - halfWidth) / halfWidth;
		}

		if (screenCoords.y < 0) {
			screenCoords.y = -1;
		} else if (screenCoords.y > Screen.currentResolution.height) {
			screenCoords.y = 1;
		} else {
			int halfHeight = Screen.currentResolution.height / 2;
			screenCoords.y = (screenCoords.y - halfHeight) / halfHeight;
		}

		return screenCoords;
	}
}
